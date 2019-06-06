using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Infusion.Proxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Infusion.Desktop.Profiles
{
    public static class ProfileRepository
    {
        private static JsonConverter[] jsonConverters =
        {
            new VersionConverter(), new ObjectIdConverter(), new StringEnumConverter(),
            new LoginEncryptionKeyConverter()
        };

        public static string ProfilesPath { get; } =
            PathUtilities.GetAbsolutePath("profiles");

        public static LaunchProfile SelectedProfile { get; set; }

        public static string LoadSelectedProfileId()
        {
            string id = null;
            try
            {
                id = File.ReadAllText(Path.Combine(ProfilesPath, "selected"));
            }
            catch (Exception)
            {
                // just ignore, when it is not possible to load selected profile id
            }

            return id;
        }

        public static void SaveSelectedProfileId(string id)
        {
            EnsureProfileDirectoryExists();

            try
            {
                File.WriteAllText(Path.Combine(ProfilesPath, "selected"), id);
            }
            catch (Exception)
            {
                // just ignore when it is not possible to save selected profile id
            }
        }

        public static LaunchProfile[] LoadProfiles()
        {
            EnsureProfileDirectoryExists();

            var profiles = new List<LaunchProfile>();

            foreach (var profileFileName in Directory.GetFiles(ProfilesPath, "*.profile"))
            {
                LaunchProfile profile;
                try
                {
                    profile = LoadProfile(profileFileName);
                }
                catch (Exception)
                {
                    // ignore malformed profile, just continue
                    continue;
                }

                profiles.Add(profile);
            }

            if (!profiles.Any())
                profiles.Add(new LaunchProfile() { Name = "new profile" });

            return profiles.ToArray();
        }

        public static void DeleteProfile(LaunchProfile profile)
        {
            try
            {
                File.Delete(Path.Combine(ProfilesPath, profile.Id + ".profile"));
            }
            catch (Exception)
            {
                // just ignore
            }
        }

        public static LaunchProfile LoadProfile(string profileFileName)
        {
            string profileJson = File.ReadAllText(profileFileName);
            var profile = DeserializeProfile(profileJson);

            return profile;
        }

        public static LaunchProfile DeserializeProfile(string json)
        {
            var jprofile = JObject.Parse(json);
            var profile = jprofile.ToObject<LaunchProfile>();
            if (jprofile.TryGetValue("LauncherOptions", out var jLauncherOptions))
            {
                if (!profile.Options.ContainsKey("launcher"))
                    profile.Options["launcher"] = jLauncherOptions;
            }

            if (jprofile.TryGetValue("ConsoleOptions", out var jConsoleOptions))
            {
                if (!profile.Options.ContainsKey("console.wpf"))
                    profile.Options["console.wpf"] = jConsoleOptions;
            }

            return profile;
        }

        private static void EnsureProfileDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(ProfilesPath))
                    Directory.CreateDirectory(ProfilesPath);

            }
            catch (Exception)
            {
                // just ignore
            }
        }

        public static void SaveProfiles(IEnumerable<LaunchProfile> profiles)
        {
            foreach (var profile in profiles)
            {
                SaveProfile(profile);
            }
        }

        internal static void FixOptions(LaunchProfile profile)
        {
            var json = SerializeProfile(profile);
            var refreshedProfile = DeserializeProfile(json);

            profile.Options = refreshedProfile.Options;
        }

        public static void SaveProfile(LaunchProfile profile)
        {
            try
            {
                EnsureProfileDirectoryExists();

                string profileJson = SerializeProfile(profile);
                string profileFileName = Path.Combine(ProfilesPath, PathUtilities.GetSafeFilename(profile.Name) + ".profile");

                File.WriteAllText(profileFileName, profileJson);

            }
            catch (Exception)
            {
                // just ignore
            }
        }

        public static string SerializeProfile(LaunchProfile profile)
            => JsonConvert.SerializeObject(profile, Formatting.Indented, jsonConverters);
    }
}