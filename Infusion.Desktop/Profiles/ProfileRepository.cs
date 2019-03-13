using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Infusion.Desktop.Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infusion.Desktop.Profiles
{
    internal static class ProfileRepository
    {
        public static string ProfilesPath { get; } =
            PathUtilities.GetAbsolutePath("Profiles");

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
            var profile = JsonConvert.DeserializeObject<LaunchProfile>(profileJson, new VersionConverter());

            ProvideDefaults(profile);

            return profile;
        }

        private static void ProvideDefaults(LaunchProfile profile)
        {
            // if a property doesn't exist in version 1.0 and the property is added in version 2.0
            // the json put null, to this property
            profile.LauncherOptions = profile.LauncherOptions ?? new LauncherOptions();
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

        public static void SaveProfile(LaunchProfile profile)
        {
            try
            {
                EnsureProfileDirectoryExists();

                string profileJson = JsonConvert.SerializeObject(profile, Formatting.Indented, new VersionConverter());
                string profileFileName = Path.Combine(ProfilesPath, PathUtilities.GetSafeFilename(profile.Name) + ".profile");

                File.WriteAllText(profileFileName, profileJson);

            }
            catch (Exception)
            {
                // just ignore
            }
        }
    }
}