namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Shell;
    using System.Windows.Threading;
    
    internal static class ServiceProvider
    {
        private static readonly TimeSpan _PollInterval = new TimeSpan(0, 1, 0);

        public static YouTrackService YouTrackService { get; private set; }
        public static ViewModel ViewModel { get; private set; }
        private static Settings _settings;

        private static DispatcherTimer _timer;

        public static void Initialize(Settings settings)
        {
            Verify.IsNotNull(settings, "settings");

            _settings = settings;
            YouTrackService = new YouTrackService("https://youtrack.hbo.com/youtrack", settings.CookieContainer);
            ViewModel = new ViewModel();

            ViewModel.PrimaryQuery = "for: me #Open";
            ViewModel.SecondaryQuery = "for: me #Resolved";

            _timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
            {
                Interval = _PollInterval,
            };
            _timer.Tick += (sender, e) => _UpdateQueryCounts();
        }

        private static void _UpdateQueryCounts()
        {
            List<YouTrackService.IssueSummary> recentPrimaryIssues = null;
            List<YouTrackService.IssueSummary> recentSecondaryIssues = null;
            try
            {
                int primaryCount = YouTrackService.GetIssueCount(ViewModel.PrimaryScope, ViewModel.PrimaryQuery);
                ViewModel.PrimaryCount = primaryCount;
                recentPrimaryIssues = YouTrackService.GetRecentlyUpdatedIssues(ViewModel.PrimaryScope, ViewModel.PrimaryQuery);
            }
            catch
            { }

            try
            {
                int secondaryCount = YouTrackService.GetIssueCount(ViewModel.SecondaryScope, ViewModel.SecondaryQuery);
                ViewModel.SecondaryCount = secondaryCount;
                recentSecondaryIssues = YouTrackService.GetRecentlyUpdatedIssues(ViewModel.SecondaryScope, ViewModel.SecondaryQuery);
            }
            catch
            { }

            try
            {
                _UpdateJumpList(recentPrimaryIssues, recentSecondaryIssues, true);
            }
            catch
            { }
        }

        private static void _UpdateJumpList(List<YouTrackService.IssueSummary> recentPrimaryIssues, List<YouTrackService.IssueSummary> recentSecondaryIssues, bool loggedIn)
        {
            var jumplistSource = (JumpList)(loggedIn ? Application.Current.FindResource("SignedInJumpList") : Application.Current.FindResource("SignedOutJumpList"));
            // Clone, rather than modifying the original;
            var jumplist = new JumpList();
            jumplist.JumpItems.AddRange(jumplistSource.JumpItems);

            if (recentSecondaryIssues != null && recentSecondaryIssues.Count > 0)
            {
                jumplist.JumpItems.AddRange(from issueSummary in recentSecondaryIssues
                    select new JumpTask
                    {
                        CustomCategory = "Recently Updated (Secondary)",
                        Title = issueSummary.Summary,
                        Description = issueSummary.Id,
                        Arguments = "-uri:" + YouTrackService.GetIssueUri(issueSummary)
                    });
            }

            if (recentPrimaryIssues != null && recentPrimaryIssues.Count > 0)
            {
                jumplist.JumpItems.AddRange(from issueSummary in recentPrimaryIssues
                    select new JumpTask
                    {
                        CustomCategory = "Recently Updated (Primary)",
                        Title = issueSummary.Summary,
                        Description = issueSummary.Id,
                        Arguments = "-uri:" + YouTrackService.GetIssueUri(issueSummary)
                    });
            }

            JumpList.SetJumpList(Application.Current, jumplist);
        }

        internal static void OnLoggedIn()
        {
            _UpdateJumpList(null, null, true);
            _UpdateQueryCounts();
            _timer.Start();
        }

        internal static void Quit()
        {
            _UpdateJumpList(null, null, false);
            _settings.Save();
            Application.Current.Shutdown();
        }

        internal static void SignOut()
        {
            _settings.ClearLogin();
            Quit();
        }

        internal static void RequestRefresh()
        {
            _UpdateQueryCounts();
        }
    }
}
