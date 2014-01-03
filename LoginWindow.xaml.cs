namespace Hbo.Sheepish
{
    using System;
    using System.Windows.Input;

    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        public string OauthCode { get; private set; }

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
            try
            {
                await ServiceProvider.YouTrackService.Login(UsernameInput.Text, PasswordInput.Password);
                this.Close();
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex.Message, ex);
            }
        }


        private void DisplayErrorMessage(string message, Exception ex = null)
        {
            ErrorOutput.Dispatcher.BeginInvoke(
                new Action(() => ErrorOutput.Text = message));
        }
    }
}
