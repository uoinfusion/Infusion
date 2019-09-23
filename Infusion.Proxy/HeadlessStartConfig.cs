using Infusion.Packets.Client;
using System;
using System.Net;

namespace Infusion.Proxy
{
    public class HeadlessStartConfig
    {
        public Version ClientVersion { get; set; }
        public EncryptionSetup Encryption { get; set; }
        public string ServerAddress { get; set; }
        public IPEndPoint ServerEndPoint { get; set; }

        public string ShardName { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string CharacterName { get; set; }
    }
}