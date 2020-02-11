using Eco.Gameplay.Players;
using System.Collections.Generic;
using System.Linq;

namespace Eco.Plugins.EcoApiExportMod
{

    public class api_user
    {
        public int? id { get; set; }
        public string slg_id { get; set; }
        public string steam_id { get; set; }
        public string name { get; set; }
        public double? total_play_time { get; set; }
        public double? last_online_time { get; set; }
        public bool online { get; set; }
        public List<store_offers> store_offers { get; set; }
    }

    class UserCollector
    {
        public static User GetUserByName(string UserName) => UserManager.FindUserByName(UserName);
        public static Player GetPlayerByName(string PlayerName) => GetUserByName(PlayerName).Player;
        public static User GetUser(string Filter) => UserManager.Users.FirstOrDefault(u => u.Name == Filter || u.SlgId == Filter || u.SteamId == Filter);

        private TradeCollector trade_collector;

        public UserCollector()
        {
            trade_collector = new TradeCollector();
        }

        public List<api_user> collect()
        {

            List<api_user> api_users = new List<api_user>();
            List<User> eco_users = UserManager.Users.ToList();
            if (eco_users != null && eco_users.Count > 0)
            {
                IEnumerable<string> online_users = UserManager.OnlineUsers.Where(user => user.Client.Connected)
                    .Select(user => user.Name);

                foreach (User user in eco_users)
                {
                    bool user_online_status = false;
                    if (online_users.Count() > 0 && user.Name != null)
                    {
                        user_online_status = online_users.Contains(user.Name);
                    }

                    api_users.Add(new api_user
                    {
                        id = user.Id,
                        slg_id = Collector.FirstNonEmptyString(user.SlgId),
                        steam_id = Collector.FirstNonEmptyString(user.SteamId),
                        name = Collector.FirstNonEmptyString(user.Name),
                        total_play_time = user.TotalPlayTime,
                        last_online_time = user.LastOnlineTime,
                        online = user_online_status,
                        store_offers = trade_collector.collect(user)
                    });
                }
            }
            else
            {
                return new List<api_user>();
            }


            return api_users;
        }
    }
}
