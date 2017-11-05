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
        internal bool ShowDialogBox { get; set; } = true;
        public DialogBox CurrentDialogBox { get; private set; }

        public DialogBoxObservers(UltimaServer server, EventJournalSource eventJournalSource)
        {
            this.server = server;
            this.eventJournalSource = eventJournalSource;
            server.RegisterFilter(FilterServerPackets);
        }

        private Packet? FilterServerPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.OpenDialogBox.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<OpenDialogBoxPacket>(rawPacket);
                var dialogBox = new DialogBox(packet.DialogId, packet.MenuId, packet.Question, packet.Responses);
                CurrentDialogBox = dialogBox;
                eventJournalSource.Publish(new DialogBoxOpenedEvent(dialogBox));

                if (!ShowDialogBox)
                    return null;
            }

            return rawPacket;
        }

        internal void TriggerDialogBox(string responseText)
        {
            if (CurrentDialogBox == null)
                throw new LegacyException("No dialog box found.");

            var response = CurrentDialogBox.Responses.FirstOrDefault(x => x.Text.Contains(responseText));
            if (response == null)
                throw new LegacyException($"Cannot find {responseText} in current dialog box. Responses are: {CurrentDialogBox.ResponseTexts}");

            TriggerDialogBox(response);
        }

        internal void TriggerDialogBox(byte responseIndex)
        {
            if (CurrentDialogBox == null)
                throw new LegacyException("No dialog box found.");

            var response = CurrentDialogBox.Responses.FirstOrDefault(x => x.Index == responseIndex);
            if (response == null)
                throw new LegacyException($"Cannot find {responseIndex} in current dialog box. Responses are: {CurrentDialogBox.ResponseTexts}");

            TriggerDialogBox(response);
        }

        private void TriggerDialogBox(DialogBoxResponse response)
        {
            server.DialogBoxResponse(CurrentDialogBox.DialogId, CurrentDialogBox.MenuId, response.Index, response.Type, response.Color);

            CurrentDialogBox = null;
        }
    }
}
