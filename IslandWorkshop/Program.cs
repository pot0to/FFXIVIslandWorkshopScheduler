// See https://aka.ms/new-console-template for more information
using IslandWorkshop;
using Newtonsoft.Json;

Dictionary<Category, List<Handicraft>> handicraftsByCategory = Parser.ParseHandicrafts(@"Values/2022_09_13.csv");

System.Console.WriteLine("Category\tCount\tValue\tSchedule");

var scheduler = new Scheduler(handicraftsByCategory);
var cowrieCompilers = new List<KeyValuePair<Category, Schedule>>();
for (int i=0; i < 5; i++)
{
    var cowrieCompiler = scheduler.GetCowrieCompiler();
    cowrieCompilers.Add(cowrieCompiler);

    // remove the selected handicrafts from the running and find the next
    // highest cowrie compiler
    foreach (var handicraftToRemove in cowrieCompiler.Value.Handicrafts)
    {
        foreach (var category in handicraftToRemove.Categories)
        {
            handicraftsByCategory[category].Remove(handicraftToRemove);
        }
    }

    System.Console.WriteLine($"{cowrieCompiler.Key}\t" +
            $"{cowrieCompiler.Value.Handicrafts.Count}\t" +
            $"{cowrieCompiler.Value.Value}\t" +
            $"{string.Join(',', cowrieCompiler.Value.Handicrafts.Select(h => h.Name))}\t");

    scheduler = new Scheduler(handicraftsByCategory);
}