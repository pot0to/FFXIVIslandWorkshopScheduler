using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CsvHelper;
using System.Globalization;

namespace IslandWorkshop
{
    public enum Category
    {
        PreservedFood,
        Attire,
        Foodstuffs,
        Confections,
        Sundries,
        Furnishings,
        Arms,
        Concoctions,
        Ingredients,
        Accessories,
        Metalworks,
        Woodworks,
        Textiles,
        CreatureCreations,
        MarineMerchandise,
        UnburiedTreasures
    }

    public enum Popularity
    {
        VeryHigh,
        High,
        Average,
        Low
    }

    public enum Supply
    {
        Nonexistent,
        Insufficient,
        Sufficient,
        Surplus,
        Overflowing
    }

    public class Handicraft
    {
        public string Name;
        public int Time;
        public List<Category> Categories;
        public int BaseValue;
        public Popularity Popularity;
        public Supply Supply;
        public int Value;

        public Handicraft(string name, int time, string[] categories, int baseValue)
        {
            this.Name = name;
            this.Time = time;
            this.Categories = new List<Category>();
            this.BaseValue = baseValue;
            foreach (var categoryString in categories)
            {
                Category category;
                if (Enum.TryParse(categoryString.Replace(" ", ""), out category))
                {
                    this.Categories.Add(category);
                }
                else
                {
                    throw new Exception($"Could not parse category \"{categoryString}\" of handicraft \"{name}\"");
                }
            }
        }
    }

    internal class Parser
    {
        public static Dictionary<Category, List<Handicraft>> ParseHandicrafts(string handicraftsSupplyAndDemandFile)
        {
            Dictionary<Category, List<Handicraft>> handicraftsByCategory = new Dictionary<Category, List<Handicraft>>();
            Dictionary<string, Handicraft> handicraftsByName = new Dictionary<string, Handicraft>();

            String jsonString = File.ReadAllText("Handicrafts.json");
            List<dynamic> allHandicraftsJson = JsonConvert.DeserializeObject<List<dynamic>>(jsonString);
            foreach (var handicraftJson in allHandicraftsJson)
            {
                string name = handicraftJson.Name;
                int time = handicraftJson.Time;
                string[] categories = handicraftJson.Category.ToObject<string[]>();
                int baseValue = handicraftJson.BaseValue;
                Handicraft handicraft = new Handicraft(name, time, categories, baseValue);

                foreach (var category in handicraft.Categories)
                {
                    if (handicraftsByCategory.ContainsKey(category))
                    {
                        handicraftsByCategory[category].Add(handicraft);
                    }
                    else
                    {
                        handicraftsByCategory.Add(category, new List<Handicraft>() { handicraft });
                    }
                }
                handicraftsByName.Add(handicraft.Name, handicraft);
            }
            ParseHandicraftSupplyAndDemand(handicraftsSupplyAndDemandFile, handicraftsByName);
            return handicraftsByCategory;
        }

        public class HandicraftValueParser
        {
            public string Name { get; set; }
            public string Popularity { get; set; }
            public string Supply { get; set; }
        }

        public static void ParseHandicraftSupplyAndDemand(string handicraftsValuesFile, Dictionary<string, Handicraft> handicraftsByName)
        {
            using (var reader = new StreamReader(handicraftsValuesFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // var handicraftTemplate = new HandicraftValueParser();
                // var allHandicraftsValues = csv.EnumerateRecords(handicraftTemplate);
                var records = csv.GetRecords<HandicraftValueParser>();
                foreach (HandicraftValueParser handicraftValue in records)
                {
                    var tryParsePopularity = Enum.TryParse(handicraftValue.Popularity.Replace(" ", ""),
                        out handicraftsByName[handicraftValue.Name].Popularity);
                    if (!tryParsePopularity)
                    {
                        throw new Exception($"Cound not parse popularity of {handicraftValue.Name}");
                    }

                    var tryParseSupply = Enum.TryParse(handicraftValue.Supply, out handicraftsByName[handicraftValue.Name].Supply);
                    if (!tryParseSupply)
                    {
                        throw new Exception($"Cound not parse supply of {handicraftValue.Name}");
                    }
                }
            }
        }

    }
}
