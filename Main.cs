namespace Hbo.Sheepish
{
    using System;
    using System.Reflection;
    using Standard;

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
        }
    }
}