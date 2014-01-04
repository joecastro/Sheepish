namespace Hbo.Sheepish
{
    using Newtonsoft.Json;
    using Standard;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    public class Settings
    {
        private static readonly string _SettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sheepish");
        private static readonly string _Path = Path.Combine(_SettingsDirectory, "Settings.json");

        public string UserLogin { get; set; }
        public string PrimaryQuery { get; set; }
        public string PrimaryQueryScope { get; set; }
        public string SecondaryQuery { get; set; }
        public string SecondaryQueryScope { get; set; }

        public Settings() {}

        public static Settings Load()
        {
            Utility.EnsureDirectory(_SettingsDirectory);
            try
            {
                dynamic json = null;
                using (var settingsStream = new StreamReader(_Path))
                {
                    json = JsonConvert.DeserializeObject<dynamic>(settingsStream.ReadToEnd());
                }

                if ((int)json["version"] != 1)
                {
                    // Don't know how to deserialize this.
                    return null;
                }

                var maybeSettings = new Settings
                {
                    UserLogin = json["login"],
                    PrimaryQuery = json["primary_query"],
                    PrimaryQueryScope = json["primary_scope"],
                    SecondaryQuery = json["secondary_query"],
                    SecondaryQueryScope = json["secondary_scope"],
                };

                return maybeSettings;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Clears the Settings file from disk.
        /// </summary>
        public void ClearLogin()
        {
            UserLogin = "";
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