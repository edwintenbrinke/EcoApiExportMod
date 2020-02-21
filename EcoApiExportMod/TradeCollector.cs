using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using System.Collections.Generic;
using System.Linq;

namespace Eco.Plugins.EcoApiExportMod
{
    public class store_offers
    {
        public int id { get; set; }
        public string name { get; set; }
        public string currency_name { get; set; }
        public List<formatted_offer> formatted_offers { get; set; }
    }

    public class formatted_offer
    {
        public int? type_id { get; set; }
        public float price { get; set; }
        public int limit { get; set; }
        public bool buying { get; set; }
        public float min_durability { get; set; }
        public bool has_min_durability { get; set; }
        public string name { get; set; }
        public bool is_set { get; set; }
        public int max_num_wanted { get; set; }
        public bool should_limit { get; set; }
    }

    class TradeCollector
    {

        public List<store_offers> collect(User user)
        {
            var result = WorldObjectManager.All.SelectMany(o => o.Components.OfType<StoreComponent>())
                .Where(store => store.Parent.OwnerUser == user);

            var result2 = result.Select(store => new { store.AllOffers, store.Parent.Name, store.Parent.ID, store.Parent.GetComponent<CreditComponent>().Currency.CurrencyName });

            List<store_offers> user_trade_results = new List<store_offers>();
            foreach (var res in result2)
            {
                List<formatted_offer> offers = new List<formatted_offer>();
                foreach (var r in res.AllOffers)
                { 
                    if (r.MaxNumWanted == 0)
                    {
                        continue;
                    }

                    offers.Add(new formatted_offer
                    {
                        price = r.Price,
                        limit = r.Limit,
                        buying = r.Buying,
                        name = Collector.FirstNonEmptyString(r.Stack.Item.DisplayName, r.SelectedItem.Item.DisplayName),
                        type_id = r.Stack.Item.TypeID,
                        min_durability = r.MinDurability,
                        has_min_durability = r.HasMinDurability,
                        is_set = r.IsSet,
                        max_num_wanted = r.MaxNumWanted,
                        should_limit = r.ShouldLimit
                    });
                }
                user_trade_results.Add(new store_offers
                {
                    id = res.ID,
                    name = Collector.FirstNonEmptyString(res.Name),
                    currency_name = Collector.FirstNonEmptyString(res.CurrencyName),
                    formatted_offers = offers
                });
            }
            return user_trade_results;
        }
    }
}
