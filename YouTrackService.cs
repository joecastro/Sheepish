namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
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

        public class IssueSummary
        {
            public string Id { get; set; }
            public string Summary { get; set; }

            public override string ToString()
            {
                return string.Format("({0}) {1}", Id, Summary);
            }
        }

        public class SavedSearch  : IEquatable<SavedSearch>
        {
            public string Name { get; set; }
            public string Query { get; set; }
            public Project ProjectScope { get; set; }

            public string GetAugmentedQuery(string query)
            {
                var queryBuilder = new StringBuilder();
                if (ProjectScope != null)
                {
                    queryBuilder.AppendFormat("project: {0} ", ProjectScope.ShortName);
                }

                queryBuilder.AppendFormat("{0} {1}", this.Query ?? "", query ?? "");

                return queryBuilder.ToString().Trim();
            }

            public static string GetAugmentedQuery(SavedSearch scope, string query)
            {
                if (scope == null)
                {
                    return query ?? "";
                }

                return scope.GetAugmentedQuery(query);
            }

            #region IEquatable<SavedSearch> implementation

            public bool Equals(SavedSearch other)
            {
                if (other == null)
                {
                    return false;
                }

                if (other.Name != this.Name || other.Query != this.Query)
                {
                    return false;
                }

                if (ProjectScope == null)
                {
                    return other.ProjectScope == null;
                }

                return ProjectScope.Equals(other.ProjectScope);
            }

            #endregion

            public override bool Equals(object obj)
            {
                return this.Equals(obj as SavedSearch);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ (Query.GetHashCode() << 4) ^ ((ProjectScope == null ? 0 : ProjectScope.GetHashCode()) >> 13);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public static class SavedSearches
        {
            public static readonly SavedSearch Everything = new SavedSearch { Name = "Everything" };
        }

        private static readonly string _UserAgentString = "HBO Sheepish Client";
        private readonly string _BaseUrl;
        private readonly CookieContainer _CookieJar = new CookieContainer();
        private const int _MaxRetryCount = 8;
        private static readonly TimeSpan _RetryDelay = new TimeSpan(1000);

        #region Http Verb Implementations

        private static XDocument _Get(string path, CookieContainer jar)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "GET";
            webRequest.Accept = "application/xml";
            webRequest.UserAgent = _UserAgentString;
            webRequest.CookieContainer = jar;

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

        private static XDocument _Post(string path, string postData, CookieContainer jar)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(path);
            webRequest.Method = "POST";
            webRequest.UserAgent = _UserAgentString;
            webRequest.Accept = "application/xml";
            webRequest.ContentType = "text/plain; charset=utf-8";
            webRequest.CookieContainer = jar;

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

        public YouTrackService(string baseUri, CookieContainer jar)
        {
            _BaseUrl = baseUri;
            _CookieJar = jar;
        }

        public void Login(string username, string password) 
        {
            var uri = string.Format("{0}/rest/user/login?login={1}&password={2}", _BaseUrl, username, password);
            _Post(uri, null, _CookieJar);
        }

        public User GetCurrentUser()
        {
            var uri = string.Format("{0}/rest/user/current", _BaseUrl);
            var response = _Get(uri, _CookieJar);
            return new User
            {
                Login = response.ToString()
            };
        }

        public List<Project> GetProjects()
        {
            var response = _Get(string.Format("{0}/rest/project/all", _BaseUrl), _CookieJar);
            return new List<Project> 
            {
                new Project { Name = response.ToString() }
            };
        }

        public List<SavedSearch> GetSavedSearches()
        {
            var retList = new List<SavedSearch> { SavedSearches.Everything };

            retList.AddRange(from proj in GetProjects() select new SavedSearch
                {
                    Name = proj.Name,
                    ProjectScope = proj
                });

            var savedSearchResponse = _Get(string.Format("{0}/rest/user/search", _BaseUrl), _CookieJar);
            retList.Add(new SavedSearch { Name = savedSearchResponse.ToString() });

            return retList;
        }

        public int GetIssueCount(SavedSearch scope, string query)
        {
            for (int i = 0; i < _MaxRetryCount; ++i)
            {
                var path = string.Format("{0}/rest/issue/count?filter={1}", _BaseUrl, Utility.UrlEncode(SavedSearch.GetAugmentedQuery(scope, query)));
                var response = _Get(path, _CookieJar);
                int count = int.Parse(response.Root.Value);
                if (count != -1)
                {
                    return count;
                }
                Thread.Sleep(_RetryDelay);
            }
            throw new Exception("Unable to determine number of issues from server.");
        }

        public List<IssueSummary> GetRecentlyUpdatedIssues(SavedSearch scope, string queryFilter) {
            const int MaxCount = 5;

            var path = string.Format("{0}/rest/issue?filter={1}+sort+by%3A+updated&with=summary&max={2}", _BaseUrl, Utility.UrlEncode(SavedSearch.GetAugmentedQuery(scope, queryFilter)), MaxCount);
            var response = _Get(path, _CookieJar);

            var retList = new List<IssueSummary>(
                from issueNode in response.Element("issueCompacts").Elements() select new IssueSummary 
                {
                    Id = issueNode.Attribute("id").Value,
                    Summary = issueNode.Element("field").Element("value").Value
                });

            return retList;
        }

        public Uri GetQueryUri(SavedSearch scope, string queryFilter)
        {
            if (scope == null)
            {
                return new Uri(string.Format("{0}/issues?q={1}", _BaseUrl, Utility.UrlEncode(queryFilter)));
            }

            string prefix = _BaseUrl + "/issues";
            if (scope.ProjectScope != null)
            {
                prefix += "/" + scope.ProjectScope.ShortName;
            }

            string queryPart = (scope.Query ?? "") + " " + queryFilter;

            return new Uri(string.Format("{0}?q={1}", prefix, queryPart));
        }

        public Uri GetIssueUri(IssueSummary summary)
        {
            return new Uri(string.Format("{0}/issue/{1}", _BaseUrl, summary.Id));
        }
    }
}
