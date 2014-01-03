using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hbo.Sheepish
{
    internal static class ServiceProvider
    {
        public static YouTrackService YouTrackService { get; private set; }
        public static ViewModel ViewModel { get; private set; }

        static ServiceProvider()
        {
            YouTrackService = new YouTrackService("https://youtrack.hbo.com/youtrack/rest");
            ViewModel = new ViewModel();
        }
    }
}
