using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;

namespace Infusion.LegacyApi
{
    internal sealed class Targeting
    {
        private readonly Cancellation cancellation;
        private readonly UltimaClient client;
        private readonly EventJournalSource eventSource;

        private readonly Queue<TargetInfo> nextTargets = new Queue<TargetInfo>();
        private readonly object nextTargetsLock = new object();
        private readonly AutoResetEvent receivedTargetInfoEvent = new AutoResetEvent(false);
        private readonly UltimaServer server;
        private readonly AutoResetEvent targetFromServerReceivedEvent = new AutoResetEvent(false);
        private bool discardNextTargetLocationRequestIfEmpty;
        private readonly EventJournal eventJournal;

        private DateTime lastActionTime;
        private CursorId lastCursorId = new CursorId(0x00000025);
        private ObjectId? lastItemIdInfo;
        private DateTime lastTargetCursorPacketTime;

        private TargetInfo? lastTargetInfo;
        private ModelId lastTypeInfo;
        private bool targetInfoRequested;

        public Targeting(UltimaServer server, UltimaClient client, Cancellation cancellation,
            EventJournalSource eventSource)
        {
            this.server = server;
            this.client = client;
            this.cancellation = cancellation;
            this.eventSource = eventSource;
            server.Subscribe(PacketDefinitions.TargetCursor, HanldeServerTargetCursorPacket);
            eventJournal = new EventJournal(eventSource, cancellation);

            IClientPacketSubject clientPacketSubject = client;
            clientPacketSubject.RegisterFilter(FilterClientTargetCursorPacket);

            IServerPacketSubject serverPacketSubject = server;
            serverPacketSubject.RegisterFilter(FilterSeverTargetCursorPacket);
        }

        internal AutoResetEvent WaitForTargetStartedEvent { get; } = new AutoResetEvent(false);
        internal AutoResetEvent AskForTargetStartedEvent { get; } = new AutoResetEvent(false);
        internal event Action<TargetInfo?> TargetInfoReceived;

        private void HanldeServerTargetCursorPacket(TargetCursorPacket packet)
        {
            targetFromServerReceivedEvent.Set();
            lastCursorId = packet.CursorId;
            lastTargetCursorPacketTime = DateTime.UtcNow;
            eventSource.Publish(new ServerRequestedTargetEvent(packet.CursorId));
        }

        private Packet? FilterSeverTargetCursorPacket(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
            {
                TargetInfo? targetInfo = null;

                lock (nextTargetsLock)
                {
                    if (nextTargets.Count > 0)
                    {
                        targetInfo = nextTargets.Dequeue();
                    }
                }

                if (targetInfo.HasValue && targetInfo.Value.Id.HasValue)
                {
                    var packet = PacketDefinitionRegistry.Materialize<TargetCursorPacket>(rawPacket);

                    server.TargetItem(packet.CursorId, targetInfo.Value.Id.Value, packet.CursorType,
                        targetInfo.Value.Location, targetInfo.Value.ModelId);

                    return null;
                }
            }

            return rawPacket;
        }

        private Packet? FilterClientTargetCursorPacket(Packet rawPacket)
        {
            if (rawPacket.Id != PacketDefinitions.TargetCursor.Id)
                return rawPacket;

            var packet = PacketDefinitionRegistry.Materialize<TargetCursorPacket>(rawPacket);

            if (discardNextTargetLocationRequestIfEmpty)
            {
                discardNextTargetLocationRequestIfEmpty = false;
                if (packet.Location.X == 0xFFFF && packet.Location.Y == 0xFFFF &&
                    packet.ClickedOnId.Value == 0)
                {
                    return null;
                }
            }

            if (packet.CursorId == new CursorId(0xDEADBEEF))
            {
                if (packet.Location.X == 0xFFFF && packet.Location.Y == 0xFFFF &&
                    packet.ClickedOnId.Value == 0)
                {
                    lastTargetInfo = null;
                    lastItemIdInfo = null;
                    receivedTargetInfoEvent.Set();
                    TargetInfoReceived.Invoke(null);
                    return null;
                }

                switch (packet.CursorTarget)
                {
                    case CursorTarget.Location:
                        lastTargetInfo = new TargetInfo(packet.Location, TargetType.Tile, packet.ClickedOnType.Value,
                            null);
                        break;
                    case CursorTarget.Object:
                        lastTypeInfo = packet.ClickedOnType;
                        lastItemIdInfo = packet.ClickedOnId;
                        lastTargetInfo = new TargetInfo(packet.Location, TargetType.Object, packet.ClickedOnType,
                            packet.ClickedOnId);
                        break;
                }

                receivedTargetInfoEvent.Set();
                TargetInfoReceived.Invoke(lastTargetInfo);
                return null;
            }

            return rawPacket;
        }

        public void WaitForTarget(TimeSpan timeout)
        {
            if (lastTargetCursorPacketTime > default(DateTime) || lastActionTime > default(DateTime))
            {
                if (lastTargetCursorPacketTime > lastActionTime)
                    return;
            }

            ClearNextTarget();
            targetFromServerReceivedEvent.Reset();
            var totalWaitingMillieseconds = 0;

            WaitForTargetStartedEvent.Set();

            while (!targetFromServerReceivedEvent.WaitOne(100))
            {
                totalWaitingMillieseconds += 100;
                if (timeout.TotalMilliseconds < totalWaitingMillieseconds)
                    throw new TimeoutException($"WaitForTarget timeout after {timeout}.");

                cancellation?.Check();
            }
        }

        public bool WaitForTarget(TimeSpan timeout, params string[] failMessages)
        {
            if (failMessages.Any())
            {
                bool result = false;

                eventJournal.When<ServerRequestedTargetEvent>(e => result = true)
                    .When<SpeechReceivedEvent>(e => failMessages.Any(msg => e.Speech.Text.Contains(msg)),
                        e => result = false)
                    .WaitAny(timeout);

                return result;
            }

            WaitForTarget(timeout);

            return true;
        }

        public TargetInfo? Info()
        {
            try
            {
                lastTargetInfo = null;
                targetInfoRequested = true;
                AskForTarget();

                while (!receivedTargetInfoEvent.WaitOne(10))
                {
                    cancellation?.Check();
                }

                return lastTargetInfo;
            }
            finally
            {
                targetInfoRequested = false;
            }
        }

        public void TargetTile(Location3D location, ModelId tileType)
        {
            if (targetInfoRequested)
            {
                lastTargetInfo = new TargetInfo(location, TargetType.Tile, tileType, null);
                receivedTargetInfoEvent.Set();
            }
            else
            {
                server.TargetLocation(lastCursorId, location, tileType, CursorType.Harmful);
            }

            discardNextTargetLocationRequestIfEmpty = true;
            client.TargetLocation(lastCursorId, location, tileType, CursorType.Cancel);
        }

        public void TargetTile(string tileInfo)
        {
            var errorMessage =
                $"Invalid tile info: '{tileInfo}'. Expecting <type> <xloc> <yloc> <zloc>. All numbers has to be decimal. Example: 3295 982 1007 0";
            var parts = tileInfo.Split(' ');
            if (parts.Length != 4)
            {
                throw new InvalidOperationException(errorMessage);
            }

            ushort rawType;
            if (!ushort.TryParse(parts[0], out rawType))
                throw new InvalidOperationException(errorMessage);

            int xloc;
            if (!int.TryParse(parts[1], out xloc))
                throw new InvalidOperationException(errorMessage);

            int yloc;
            if (!int.TryParse(parts[2], out yloc))
                throw new InvalidOperationException(errorMessage);

            int zloc;
            if (!int.TryParse(parts[3], out zloc))
                throw new InvalidOperationException(errorMessage);

            TargetTile(xloc, yloc, zloc, rawType);
        }

        public void TargetTile(int xloc, int yloc, int zloc, ModelId tileType)
        {
            TargetTile(new Location3D(xloc, yloc, zloc), tileType);
        }

        public void Target(TargetInfo targetInfo)
        {
            switch (targetInfo.Type)
            {
                case TargetType.Object:
                    if (targetInfo.Id.HasValue)
                    {
                        Target(targetInfo.Id.Value, targetInfo.ModelId, targetInfo.Location);
                    }

                    return;
                case TargetType.Tile:
                    TargetTile(targetInfo.Location.X, targetInfo.Location.Y, targetInfo.Location.Z, targetInfo.ModelId);
                    return;
                default:
                    throw new NotSupportedException($"TartetType {targetInfo.Type} not supported.");
            }
        }

        public void Target(Player player)
        {
            Target(player.PlayerId, player.BodyType, player.Location);
        }

        public void Target(ObjectId id)
        {
            Target(id, 0, new Location3D(0, 0, 0));
        }

        private void Target(ObjectId itemId, ModelId type, Location3D location)
        {
            if (targetInfoRequested)
            {
                lastItemIdInfo = itemId;
                lastTypeInfo = type;
                lastTargetInfo = new TargetInfo(location, TargetType.Object, type, itemId);
                receivedTargetInfoEvent.Set();
            }
            else
            {
                server.TargetItem(lastCursorId, itemId, CursorType.Harmful, location, type);
            }

            discardNextTargetLocationRequestIfEmpty = true;
            client.CancelTarget(lastCursorId, itemId, location, type);

            lock (nextTargetsLock)
                nextTargets.Clear();
        }

        public void Target(GameObject gameObject)
        {
            Target(gameObject.Id, gameObject.Type, gameObject.Location);
        }

        public void AskForTarget()
        {
            receivedTargetInfoEvent.Reset();
            targetFromServerReceivedEvent.Reset();
            ClearNextTarget();

            client.TargetCursor(CursorTarget.Location, new CursorId(0xDEADBEEF), CursorType.Neutral);
        }


        public ObjectId? ItemIdInfo()
        {
            try
            {
                targetInfoRequested = true;
                AskForTarget();

                var originalTime = lastTargetCursorPacketTime;

                AskForTargetStartedEvent.Set();

                int receivedTargetInfoEventIndex = 1;
                while (WaitHandle.WaitAny(new WaitHandle[] { this.targetFromServerReceivedEvent, receivedTargetInfoEvent }, 10) != receivedTargetInfoEventIndex)
                {
                    if (originalTime != lastTargetCursorPacketTime)
                        return null;

                    cancellation?.Check();
                }

                return lastItemIdInfo;
            }
            finally
            {
                targetInfoRequested = false;
            }
        }

        public TargetInfo? LocationInfo()
        {
            AskForTarget();

            var originalTime = lastTargetCursorPacketTime;

            AskForTargetStartedEvent.Set();

            int receivedTargetInfoEventIndex = 1;
            while (WaitHandle.WaitAny(new WaitHandle[] { this.targetFromServerReceivedEvent, receivedTargetInfoEvent }, 10) != receivedTargetInfoEventIndex)
            {
                if (originalTime != lastTargetCursorPacketTime)
                    return null;

                cancellation?.Check();
            }

            return lastTargetInfo;
        }

        public void NotifyLastAction(DateTime lastActionTime)
        {
            this.lastActionTime = lastActionTime;
        }

        public void AddNextTarget(Item[] items)
        {
            lock (nextTargetsLock)
            {
                nextTargets.Clear();
                foreach (var item in items)
                {
                    nextTargets.Enqueue(new TargetInfo(item.Location, TargetType.Object, item.Type, item.Id));
                }
            }
        }

        public void AddNextTarget(Mobile[] mobiles)
        {
            lock (nextTargetsLock)
            {
                nextTargets.Clear();
                foreach (var mobile in mobiles)
                {
                    nextTargets.Enqueue(
                        new TargetInfo(mobile.Location, TargetType.Object, mobile.Type, mobile.Id));
                }
            }
        }

        public void AddNextTarget(Player player)
        {
            lock (nextTargetsLock)
            {
                nextTargets.Clear();
                nextTargets.Enqueue(
                    new TargetInfo(player.Location, TargetType.Object, player.BodyType, player.PlayerId));
            }
        }

        public void AddNextTarget(ObjectId[] ids)
        {
            lock (nextTargetsLock)
            {
                nextTargets.Clear();
                foreach (var id in ids)
                    nextTargets.Enqueue(new TargetInfo(new Location3D(0, 0, 0), TargetType.Object, 0, id));
            }
        }

        public void ClearNextTarget()
        {
            lock (nextTargetsLock)
                nextTargets.Clear();
        }
    }
}