using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class QuestArrowObserver
    {
        private readonly IEventJournalSource eventJournalSource;
        public event EventHandler<QuestArrowEvent> QuestArrowChanged;

        public QuestArrowObserver(IServerPacketSubject serverPacketSubject, IEventJournalSource eventJournalSource)
        {
            this.eventJournalSource = eventJournalSource;
            serverPacketSubject.Subscribe(PacketDefinitions.QuestArrow, HandleQuestArrow);
        }

        private void HandleQuestArrow(QuestArrowPacket packet)
        {
            var questArrowEvent = new QuestArrowEvent(packet.Active, packet.Location);
            eventJournalSource.Publish(questArrowEvent);
            QuestArrowChanged.RaiseScriptEvent(this, questArrowEvent);
        }

        public void ResetEvents()
        {
            QuestArrowChanged = null;
        }
    }
}
