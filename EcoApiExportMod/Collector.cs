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
        public string base_mod_path;
        private int POLL_DELAY;
        private UserCollector user_collector;
        private ServerCollector server_collector;
        private DatabaseCollector database_collector;

        public Collector()
        {
            user_collector = new UserCollector();
            server_collector = new ServerCollector();
            database_collector = new DatabaseCollector();
        }

        public static string FirstNonEmptyString(params string[] strings)
        {
            return strings.FirstOrDefault(str => !String.IsNullOrEmpty(str)) ?? "";
        }

        public static config getConfigData(string base_path)
        {
            Logger.Debug("Loading config");
            // load config file
            string config_file_location = string.Format("{0}{1}", base_path, config_file_name);
            string config_json = File.ReadAllText(config_file_location);
            return JsonConvert.DeserializeObject<config>(config_json);
        }

        public void collect()
        {
            while (true)
            {
                config config_data = getConfigData(string.Format("{0}Mods/EcoApiExportMod/", AppDomain.CurrentDomain.BaseDirectory));
                Logger.debug = config_data.debug;
                // find if server is running check
                try
                {
                    // handle game data
                    List<dynamic> api_data = new List<dynamic>();

                    // get users
                    Dictionary<string, List<api_user>> api_users = new Dictionary<string, List<api_user>>();
                    api_users.Add("users", user_collector.collect());
                    api_data.Add(api_users);

                    // get server_info
                    Dictionary<string, api_server> api_server = new Dictionary<string, api_server>();
                    api_server.Add("server", server_collector.collect());
                    api_data.Add(api_server);

                    Api.Post("/api/eco/mod/data", config_data, api_data);

                    // handle database data
                    database_collector.collect(config_data);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.Message);
                }

                Thread.Sleep(config_data.timeout);
            }
        }
    }
}
