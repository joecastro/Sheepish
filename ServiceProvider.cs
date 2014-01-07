namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Shell;
    using System.Windows.Threading;
    using System.Diagnostics;
    using System.Windows.Interop;
    
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
            catch { }

            try
            {
                int secondaryCount = YouTrackService.GetIssueCount(ViewModel.SecondaryScope, ViewModel.SecondaryQuery);
                ViewModel.SecondaryCount = secondaryCount;
                recentSecondaryIssues = YouTrackService.GetRecentlyUpdatedIssues(ViewModel.SecondaryScope, ViewModel.SecondaryQuery);
            }
            catch { }

            try
            {
                // Keep the ViewModel's Primary and Secondary Scopes referencing actual objects in the Scopes list.
                ViewModel.Scopes = YouTrackService.GetSavedSearches();
                ViewModel.PrimaryScope = ViewModel.Scopes.FirstOrDefault(scope => scope.Name == ViewModel.PrimaryScope.Name) ?? YouTrackService.SavedSearches.Everything;
                ViewModel.SecondaryScope = ViewModel.Scopes.FirstOrDefault(scope => scope.Name == ViewModel.SecondaryScope.Name) ?? YouTrackService.SavedSearches.Everything;
            }
            catch { }

            try
            {
                _UpdateJumpList(recentPrimaryIssues, recentSecondaryIssues, true);
            }
            catch { }
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
                        Arguments = "-issue:" + issueSummary.Id,
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
                        Arguments = "-issue:" + issueSummary.Id,
                    });
            }

            JumpList.SetJumpList(Application.Current, jumplist);
        }

        internal static void OnLoggedIn()
        {
            ViewModel.PrimaryQuery = _settings.PrimaryQuery;
            ViewModel.SecondaryQuery = _settings.SecondaryQuery;

            ViewModel.Scopes = YouTrackService.GetSavedSearches();

            ViewModel.PrimaryScope = ViewModel.Scopes.FirstOrDefault(scope => scope.Name == _settings.PrimaryQueryScope) ?? YouTrackService.SavedSearches.Everything;
            ViewModel.SecondaryScope = ViewModel.Scopes.FirstOrDefault(scope => scope.Name == _settings.SecondaryQueryScope) ?? YouTrackService.SavedSearches.Everything;

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

        internal static void ShowQueryDialog()
        {
            ViewModel.ShowingEditDialog = true;

            var dialog = new QueryDialog();
            dialog.Owner = Application.Current.MainWindow;
            dialog.Closed += (sender, e) =>
            {
                RequestRefresh();
                ViewModel.ShowingEditDialog = false;
                _UpdateSettings();
            };
            // Focus tricks we're playing with the status window wreak havoc when trying to actually display a window,
            // so force the window to really be shown.
            dialog.SourceInitialized += (sender, e) => NativeMethods.SetForegroundWindow(new WindowInteropHelper(dialog).Handle);
            Application.Current.Dispatcher.Invoke((Action)(() => dialog.Show()));
        }

        private static void _UpdateSettings()
        {
            _settings.PrimaryQuery = ViewModel.PrimaryQuery;
            _settings.PrimaryQueryScope = ViewModel.PrimaryScope.Name;
            _settings.SecondaryQuery = ViewModel.SecondaryQuery;
            _settings.SecondaryQueryScope = ViewModel.SecondaryScope.Name;
        }

        internal static void ShowQuery(YouTrackService.SavedSearch scope, string query)
        {
            Process.Start(YouTrackService.GetQueryUri(scope, query).ToString());
        }

        internal static void ShowIssue(YouTrackService.IssueSummary issue)
        {
            Process.Start(YouTrackService.GetIssueUri(issue).ToString());
        }
    }
}
