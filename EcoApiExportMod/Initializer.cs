using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using System;
using System.Threading;

namespace Eco.Plugins.EcoApiExportMod
{
    public class config
    {
        public string api_access_token { get; set; }
        public int timeout { get; set; }
        public string api_url { get; set; }
    }

    public class data_models
    {
        public int _id { get; set; }
        public int TimeSeconds { get; set; }
        public string Username { get; set; }
        public Guid AuthId { get; set; }
        public Guid WorldObjectId { get; set; }
        public double Value { get; set; }
        public string ItemTypeName { get; set; }
        public string SpeciesName { get; set; }
        public string WorldObjectTypeName { get; set; }
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
                Logger.Error(ex.Message);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
