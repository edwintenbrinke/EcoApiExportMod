using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using System;
using System.Threading;

namespace Eco.Plugins.EcoApiExportMod
{
    public class previous_run_data
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class config
    {
        public bool debug { get; set; }
        public string api_access_token { get; set; }
        public int timeout { get; set; }
        public int db_query_limit { get; set; }
        public string base_api_url { get; set; }
        public previous_run_data[] previous_run_data { get; set; }
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
            try
            {
                new Thread(() => { collector.collect(); })
                {
                    Name = "Collector"
                }.Start();
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
            }
        }
    }
}
