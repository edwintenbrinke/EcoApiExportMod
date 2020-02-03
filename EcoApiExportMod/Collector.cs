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
            string config_file_location = string.Format("{0}Mods/EcoApiExportMod/{1}", AppDomain.CurrentDomain.BaseDirectory, config_file_name);
            if (!File.Exists(config_file_location))
            {
                using (StreamWriter w = File.AppendText(config_file_location))
                {
                    w.WriteLine(JsonConvert.SerializeObject(new config
                    {
                        api_access_token = "xxxxx",
                        timeout = 5000,
                        api_url = "http://192.168.178.47:8000/api/eco/mod/data"
                    }));
                }
            }
            string config_json = File.ReadAllText(config_file_location);
            return JsonConvert.DeserializeObject<config>(config_json);
        }

        public void collect()
        {
            while (true)
            {
                config config_data = getConfigData();
                // find if server is running check
                Thread.Sleep(config_data.timeout);
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

                    postData(config_data, api_data);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            }
        }

        // post the data to the api
        static void postData(config config_data, List<dynamic> post_data)
        {
            //string data = JsonConvert.SerializeObject(post_data);
            Logger.Debug(string.Format("posting data to {0}", config_data.api_url));
            using (WebClient wc = new WebClient())
            {
                // set it so the api knows that type of data is being received
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers[HttpRequestHeader.Authorization] = config_data.api_access_token;
                // post to the api & transform the api array to JSON
                string HtmlResult = wc.UploadString(
                    config_data.api_url,
                    JsonConvert.SerializeObject(post_data)
                );
                Logger.Debug("posted to api");
            }
        }
    }
}
