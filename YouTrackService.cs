namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    // General documentation for the YouTrack REST APIs: http://confluence.jetbrains.com/display/YTD5/YouTrack+REST+API+Reference
    public class YouTrackService
    {
        public class User
        {
            public string Login { get; set; }
            public string Email { get; set; }
        }

        public class Project 
        {
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Description { get; set; }
        }

        private static readonly string _UserAgentString = "HBO Sheepish Client";
        private readonly string _BaseApiUrl;

        #region Async Http Verb Implementations

        private static XDocument _Get(string path)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "GET";
            webRequest.Accept = "application/xml";
            webRequest.UserAgent = _UserAgentString;

            WebResponse response = webRequest.GetResponse();
            try
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return XDocument.Parse(reader.ReadToEnd());
                }
            }
            finally
            {
                response.Close();
            }
        }

        private static async Task<XDocument> _GetAsync(string path)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "GET";
            webRequest.Accept = "application/xml";
            webRequest.UserAgent = _UserAgentString;

            WebResponse response = await webRequest.GetResponseAsync();
            try
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return XDocument.Parse(await reader.ReadToEndAsync());
                }
            }
            finally
            {
                response.Close();
            }
        }

        private static async Task<XDocument> _PostAsync(string path, string postData)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "POST";
            webRequest.UserAgent = _UserAgentString;
            webRequest.Accept = "application/xml";
            webRequest.ContentType = "text/plain; charset=utf-8";
            if (!string.IsNullOrEmpty(postData))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(postData);

                webRequest.ContentLength = postData.Length;
                using (var dataStream = await webRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }
            else
            {
                webRequest.ContentLength = 0;
            }

            var response = await webRequest.GetResponseAsync();
            try
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return XDocument.Parse(await reader.ReadToEndAsync());
                }
            }
            finally
            {
                Utility.SafeDispose(ref response);
            }
        }

        #endregion

        public YouTrackService(string baseUri)
        {
            _BaseApiUrl = baseUri;
        }

        public async Task Login(string username, string password) 
        {
            var uri = String.Format("{0}/user/login?login={1}&password={2}", _BaseApiUrl, username, password);
            await _PostAsync(uri, null);
        }

        public User GetCurrentUser()
        {
            var uri = String.Format("{0}/user/current", _BaseApiUrl);
            var response = _Get(uri);
            return new User
            {
                Login = response.ToString()
            };
        }

        public async Task<List<Project>> GetProjects()
        {
            var response = await _GetAsync(String.Format("{0}/project/all", _BaseApiUrl));
            return new List<Project> 
            {
                new Project { Name = response.ToString() }
            };
        }

        public int GetIssueCount(string projectShortName, string query) {
            return query.GetHashCode() & 0xFF;
        }
    }
}
