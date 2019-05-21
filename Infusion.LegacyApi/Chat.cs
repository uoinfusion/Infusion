using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.LegacyApi
{
    public sealed class Chat
    {
        private readonly UltimaServer ultimaServer;

        internal Chat(UltimaServer ultimaServer)
        {
            this.ultimaServer = ultimaServer;
        }

        public void SendMessage(string message) => this.ultimaServer.SendChatMessage(message);
    }
}
