using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Eco.Plugins.EcoApiExportMod
{
    public class config
    {
        public string api_access_token { get; set; }
        public int timeout { get; set; }
        public string api_url { get; set; }
    }

    public class Initializer : IModKitPlugin, IInitializablePlugin
    {
        private string status = "initializing";
        protected Collector collector;

        public Initializer()
        {
            collector = new Collector();
        }

        public override string ToString()
        {
            return "EcoApiExportMod";
        }

        public string GetStatus()
        {
            return status;
        }

        public void Initialize(TimedTask timer)
        {
            Logger.DebugVerbose("initializing");
            try
            {
                new Thread(() => { collector.collect(); })
                {
                    Name = "Collector"
                }.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}
