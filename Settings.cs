namespace Hbo.Sheepish
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Standard;

    public class Settings
    {
        private static readonly string _SettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sheepish");
        private static readonly string _Path = Path.Combine(_SettingsDirectory, "Settings.json");

        public string UserLogin { get; private set; }
        public string PrimaryQuery { get; set; }
        public string PrimaryQueryScope { get; set; }
        public string SecondaryQuery { get; set; }
        public string SecondaryQueryScope { get; set; }

        private Settings() { }

        public async static Task<Settings> CreateAsync()
        {
            var settings = new Settings();

            bool valid = await settings._VerifyAndUpdateUserAsync();
            if (!valid)
            {
                throw new ArgumentException("oauthToken is invalid", "oauthToken");
            }

            return settings;
        }

        public async static Task<Settings> LoadAsync()
        {
            Settings settings = null;
            Utility.EnsureDirectory(_SettingsDirectory);

            dynamic json = null;
            using (var settingsStream = new StreamReader(_Path))
            {
                json = JsonConvert.DeserializeObject<dynamic>(settingsStream.ReadToEnd());
            }

            if ((int)json["version"] == 1)
            {
                var maybeSettings = new Settings
                {
                    PrimaryQuery = json["primary_query"],
                    PrimaryQueryScope = json["primary_scope"],
                    SecondaryQuery = json["secondary_query"],
                    SecondaryQueryScope = json["secondary_scope"],
                };

                if (await maybeSettings._VerifyAndUpdateUserAsync())
                {
                    settings = maybeSettings;
                }
            }
            return settings;
        }

        /// <summary>
        /// Clears the Settings file from disk.
        /// </summary>
        public static void Clear()
        {
            Utility.SafeDeleteFile(_Path);
        }

        private async Task<bool> _VerifyAndUpdateUserAsync()
        {
            try
            {
                // Mostly just care that we don't get a 400 something from this call.
                YouTrackService.User currentUser = await ServiceProvider.YouTrackService.GetCurrentUserAsync();
                UserLogin = currentUser.Login;
            }
            catch (WebException) { }
            return UserLogin != null;
        }

        public void Save()
        {
            var dict = new Dictionary<string, object>
            {
                { "description", "Cached settings for Sheepish" },
                { "version", 1 },
                // Included for readability purposes, but it's implied by the OauthToken.
                { "login", UserLogin },
                { "primary_query", PrimaryQuery },
                { "primary_scope", PrimaryQueryScope },
                { "secondary_query", SecondaryQuery },
                { "secondary_scope", SecondaryQueryScope },
            };

            Utility.EnsureDirectory(Path.GetDirectoryName(_Path));

            using (var sw = new StreamWriter(_Path))
            {
                sw.Write(JsonConvert.SerializeObject(dict));
            }
        }
    }
}