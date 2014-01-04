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
        private static readonly CookieContainer _cookieJar = new CookieContainer();

        #region Http Verb Implementations

        private static XDocument _Get(string path)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "GET";
            webRequest.Accept = "application/xml";
            webRequest.UserAgent = _UserAgentString;
            webRequest.CookieContainer = _cookieJar;

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

        private static XDocument _Post(string path, string postData)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "POST";
            webRequest.UserAgent = _UserAgentString;
            webRequest.Accept = "application/xml";
            webRequest.ContentType = "text/plain; charset=utf-8";
            webRequest.CookieContainer = _cookieJar;

            if (!string.IsNullOrEmpty(postData))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(postData);

                webRequest.ContentLength = postData.Length;
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }
            else
            {
                webRequest.ContentLength = 0;
            }

            var response = webRequest.GetResponse();
            try
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return XDocument.Parse(reader.ReadToEnd());
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

        public void Login(string username, string password) 
        {
            var uri = String.Format("{0}/user/login?login={1}&password={2}", _BaseApiUrl, username, password);
            _Post(uri, null);
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

        public List<Project> GetProjects()
        {
            var response = _Get(String.Format("{0}/project/all", _BaseApiUrl));
            return new List<Project> 
            {
                new Project { Name = response.ToString() }
            };
        }

        public int GetIssueCount(string projectShortName, string query)
        {
            return query.GetHashCode() & 0xFF;
        }
    }
}
