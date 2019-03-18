using Infusion.Proxy;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ultima;

namespace Infusion.Desktop.Launcher
{
    public class LauncherOptions
    {
        private bool defaultInitialScriptTestPerformed;
        private string defaultInitialScriptFileName;
        private string initialScriptFileName;

        public string ServerEndpoint { get; set; }
        public UltimaClientType ClientType { get; set; } = UltimaClientType.Classic;
        public Version ProtocolVersion { get; set; } = new Version(3, 0, 0);
        public EncryptionSetup Encryption
        {
            get
            {
                switch (ClientType)
                {
                    case UltimaClientType.Classic:
                        return Classic.Encryption;
                    default:
                        return EncryptionSetup.Autodetect;
                }
            }
        }

        public string UserName { get; set; }
        public string Password { get; set; }

        public OrionLauncherOptions Orion { get; set; } = new OrionLauncherOptions();
        public CrossUOLauncherOptions Cross { get; set; } = new CrossUOLauncherOptions();
        public ClassicClientLauncherOptions Classic { get; set; } = new ClassicClientLauncherOptions();

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
                                {
                                    defaultInitialScriptFileName = new FileInfo(scriptFileName).FullName;
                                }
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

        public string ClientExePath
        {
            get
            {
                switch (this.ClientType)
                {
                    case UltimaClientType.Classic:
                        return Classic.ClientExePath;
                    case UltimaClientType.Orion:
                        return Orion.ClientExePath;
                    case UltimaClientType.CrossUO:
                        return Cross.ClientExePath;
                    default:
                        throw new NotImplementedException();
                }
            }
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

            if (ClientType == UltimaClientType.Orion && !Orion.Validate(out validationMessage))
                return false;
            else if (ClientType == UltimaClientType.CrossUO && !Cross.Validate(out validationMessage))
                return false;

            validationMessage = string.Empty;
            return true;
        }
    }
}
