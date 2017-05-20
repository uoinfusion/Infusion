using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher
{
    public class LauncherOptions
    {
        private bool defaultInitialScriptTestPerformed;
        private string defaultInitialScriptFileName;
        private string initialScriptFileName;

        public string ServerEndpoint { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ushort ProxyPort { get; set; } = 0;

        public string InitialScriptFileName
        {
            get
            {
                if (string.IsNullOrEmpty(initialScriptFileName))
                {
                    if (!defaultInitialScriptTestPerformed)
                    {
                        string assemblyFileName = typeof(LauncherOptions).Assembly.Location;
                        if (!string.IsNullOrEmpty(assemblyFileName))
                        {
                            string assemblyPath = Path.GetDirectoryName(assemblyFileName);;
                            if (!string.IsNullOrEmpty(assemblyPath))
                            {
                                string scriptFileName = Path.Combine(assemblyPath, @"..\scripts\startup.csx");
                                if (File.Exists(scriptFileName))
                                    defaultInitialScriptFileName = scriptFileName;
                            }
                        }

                        defaultInitialScriptTestPerformed = true;
                    }

                    return defaultInitialScriptFileName;
                }

                return initialScriptFileName;
            }
            set => initialScriptFileName = value;
        }

        public string EncryptPassword()
        { 
            if (string.IsNullOrEmpty(Password))
                return null;

            StringBuilder encryptedPassword = new StringBuilder(Password.Length);

            foreach (char t in Password)
            {
                int c = t;
                c += 13;
                if (c > 126)
                    c -= 95;
                if (c == 32) c = 127;
                encryptedPassword.Append((char) c);
            }

            return encryptedPassword.ToString();
        }

        public async Task<IPEndPoint> ResolveServerEndpoint()
        {
            var parts = ServerEndpoint.Split(',').Select(x => x.Trim()).ToArray();

            IPAddress address;
            if (!IPAddress.TryParse(parts[0], out address))
            {
                var entry = await Dns.GetHostEntryAsync(parts[0]);
                address = entry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
            ushort port = parts.Length > 1 ? ushort.Parse(parts[1]) : (ushort)2593;

            return new IPEndPoint(address, port);
        }

        public bool Validate(out string validationMessage)
        {
            if (string.IsNullOrEmpty(ServerEndpoint))
            {
                validationMessage = "Please enter server address, including port. For example: server.uoerebor.com,2593";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }

        public ushort ResolveProxyPort()
        {
            if (ProxyPort > 0)
                return ProxyPort;

            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return (ushort)port;
        }
    }
}
