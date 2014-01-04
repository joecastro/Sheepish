namespace Hbo.Sheepish
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Standard;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class StatusWindow
    {
        public StatusWindow()
        {
            InitializeComponent();

            SourceInitialized += (sender, e) => OnStateChanged(EventArgs.Empty);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            // Keep this Window minimized at all times.
            // Trying to show it should just launch the Issues URL.
            if (WindowState != WindowState.Minimized)
            {
                this.Icon = this.CountControl.ImageSource;
                this.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo
                {
                    ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error,
                    ProgressValue = 1,
                    Overlay = ResolvedCountControl.ImageSource,
                };
                //Process.Start(GithubService.IssuesAssignedToMeUri.ToString());
                this.WindowState = WindowState.Minimized;
            }
        }

        // Let the application handle the window's lifetime.
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }

        internal bool ProcessCommandLineArgs(IList<string> commandLineArgs)
        {
            if (commandLineArgs == null || commandLineArgs.Count < 2)
            {
                commandLineArgs = new[] { null, "-primary" };
            }

            int argIndex = 1;
            while (argIndex < commandLineArgs.Count)
            {
                string commandSwitch = commandLineArgs[argIndex].ToLowerInvariant();
                switch (commandSwitch)
                {
                    case "-signout":
                    case "/signout":
                        ServiceProvider.SignOut();
                        return true;
                    case "-exit":
                    case "/exit":
                        ServiceProvider.Quit();
                        return true;
                }
                ++argIndex;
            }

            return false;
        }
    }
}
