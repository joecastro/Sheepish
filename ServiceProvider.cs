namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Windows;
    using System.Windows.Shell;
    using System.Windows.Threading;
    
    internal static class ServiceProvider
    {
        private static readonly TimeSpan _PollInterval = new TimeSpan(0, 1, 0);

        public static YouTrackService YouTrackService { get; private set; }
        public static ViewModel ViewModel { get; private set; }
        private static Settings _settings;

        private static readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        {
            Interval = _PollInterval,
        };

        public static void Initialize(Settings settings)
        {
            Verify.IsNotNull(settings, "settings");

            _settings = settings;
            YouTrackService = new YouTrackService("https://youtrack.hbo.com/youtrack/rest", settings.CookieContainer);
            ViewModel = new ViewModel();

            ViewModel.PrimaryQuery = "for: me #Open";
            ViewModel.SecondaryQuery = "for: me #Resolved";

            _timer.Tick += _timer_Tick;
        }

        private static void _timer_Tick(object sender, EventArgs e)
        {
            
        }

        internal static void OnLoggedIn()
        {
            JumpList.SetJumpList(Application.Current, (JumpList)Application.Current.FindResource("SignedInJumpList"));
            ViewModel.PrimaryCount = YouTrackService.GetIssueCount(ViewModel.PrimaryScope, ViewModel.PrimaryQuery);
            ViewModel.SecondaryCount = YouTrackService.GetIssueCount(ViewModel.SecondaryScope, ViewModel.SecondaryQuery);
            _timer.Start();
        }

        internal static void Quit()
        {
            JumpList.SetJumpList(Application.Current, (JumpList)Application.Current.FindResource("SignedOutJumpList"));
            _settings.Save();
            Application.Current.Shutdown();
        }

        internal static void SignOut()
        {
            _settings.ClearLogin();
            Quit();
        }
    }
}
