using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Eco.Plugins.EcoApiExportMod
{
    public class Collector
    {
        public const string config_file_name = "config.json";
        private int POLL_DELAY;
        private UserCollector user_collector;
        private ServerCollector server_collector;

        public Collector()
        {
            user_collector = new UserCollector();
            server_collector = new ServerCollector();
        }

        public static string FirstNonEmptyString(params string[] strings)
        {
            return strings.FirstOrDefault(str => !String.IsNullOrEmpty(str)) ?? "";
        }

        static config getConfigData()
        {
            // load config file
            Logger.Debug("loading config data");
            string config_file_location = string.Format("{0}{1}", base_mod_path, config_file_name);
            string config_json = File.ReadAllText(config_file_location);
            return JsonConvert.DeserializeObject<config>(config_json);
        }

        public void collect()
        {
            while (true)
            {
                config config_data = getConfigData();
                // find if server is running check
                Logger.Debug("collecting");
                try
                {
                    List<dynamic> api_data = new List<dynamic>();

                    // get users
                    Logger.Debug("collecting users");
                    Dictionary<string, List<api_user>> api_users = new Dictionary<string, List<api_user>>();
                    api_users.Add("users", user_collector.collect());
                    api_data.Add(api_users);

                    // get server_info
                    Logger.Debug("collecting server info");
                    Dictionary<string, api_server> api_server = new Dictionary<string, api_server>();
                    api_server.Add("server", server_collector.collect());
                    api_data.Add(api_server);

                    Api.Post("/api/eco/mod/data", config_data, api_data);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                Thread.Sleep(config_data.timeout);
            }
        }
    }
}
