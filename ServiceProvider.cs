namespace Hbo.Sheepish
{
    using System;
using System.Windows;
using System.Windows.Threading;
    
    internal static class ServiceProvider
    {
        private static readonly TimeSpan _PollInterval = new TimeSpan(0, 1, 0);

        public static YouTrackService YouTrackService { get; private set; }
        public static ViewModel ViewModel { get; private set; }

        private static DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        {
            Interval = _PollInterval,
        };

        static ServiceProvider()
        {
            YouTrackService = new YouTrackService("https://youtrack.hbo.com/youtrack/rest");
            ViewModel = new ViewModel();

            ViewModel.PrimaryQuery = "for: me #Open";
            ViewModel.SecondaryQuery = "for: me #Resolved";

            _timer.Tick += _timer_Tick;
        }

        static void _timer_Tick(object sender, EventArgs e)
        {
            
        }

        internal static void OnLoggedIn()
        {
            ViewModel.PrimaryCount = YouTrackService.GetIssueCount(ViewModel.PrimaryScope, ViewModel.PrimaryQuery);
            ViewModel.SecondaryCount = YouTrackService.GetIssueCount(ViewModel.SecondaryScope, ViewModel.SecondaryQuery);
            _timer.Start();
        }
    }
}
