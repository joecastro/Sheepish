namespace Hbo.Sheepish
{
    using Standard;
    using System.Net;
    using System.Windows;

    public partial class App
    {
        private Settings _settings;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool showLogin = false;

            _settings = Settings.Load();
            if (_settings == null)
            {
                _settings = new Settings
                {
                    PrimaryQuery = "for: me #Open",
                    SecondaryQuery = "for: me #Resolved",
                };
                showLogin = true;
            }

            ServiceProvider.Initialize(_settings);
            MainWindow = new StatusWindow();

            if (!showLogin)
            {
                // Verify that the userlogin matches our cookie's credentials
                try
                {
                    // Mostly just care that we don't get a 400 something from this call.
                    YouTrackService.User currentUser = ServiceProvider.YouTrackService.GetCurrentUser();
                    if (_settings.UserLogin != currentUser.Login)
                    {
                        showLogin = true;
                    }
                }
                catch (WebException) { }
            }

            if (showLogin)
            {
                var loginWindow = new LoginWindow();
                if (!(loginWindow.ShowDialog() ?? false))
                {
                    Application.Current.Shutdown(0);
                    return;
                }

                _settings.UserLogin = ServiceProvider.YouTrackService.GetCurrentUser().Login;
                _settings.Save();
            }

            ServiceProvider.OnLoggedIn();
            MainWindow.Show();
            SingleInstance.SingleInstanceActivated += _SignalExternalCommandLineArgs;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void _SignalExternalCommandLineArgs(object sender, SingleInstanceEventArgs e)
        {
            ((StatusWindow)MainWindow).ProcessCommandLineArgs(e.Args);
        }
    }
}
