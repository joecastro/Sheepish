namespace Hbo.Sheepish
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Standard;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;

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
    }
}
