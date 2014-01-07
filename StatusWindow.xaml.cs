namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shell;

    public partial class StatusWindow
    {
        private readonly ElementToImageSourceConverter _elementToImageSourceConverter;

        // Keep track of this so we can tell direction of change.
        private int? _lastPrimary = null;

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

            _elementToImageSourceConverter = (ElementToImageSourceConverter)FindResource("ElementToImageSourceConverter");

            // Normally we'd have to use a named delegate here so we can unhook it and prevent a memory leak,
            // but this window is bound to the lifetime of the app so there isn't the need.
            Utility.AddDependencyPropertyChangeListener(CountControl, SheepCounterControl.CountProperty, (sender, e) =>
            {
                // Don't make the background red on initial render.
                // Often the window will initially load with the count controls at zero, so wait till it's not empty
                // to start this check
                if (_lastPrimary != null)
                {
                    if (_lastPrimary.Value < CountControl.Count)
                    {
                        HasMoreIssues = true;
                    }
                    if (_lastPrimary.Value > CountControl.Count)
                    {
                        HasLessIssues = true;
                    }
                    _lastPrimary = CountControl.Count;
                }
                else if (CountControl.Count != 0)
                {
                    _lastPrimary = CountControl.Count;
                }
                _OnIssueCountChanged();
            });
            Utility.AddDependencyPropertyChangeListener(SecondaryCountControl, SheepCounterControl.CountProperty, (sender, e) => _OnIssueCountChanged());

            // Let the window get created and shown normally first, and then explicitly minimized.
            // If this happens too early then the Shell tries to be helpful and create a fake preview window
            // when hovering over the taskbar item.
            SourceInitialized += (sender, e) =>
            {
                // But clear opacity immediately (keeping it opaque for the sake of the design surface)
                Opacity = 0;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    WindowState = WindowState.Minimized;
                    _OnIssueCountChanged();
                    StateChanged += (sender2, e2) => _ForceHidden();
                }));
            };
        }

        private void _ForceHidden()
        {
            // Keep this Window minimized at all times.
            if (WindowState != WindowState.Minimized)
            {
                this.WindowState = WindowState.Minimized;

                if (!ServiceProvider.ViewModel.ShowingEditDialog)
                {
                    HasMoreIssues = false;
                    HasLessIssues = false;
                    ServiceProvider.ShowQuery(ServiceProvider.ViewModel.PrimaryScope, ServiceProvider.ViewModel.PrimaryQuery);
                }
            }
        }

        private void _OnIssueCountChanged()
        {
            // Explicitly force the Icon to update.  We're bound to the control through a converter so it doesn't see this change.
            this.GetBindingExpression(Window.IconProperty).UpdateTarget();

            var state = HasMoreIssues
                ? TaskbarItemProgressState.Error
                : HasLessIssues
                    ? TaskbarItemProgressState.Normal
                    : TaskbarItemProgressState.None;

            ImageSource overlay = SecondaryCountControl.Count == 0 ? null : (ImageSource)_elementToImageSourceConverter.Convert(SecondaryCountControl, typeof(ImageSource), "Small", null);

            PrimaryThumbButton.Description = string.Format("View {0} issues in the primary query", CountControl.Count);
            SecondaryThumbButton.Description = string.Format("View {0} issues in the alternate query", SecondaryCountControl.Count);
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

            for (int argIndex = 1; argIndex < commandLineArgs.Count; ++argIndex)
            {
                string commandSwitch = commandLineArgs[argIndex].ToLowerInvariant();
                if (commandSwitch.StartsWith("-issue:") || commandSwitch.StartsWith("/issue:"))
                {
                    string id = commandSwitch.Substring("-issue:".Length);
                    ServiceProvider.ShowIssue(new YouTrackService.IssueSummary { Id = id, Summary = "summary" });
                }
                else switch (commandSwitch)
                {
                case "-signout":
                case "/signout":
                    ServiceProvider.SignOut();
                    return true;
                case "-exit":
                case "/exit":
                    ServiceProvider.Quit();
                    return true;
                case "-edit":
                case "/edit":
                    _OnEditClicked(this, EventArgs.Empty);
                    return true;
                }
            }

            return false;
        }

        private void _OnRefreshClicked(object sender, EventArgs e)
        {
            ServiceProvider.RequestRefresh();
        }

        private void _OnSignoutClicked(object sender, EventArgs e)
        {
            ServiceProvider.SignOut();
        }

        private void _OnEditClicked(object sender, EventArgs e)
        {
            ServiceProvider.ShowQueryDialog();
        }

        private void _OnPrimaryClicked(object sender, EventArgs e)
        {
            ServiceProvider.ShowQuery(ServiceProvider.ViewModel.PrimaryScope, ServiceProvider.ViewModel.PrimaryQuery);
        }

        private void _OnSecondaryClicked(object sender, EventArgs e)
        {
            ServiceProvider.ShowQuery(ServiceProvider.ViewModel.SecondaryScope, ServiceProvider.ViewModel.SecondaryQuery);
        }
    }
}
