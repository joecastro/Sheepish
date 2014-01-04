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