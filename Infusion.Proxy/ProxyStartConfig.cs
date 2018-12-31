using System;
using System.Net;

namespace Infusion.Proxy
{
    public class ProxyStartConfig
    {
        public IPEndPoint ServerAddress { get; set; }
        public ushort LocalProxyPort { get; set; } = 33333;
        public bool Encrypted { get; set; } = true;
        public Version ProtocolVersion { get; set; }
    }
}