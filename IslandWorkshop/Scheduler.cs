using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandWorkshop
{
    public class Schedule
    {
        public List<Handicraft> Handicrafts;
        public int Value;
        public int Time;

        public Schedule()
        { 
            this.Handicrafts = new List<Handicraft>();
            this.Value = 0;
            this.Time = 0;
        }

        public Schedule Add(Handicraft newHandicraft)
        {
            // create a deep copy
            var newSchedule = CopySchedule();
            if (newHandicraft.Name != newSchedule.Handicrafts.LastOrDefault()?.Name)
            {
                newSchedule.Handicrafts.Add(newHandicraft);
                newSchedule.Value += Pricing.CalculatePrice(newHandicraft);
                newSchedule.Time += newHandicraft.Time;
            }
            return newSchedule;
        }

        private Schedule CopySchedule()
        {
            Schedule newSchedule = new Schedule();
            foreach (var handicraft in this.Handicrafts)
            {
                newSchedule.Handicrafts.Add(handicraft);
            }
            newSchedule.Time = Time;
            newSchedule.Value = Value;

            return newSchedule;
        }
    }

    public class Scheduler
    {
        public Dictionary<Category, List<Handicraft>> HandicraftsByCategory;
        public Dictionary<Category, Schedule> SchedulesByCategory;

        public Scheduler(Dictionary<Category, List<Handicraft>> handicraftsByCategory)
        {
            this.HandicraftsByCategory = handicraftsByCategory;
            this.Process();
        }

        public void Process()
        {
            this.SchedulesByCategory = this.Schedule();
        }

        public Dictionary<Category, Schedule> Schedule()
        {
            Dictionary<Category, Schedule> schedulesByCategory = new Dictionary<Category, Schedule>();

            foreach(var category in HandicraftsByCategory.Keys)
            {
                schedulesByCategory.Add(category, ScheduleCategory(category));
            }

            return schedulesByCategory;
        }

        public Schedule ScheduleCategory(Category category)
        {
            var totalHours = 25; // because we add a 0th hour as our base case

            // across 25 to represent 24h + 0th hour, down the number of items in the schedule
            var dpMatrix = new List<List<Schedule>>();
            // init the first row of dpMatrix
            var zeroItemList = new List<Schedule>();
            zeroItemList.Add(new Schedule());
            for (int i = 1; i<totalHours; i++)
            {
                zeroItemList.Add(new Schedule());
            }
            dpMatrix.Add(zeroItemList);

            var flag = true;
            var itemCount = 1;
            while (flag)
            {
                flag = false;
                var newItemList = new List<Schedule>();
                for (var hour=0; hour<totalHours; hour++)
                {
                    newItemList.Add(new Schedule());
                }
                dpMatrix.Add(newItemList);

                for (var hour = 1; hour < totalHours; hour++)
                {
                    // check the above and left cells to find the best schedule
                    // up until this point
                    var leftSchedule = dpMatrix[itemCount][hour - 1];
                    var aboveSchedule = dpMatrix[itemCount - 1][hour];

                    Schedule bestSchedule = leftSchedule;
                    if (aboveSchedule.Value > bestSchedule.Value)
                    {
                        bestSchedule = aboveSchedule;
                    }

                    foreach (var handicraft in this.HandicraftsByCategory[category])
                    {
                        if (hour - handicraft.Time < 0)
                        {
                            continue;
                        }
                        else
                        {
                            // Schedule previousSchedule = dpMatrix[itemCount - 1][hour - handicraft.Time];
                            var addHandicraftSchedule = dpMatrix[itemCount - 1][hour - handicraft.Time];
                            if (handicraft.Name == "Isleworks Silver Ear Cuffs" && itemCount == 2)
                            {

                            }
                            addHandicraftSchedule = addHandicraftSchedule.Add(handicraft);

                            if (addHandicraftSchedule.Value > bestSchedule.Value)
                            {
                                bestSchedule = addHandicraftSchedule;
                                flag = true;
                            }
                        }
                    }
                    dpMatrix[itemCount][hour] = bestSchedule;
                }
                itemCount++;
            }

            return dpMatrix[dpMatrix.Count - 1][totalHours-1];
        }

        public KeyValuePair<Category, Schedule> GetGrooveMover()
        {
            List<KeyValuePair<Category, Schedule>> grooveMovers = this.SchedulesByCategory.ToList();
            // grooveMovers.Sort((schedule1, schedule2) =>
            //     schedule1.Value.Handicrafts.Count.CompareTo(schedule2.Value.Handicrafts.Count));
            grooveMovers = grooveMovers.OrderBy(schedule => schedule.Value.Handicrafts.Count * -1).ToList();
            return grooveMovers[0];
        }

        public KeyValuePair<Category, Schedule> GetCowrieCompiler()
        {
            var cowrieCompilers = this.SchedulesByCategory.ToList();
            cowrieCompilers = cowrieCompilers.OrderBy(schedule =>schedule.Value.Value * -1).ToList();
            return cowrieCompilers[0];
        }

        //public Dictionary<Handicraft, float> CalculateRates(Category category)
        //{
        //    Dictionary<Handicraft, float> rates = new Dictionary<Handicraft, float>();

        //    foreach (var handicraft in this.HandicraftsByCategory[category])
        //    {
        //        float rate = (float) handicraft.Value / handicraft.Time;
        //        rates[handicraft] = rate;
        //    }
        //    return rates;
        //}

        //public Dictionary<int, Handicraft> DedupeByTime(Dictionary<Handicraft, float> handicraftRates)
        //{
        //    Dictionary<int, Handicraft> bestHandicraftForTime = new Dictionary<int, Handicraft>();
        //    foreach (var handicraft in handicraftRates.Keys)
        //    {
        //        var timeslot = handicraft.Time;
        //        if (bestHandicraftForTime.ContainsKey(timeslot))
        //        {
        //            var bestHandicraft = bestHandicraftForTime[timeslot];
        //            var handicraftRate = handicraftRates[handicraft];
        //            if (handicraftRate > handicraftRates[bestHandicraft])
        //            {
        //                bestHandicraftForTime[timeslot] = handicraft;
        //            }
        //        }
        //    }
        //    return bestHandicraftForTime;
        //}
    }
}
