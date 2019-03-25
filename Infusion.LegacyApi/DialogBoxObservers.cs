using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class DialogBoxObservers
    {
        private readonly UltimaServer server;
        private readonly EventJournalSource eventJournalSource;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly object blockedQuestionLock = new object();

        private string blockedQuestion;

        internal bool ShowDialogBox { get; set; } = true;
        public DialogBox CurrentDialogBox { get; private set; }

        internal event Action<DialogBox> DialogBoxOpened;

        public DialogBoxObservers(UltimaServer server, EventJournalSource eventJournalSource, PacketDefinitionRegistry packetRegistry)
        {
            this.server = server;
            this.eventJournalSource = eventJournalSource;
            this.packetRegistry = packetRegistry;
            server.RegisterFilter(FilterServerPackets);
        }

        private Packet? FilterServerPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.OpenDialogBox.Id)
            {
                var packet = packetRegistry.Materialize<OpenDialogBoxPacket>(rawPacket);
                var dialogBox = new DialogBox(packet.DialogId, packet.MenuId, packet.Question, packet.Responses);
                CurrentDialogBox = dialogBox;
                bool showDialogBox = ShowDialogBox;

                bool block = false;
                lock (blockedQuestionLock)
                {
                    if (!string.IsNullOrEmpty(blockedQuestion) && dialogBox.Question.IndexOf(blockedQuestion, StringComparison.Ordinal) >= 0)
                    {
                        blockedQuestion = null;
                        block = true;
                    }
                }

                eventJournalSource.Publish(new DialogBoxOpenedEvent(dialogBox));
                DialogBoxOpened?.Invoke(dialogBox);

                if (!showDialogBox || block)
                    return null;
            }

            return rawPacket;
        }

        internal bool TriggerDialogBox(string responseText)
        {
            var response = CurrentDialogBox?.Responses.FirstOrDefault(x => x.Text.Contains(responseText));
            if (response == null)
                return false;

            TriggerDialogBox(response);

            return true;
        }

        internal bool TriggerDialogBox(byte responseIndex)
        {
            var response = CurrentDialogBox?.Responses.FirstOrDefault(x => x.Index == responseIndex);
            if (response == null)
                return false;

            TriggerDialogBox(response);

            return true;
        }

        private void TriggerDialogBox(DialogBoxResponse response)
        {
            server.DialogBoxResponse(CurrentDialogBox.DialogId, CurrentDialogBox.MenuId, response.Index, response.Type, response.Color);

            CurrentDialogBox = null;
        }

        internal void BlockQuestion(string question)
        {
            lock (blockedQuestionLock)
            {
                blockedQuestion = question;
            }
        }

        internal void UnblockQuestion()
        {
            lock (blockedQuestionLock)
            {
                blockedQuestion = null;
            }
        }

        internal void CloseDialogBox()
        {
            if (CurrentDialogBox != null)
            {
                server.DialogBoxResponse(CurrentDialogBox.DialogId, CurrentDialogBox.MenuId, 0, 0, (Color)0);
                CurrentDialogBox = null;
            }
        }
    }
}
