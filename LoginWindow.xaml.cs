namespace Hbo.Sheepish
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public partial class LoginWindow
    {
        public static DependencyProperty ProcessingProperty = DependencyProperty.Register(
            "Processing",
            typeof(bool),
            typeof(LoginWindow));

        public bool Processing
        {
            get { return (bool)GetValue(ProcessingProperty); }
            set { SetValue(ProcessingProperty, value); }
        }

        public static DependencyProperty CanSubmitProperty = DependencyProperty.Register(
            "CanSubmit",
            typeof(bool),
            typeof(LoginWindow),
            new FrameworkPropertyMetadata(false));

        public bool CanSubmit
        {
            get { return (bool)GetValue(CanSubmitProperty); }
            set { SetValue(CanSubmitProperty, value); }
        }

        public LoginWindow()
        {
            InitializeComponent();

            // Binding to password change events is awkward.  Just directly wiring this.
            UsernameInput.TextChanged += (sender, e) => _CheckCanSubmit();
            PasswordInput.PasswordChanged += (sender, e) => _CheckCanSubmit();

            UsernameInput.Focus();
        }

        private void _CheckCanSubmit()
        {
            bool canSubmit = true;
            if (string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                canSubmit = false;
            }
            if (string.IsNullOrEmpty(PasswordInput.Password))
            {
                canSubmit = false;
            }
            if (Processing)
            {
                canSubmit = false;
            }

            CanSubmit = canSubmit;
        }

        private void _OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                _OnSignInClick(sender, null);
            }
        }

        private async void _OnSignInClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!CanSubmit)
            {
                return;
            }

            DisplayErrorMessage(null);
            try
            {
                Processing = true;
                await ServiceProvider.YouTrackService.Login(UsernameInput.Text, PasswordInput.Password);
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
            finally
            {
                Processing = false;
            }
        }


        private void DisplayErrorMessage(Exception e)
        {
            string message = "";
            if (e != null)
            {
                message = e.Message;
            }
            ErrorOutput.Dispatcher.BeginInvoke(new Action(() => ErrorOutput.Text = message));
        }
    }
}
