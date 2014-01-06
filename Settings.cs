namespace Hbo.Sheepish
{
    using Newtonsoft.Json;
    using Standard;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;

    public class Settings
    {
        private static readonly string _SettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sheepish");
        private static readonly string _Path = Path.Combine(_SettingsDirectory, "Settings.json");

        public string UserLogin { get; set; }
        public string PrimaryQuery { get; set; }
        public string PrimaryQueryScope { get; set; }
        public string SecondaryQuery { get; set; }
        public string SecondaryQueryScope { get; set; }
        public CookieContainer CookieContainer { get; private set; }

        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        public Settings()
        {
            CookieContainer = new CookieContainer();
        }

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

                CookieContainer jar = null;

                try
                {
                    var serializedJar = System.Convert.FromBase64String(json["cookie_jar"].ToString());
                    jar = (CookieContainer)_formatter.Deserialize(new MemoryStream(serializedJar));
                }
                catch
                {
                }

                var maybeSettings = new Settings
                {
                    UserLogin = json["login"],
                    PrimaryQuery = json["primary_query"],
                    PrimaryQueryScope = json["primary_scope"],
                    SecondaryQuery = json["secondary_query"],
                    SecondaryQueryScope = json["secondary_scope"],
                    CookieContainer = jar ?? new CookieContainer(),
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
            CookieContainer = new CookieContainer();
        }

        public void Save()
        {
            string serializedJar = null;
            using (var memstream = new MemoryStream())
            {
                _formatter.Serialize(memstream, CookieContainer);
                serializedJar = Convert.ToBase64String(memstream.ToArray());
            }

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
                { "cookie_jar", serializedJar },
            };

            Utility.EnsureDirectory(Path.GetDirectoryName(_Path));

            using (var sw = new StreamWriter(_Path))
            {
                sw.Write(JsonConvert.SerializeObject(dict));
            }
        }
    }
}