namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new StatusWindow();

            SingleInstance.SingleInstanceActivated += _SignalExternalCommandLineArgs;
            base.OnStartup(e);

            TaskScheduler.UnobservedTaskException += (sender, e2) => { MessageBox.Show(e2.Exception.ToString()); };

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
                loginWindow.ShowDialog();
                settings = Settings.Create();
            }

            MainWindow.Show();

        }

        private void _SignalExternalCommandLineArgs(object sender, SingleInstanceEventArgs e)
        {
        }
    }
}
