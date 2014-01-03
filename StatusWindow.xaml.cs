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
        }

        protected override void OnStateChanged(System.EventArgs e)
        {
            base.OnStateChanged(e);

            // Keep this Window minimized at all times.
            // Trying to show it should just launch the Issues URL.
            if (WindowState != WindowState.Minimized)
            {
                this.CountControl.Count = 15;
                this.ResolvedCountControl.Count = 7;
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
