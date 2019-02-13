using System;
using System.Net;

namespace Infusion.Proxy
{
    public class ProxyStartConfig
    {
        public IPEndPoint ServerEndPoint { get; set; }
        public string ServerAddress { get; set; }
        public ushort LocalProxyPort { get; set; } = 33333;
        public Version ProtocolVersion { get; set; }
    }
}