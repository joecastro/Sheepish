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
    using System.Windows.Shell;

    public partial class StatusWindow
    {
        public static DependencyProperty HasMoreIssuesProperty = DependencyProperty.Register(
            "HasMoreIssues",
            typeof(bool),
            typeof(StatusWindow),
            new UIPropertyMetadata(false,
                (d, e) => ((StatusWindow)d)._OnIssueCountChanged()));

        public bool HasMoreIssues
        {
            get { return (bool)GetValue(HasMoreIssuesProperty); }
            set { SetValue(HasMoreIssuesProperty, value); }
        }

        public static DependencyProperty HasLessIssuesProperty = DependencyProperty.Register(
            "HasLessIssues",
            typeof(bool),
            typeof(StatusWindow),
            new UIPropertyMetadata(false,
                (d, e) => ((StatusWindow)d)._OnIssueCountChanged()));

        public bool HasLessIssues
        {
            get { return (bool)GetValue(HasLessIssuesProperty); }
            set { SetValue(HasLessIssuesProperty, value); }
        }

        public StatusWindow()
        {
            InitializeComponent();

            Utility.AddDependencyPropertyChangeListener(CountControl, CountControl.CountProperty, (sender, e) => { });
            Utility.AddDependencyPropertyChangeListener(SecondaryCountControl, CountControl.CountProperty, (sender, e) => { });

            // Let the window get created and shown normally first, and then explicitly minimized.
            // If this happens too early then the Shell tries to be helpful and create a fake preview window
            // when hovering over the taskbar item.
            SourceInitialized += (sender, e) => Dispatcher.BeginInvoke((Action)_ForceHidden);

            StateChanged += (sender, e) => _ForceHidden();
            Activated += (sender, e) => _OnActivate();
        }

        private void _OnActivate()
        {
            HasMoreIssues = false;
            HasLessIssues = false;
        }

        private void _ForceHidden()
        {
            // Keep this Window minimized at all times.
            if (WindowState != WindowState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void _OnIssueCountChanged()
        {
            var state = HasMoreIssues
                ? TaskbarItemProgressState.Error
                : HasLessIssues
                    ? TaskbarItemProgressState.Normal
                    : TaskbarItemProgressState.None;

            ImageSource overlay = SecondaryCountControl.Count == 0 ? null : SecondaryCountControl.ImageSource;

            this.TaskbarItemInfo.ProgressState = state;
            this.TaskbarItemInfo.Overlay = overlay;
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

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            ServiceProvider.RequestRefresh();
        }
    }
}
