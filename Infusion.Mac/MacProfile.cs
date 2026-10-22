using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Infusion.Config;
using Infusion.IO.Encryption.Login;
using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Infusion.Mac
{
    /// <summary>
    /// Profile compatible with Windows Infusion.Desktop (*.profile JSON), so the same profiles
    /// can be used on both systems. Unknown parts of the JSON (Orion, Official, ConsoleOptions...) are kept unchanged.
    /// </summary>
    internal sealed class MacProfile
    {
        public string FileName { get; private set; }
        public JObject Raw { get; private set; }

        public string Name => (string)Raw["Name"];
        public string Id => (string)Raw["Id"];
        public JObject Launcher => (JObject)Raw["LauncherOptions"] ?? new JObject();

        public string ServerOverride { get; set; }
        public string ServerEndpoint => ServerOverride ?? (string)Launcher["ServerEndpoint"];
        public Version ProtocolVersion => ParseVersion((string)Launcher["ProtocolVersion"]) ?? new Version(3, 0, 0);
        public string UserName => (string)Launcher["UserName"];
        public string Password => (string)Launcher["Password"];
        public string InitialScriptFileName => (string)Launcher["InitialScriptFileName"];
        public string ClientType => (string)Launcher["ClientType"];

        public EncryptionSetup EncryptionSetup
        {
            get
            {
                var value = (string)Launcher["ClassicUO"]?["EncryptionSetup"];
                return Enum.TryParse<EncryptionSetup>(value, out var setup) ? setup : EncryptionSetup.Autodetect;
            }
        }

        public Version EncryptionVersion => ParseVersion((string)Launcher["ClassicUO"]?["EncryptionVersion"]);

        public LoginEncryptionKey? GetEncryptionKey()
        {
            if (EncryptionSetup == EncryptionSetup.EncryptedServer && EncryptionVersion != null)
                return LoginEncryptionKey.FromVersion(EncryptionVersion);
            return null;
        }

        public Dictionary<string, object> Options { get; private set; }

        private static Version ParseVersion(string text)
            => !string.IsNullOrEmpty(text) && Version.TryParse(text, out var v) ? v : null;

        internal static readonly JsonConverter[] Converters =
        {
            new VersionConverter(), new ObjectIdConverter(), new StringEnumConverter(), new LoginEncryptionKeyConverter()
        };

        public static MacProfile Load(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var raw = JObject.Parse(json);
            var profile = new MacProfile { FileName = fileName, Raw = raw };
            profile.Options = raw["Options"] is JObject optionsObject
                ? optionsObject.ToObject<Dictionary<string, object>>(JsonSerializer.Create(new JsonSerializerSettings { Converters = Converters }))
                : new Dictionary<string, object>();
            return profile;
        }

        public bool ReadOnly { get; set; }

        public void Save(IConsole console)
        {
            if (ReadOnly) return;
            try
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings { Converters = Converters });
                Raw["Options"] = JObject.FromObject(Options, serializer);
                var json = JsonConvert.SerializeObject(Raw, Formatting.Indented, Converters);
                var tmp = FileName + ".tmp";
                File.WriteAllText(tmp, json);
                File.Move(tmp, FileName, true);
            }
            catch (Exception ex)
            {
                console?.Error($"Cannot save profile: {ex.Message}");
            }
        }

        /// <summary>Same as Desktop: reloads Options after saving, so scripts get JObjects.</summary>
        public void FixOptions()
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings { Converters = Converters });
            var obj = JObject.FromObject(Options, serializer);
            Options = obj.ToObject<Dictionary<string, object>>(serializer);
        }

        public async Task<IPEndPoint> ResolveServerEndpoint()
        {
            var parts = ServerEndpoint.Split(',').Select(x => x.Trim()).ToArray();
            if (!IPAddress.TryParse(parts[0], out var address))
            {
                var entry = await Dns.GetHostEntryAsync(parts[0]);
                address = entry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
            }

            ushort port = parts.Length > 1 ? ushort.Parse(parts[1]) : (ushort)2593;
            return new IPEndPoint(address, port);
        }

        public static ushort GetFreeLocalPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return (ushort)port;
        }

        public static MacProfile Find(string profilesPath, string nameOrId)
        {
            var files = Directory.Exists(profilesPath)
                ? Directory.GetFiles(profilesPath, "*.profile")
                : Array.Empty<string>();

            var profiles = new List<MacProfile>();
            foreach (var file in files)
            {
                try { profiles.Add(Load(file)); }
                catch { /* broken profile is ignored, same as in Desktop */ }
            }

            if (string.IsNullOrEmpty(nameOrId))
            {
                try { nameOrId = File.ReadAllText(Path.Combine(profilesPath, "selected")).Trim(); }
                catch { }
            }

            if (string.IsNullOrEmpty(nameOrId))
                return profiles.FirstOrDefault();

            // more profiles can have the same Id (renamed copy) - take the most recently changed one
            profiles = profiles.OrderByDescending(p => File.GetLastWriteTimeUtc(p.FileName)).ToList();
            return profiles.FirstOrDefault(p => string.Equals(p.Id, nameOrId, StringComparison.OrdinalIgnoreCase))
                ?? profiles.FirstOrDefault(p => string.Equals(p.Name, nameOrId, StringComparison.OrdinalIgnoreCase))
                ?? profiles.FirstOrDefault(p => string.Equals(Path.GetFileNameWithoutExtension(p.FileName), nameOrId, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<string> ListNames(string profilesPath)
            => Directory.Exists(profilesPath)
                ? Directory.GetFiles(profilesPath, "*.profile").Select(Path.GetFileNameWithoutExtension)
                : Enumerable.Empty<string>();
    }

    internal sealed class ProfileConfigRepository : IConfigBagRepository
    {
        private readonly MacProfile profile;
        private readonly IConsole console;

        public ProfileConfigRepository(MacProfile profile, IConsole console)
        {
            this.profile = profile;
            this.console = console;
        }

        public T Get<T>(string name) => Get(name, default(T));

        public T Get<T>(string name, T defaultValue)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ObjectIdConverter());

            if (profile.Options.TryGetValue(name, out var value))
            {
                if (value is JObject jobj)
                    return jobj.ToObject<T>(serializer);
                if (value is JArray jarr)
                    return jarr.ToObject<T>(serializer);
                if (typeof(T) == typeof(ObjectId))
                {
                    object result = new ObjectId((uint)Convert.ChangeType(value, typeof(uint)));
                    return (T)result;
                }

                if (value is IConvertible)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        console.Error(ex.ToString());
                    }
                }
                else
                {
                    return (T)value;
                }
            }

            return defaultValue;
        }

        public void Update(string name, object value) => profile.Options[name] = value;
    }

    internal sealed class ObjectIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectId);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(long))
                return new ObjectId((uint)(long)reader.Value);
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => writer.WriteValue(((ObjectId)value).Value);
    }

    internal sealed class LoginEncryptionKeyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(LoginEncryptionKey);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new LoginEncryptionKey((uint)obj["Key1"], (uint)obj["Key2"], (uint)obj["Key3"]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var key = (LoginEncryptionKey)value;
            writer.WriteStartObject();
            writer.WritePropertyName("Key1");
            serializer.Serialize(writer, key.Key1);
            writer.WritePropertyName("Key2");
            serializer.Serialize(writer, key.Key2);
            writer.WritePropertyName("Key3");
            serializer.Serialize(writer, key.Key3);
            writer.WriteEndObject();
        }
    }
}
