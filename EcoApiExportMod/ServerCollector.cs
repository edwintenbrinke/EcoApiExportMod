using Eco.Plugins.Networking;
using Eco.Shared.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.Plugins.EcoApiExportMod
{
    public class api_server
    {
        public Guid id { get; set; }
        public string play_times { get; set; }
        public bool? has_meteor { get; set; }
        public string version { get; set; }
        public double? time_left { get; set; }
        public double? time_since_start { get; set; }
        public string description { get; set; }
        public string detailed_description { get; set; }
        public int? online_players { get; set; }
        public int? total_players { get; set; }
    }

    class ServerCollector
    {
        public api_server collect()
        {
            ServerInfo server_info;
            try
            {
                server_info = NetworkManager.GetServerInfo();
            }
            catch
            {
                return new api_server();
            }

            return new api_server
            {
                id = server_info.Id,
                play_times = Collector.FirstNonEmptyString(server_info.Playtimes),
                has_meteor = server_info.HasMeteor,
                version = Collector.FirstNonEmptyString(server_info.Version),
                time_left = server_info.TimeLeft,
                time_since_start = server_info.TimeSinceStart,
                description = Collector.FirstNonEmptyString(server_info.Description),
                detailed_description = Collector.FirstNonEmptyString(server_info.DetailedDescription),
                online_players = server_info.OnlinePlayers,
                total_players = server_info.TotalPlayers
            };
        }
    }
}
