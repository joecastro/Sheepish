namespace Hbo.Sheepish
{
    using Standard;
    using System;
    using System.Windows;

    public static class Sheepish
    {
        [STAThread]
        public static void Main()
        {
            if (SingleInstance.InitializeAsFirstInstance("Sheepish"))
            {
                // Explicitly set this to ensure the taskbar icon gets set properly, even if the app is pinned to the taskbar.
                // Otherwise when pinned the shell will display the application's icon rather than the Window's.
                NativeMethods.SetCurrentProcessExplicitAppUserModelID("Sheepish-" + Guid.NewGuid().ToString());

                var application = new Hbo.Sheepish.App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance.Cleanup();
            }
            else if (System.Diagnostics.Debugger.IsAttached)
            {
                MessageBox.Show(
                    "The application was started but the startup arguments are being delegated to another running instance.",
                    "Sheepish", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}