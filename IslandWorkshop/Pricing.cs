using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandWorkshop
{
    public class Pricing
    {
        public static Dictionary<Popularity, double> PopularityModifiers = new Dictionary<Popularity, double>() {
            { Popularity.VeryHigh, 1.4 },
            { Popularity.High, 1.2 },
            { Popularity.Average, 1.0 },
            { Popularity.Low, 0.8 }
        };

        public static Dictionary<Supply, double> SupplyModifiers = new Dictionary<Supply, double>() {
            { Supply.Nonexistent, 1.6 },
            { Supply.Insufficient, 1.3 },
            { Supply.Sufficient, 1.0 },
            { Supply.Surplus, 0.8 },
            { Supply.Overflowing, 0.6 }
        };

        public static int CalculatePrice(Handicraft handicraft, int groove=0, int efficiency=0)
        {
            return Convert.ToInt32(handicraft.BaseValue * PopularityModifiers[handicraft.Popularity] * SupplyModifiers[handicraft.Supply]);
        }
    }
}
