using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tagurBot
{
    public static class Common
    {
        public static ConnectorClient Connector { get; set; }
        public static Activity Activity { get; set; }
        public static bool IsInitialized { get; set; }
    }
}