namespace Hbo.Sheepish
{
    using Standard;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new StatusWindow();

            SingleInstance.SingleInstanceActivated += _SignalExternalCommandLineArgs;
            base.OnStartup(e);

            Settings settings = null;
            try
            {
                settings = Settings.TryLoad();
            }
            catch
            {
            }

            if (settings == null)
            {
                var loginWindow = new LoginWindow();
                if (!(loginWindow.ShowDialog() ?? false))
                {
                    Application.Current.Shutdown(0);
                    return;
                }
                settings = Settings.Create();
            }

            ServiceProvider.OnLoggedIn();
            MainWindow.Show();
        }

        private void _SignalExternalCommandLineArgs(object sender, SingleInstanceEventArgs e)
        {
        }
    }
}
