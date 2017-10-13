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
        public event EventHandler<QuestArrowEvent> QuestArrowChanged;

        public QuestArrowObserver(IServerPacketSubject serverPacketSubject)
        {
            serverPacketSubject.Subscribe(PacketDefinitions.QuestArrow, HandleQuestArrow);
        }

        private void HandleQuestArrow(QuestArrowPacket packet)
        {
            QuestArrowChanged.RaiseScriptEvent(this, new QuestArrowEvent(packet.Active, packet.Location));   
        }

        public void ResetEvents()
        {
            QuestArrowChanged = null;
        }
    }
}
