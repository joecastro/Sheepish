namespace Hbo.Sheepish
{
    using System;
    using System.Diagnostics;
    using System.Net;
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
            if (string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                DisplayErrorMessage("Cannot login without a username!");
            }
            else if (string.IsNullOrWhiteSpace(PasswordInput.Password))
            {
                DisplayErrorMessage("Cannot login without a password!");
            }
            else
            {
                try
                {
                    bool authenticated = await ServiceProvider.YouTrackService.LoginAsync(UsernameInput.Text, PasswordInput.Password);

                    if (authenticated)
                    {
                        // DialogResult is a nullable bool. Setting it to true 
                        // (or false) will cause the dialog to close and the 
                        // result to be returned to the caller. Therefore, we'll 
                        // only set this property when the user has successfully 
                        // authenticated.
                        DialogResult = true;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError
                        && ex.Message.Contains("(403)"))
                    {
                        DisplayErrorMessage("Authentication Failed. Please re-type your credentials and retry", ex);
                    }
                    else throw;
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unexpected error: " + ex.Message, ex);
                }
            }
        }


        private void DisplayErrorMessage(string message, Exception ex = null)
        {
            Trace.TraceWarning("LoginWindow: " + message);
            ErrorOutput.Dispatcher.BeginInvoke(
                new Action(() => ErrorOutput.Text = message));
        }
    }
}
