namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Diagnostics;

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
                if (loginWindow.DialogResult == true)
                {
                    settings = Settings.Create();
                }
                else
                {
                    // Terminate the app here if the login dialog was cancelled.
                    Trace.TraceInformation("User terminated app at login screen");
                    Application.Current.Shutdown(0);
                }
            }

            MainWindow.Show();

        }

        private void _SignalExternalCommandLineArgs(object sender, SingleInstanceEventArgs e)
        {
        }
    }
}
