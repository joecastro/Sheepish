namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Diagnostics;
    using System.IO;

    public partial class App
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new StatusWindow();

            SingleInstance.SingleInstanceActivated += _SignalExternalCommandLineArgs;
            base.OnStartup(e);

            Settings settings = null;
            try
            {
                settings = await Settings.LoadAsync();
            }
            catch (FileNotFoundException)
            {
                // Eat FnF exceptions!
            }

            if (settings == null)
            { 
                var loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DialogResult == true)
                {
                    settings = await Settings.CreateAsync();
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
