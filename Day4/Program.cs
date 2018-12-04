using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new Day4Calculator();
            var result = calc.Problem1Calculator(Resource.Input);

            Console.WriteLine($"Problem 1 Result: {result}");

            var result2 = calc.Problem2Calculator(Resource.Input);

            Console.WriteLine($"Problem 2 Result: {result2}");
            Console.ReadLine();
        }
    }

    public class Day4Calculator
    {
        public int Problem1Calculator(
            string input)
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var lineDatas = GetLineDatasFromLines(lines);

            var sleepTimes = GetGuardSleepTimes(lineDatas);

            var groupedByGuardNumber = sleepTimes.GroupBy(g => g.GuardNumber).ToDictionary(i => i.Key, i => i.ToList());

            var sleepyGuards = groupedByGuardNumber.ToDictionary(i => i.Key, i => i.Value.Sum(j => j.SleepMinutes));

            var sleepiestGuard = sleepyGuards.SingleOrDefault(kvp => kvp.Value == sleepyGuards.Values.Max());

            var allSleepMinutes = groupedByGuardNumber[sleepiestGuard.Key].SelectMany(i => i.MinutesAsleep);

            var groupedByMinute = allSleepMinutes.GroupBy(s => s.Minute);

            var minute = groupedByMinute.OrderByDescending(i => i.Count()).First();

            return sleepiestGuard.Key * minute.Key;
        }

        public int Problem2Calculator(
            string input)
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var lineDatas = GetLineDatasFromLines(lines);

            var sleepTimes = GetGuardSleepTimes(lineDatas);

            var groupedByGuardNumber = sleepTimes.GroupBy(g => g.GuardNumber).ToDictionary(i => i.Key, i => i.ToList());

            var guardsWithGroupedMinutes = groupedByGuardNumber.ToDictionary(i => i.Key,
                i => i.Value.SelectMany(j => j.MinutesAsleep).GroupBy(s => s.Minute)
                    .ToDictionary(s => s.Key, s => s.Count()));

            var maxPerGuard = guardsWithGroupedMinutes
                .Select(i => new {Key = i.Key, Minute = i.Value.OrderByDescending(j => j.Value).First()})
                .OrderByDescending(i => i.Minute.Value)
                .First();

            return maxPerGuard.Key * maxPerGuard.Minute.Key;
        }

        private List<GuardSleepTime> GetGuardSleepTimes(
            List<LineData> lineDatas)
        {
            var guardSleepTimes = new List<GuardSleepTime>();

            var currentGuardNumber = -1;
            DateTime? startDate = null;

            foreach (var line in lineDatas.OrderBy(d => d.DateTime))
            {
                if (line.Event == EventEnum.StartsShift)
                {
                    if (currentGuardNumber != -1 && startDate.HasValue)
                    {
                        guardSleepTimes.Add(new GuardSleepTime()
                            {End = line.DateTime, GuardNumber = currentGuardNumber, Start = startDate.Value});

                        startDate = null;
                    }

                    currentGuardNumber = line.GuardNumber;
                }

                if (line.Event == EventEnum.FallsAsleep)
                {
                    startDate = line.DateTime;
                }

                if (line.Event == EventEnum.WakesUp)
                {
                    if (currentGuardNumber != -1 && startDate.HasValue)
                    {
                        guardSleepTimes.Add(new GuardSleepTime()
                            {End = line.DateTime.AddMinutes(-1), GuardNumber = currentGuardNumber, Start = startDate.Value});

                        startDate = null;
                    }
                }
            }

            guardSleepTimes.ForEach(g => g.CalculateSleepMinutes());

            return guardSleepTimes;
        }

        private List<LineData> GetLineDatasFromLines(
            string[] lines)
        {
            var lineDatas = new List<LineData>();

            var currentGuardNumber = -1;

            foreach (var line in lines)
            {
                var date = GetDateFromLine(line);
                var guardNumber = GetGuardNumFromLine(line);

                var thisEvent = GetEventEnumFromLine(line);

                lineDatas.Add(new LineData() {DateTime = date, Event = thisEvent, GuardNumber = guardNumber});
            }

            var sortedLineDatas = lineDatas.OrderBy(i => i.DateTime);

            foreach (var line in sortedLineDatas)
            {
                if (line.GuardNumber == -1)
                    line.GuardNumber = currentGuardNumber;
                else
                    currentGuardNumber = line.GuardNumber;
            }

            return lineDatas;
        }

        private DateTime GetDateFromLine(
            string line)
        {
            var dateString = new string(line.ToCharArray().Skip(1).TakeWhile(i => i != ']').ToArray());

            return DateTime.Parse(dateString);
        }

        private int GetGuardNumFromLine(string line)
        {
            var guardPrefix = "Guard #";

            var indexOfGuardPrefix = line.IndexOf(guardPrefix);
            if (indexOfGuardPrefix == -1) return -1;

            var indexOfAfterGuard = line.IndexOf(' ', indexOfGuardPrefix + guardPrefix.Length);

            return int.Parse(line.Substring(indexOfGuardPrefix + guardPrefix.Length,
                indexOfAfterGuard - indexOfGuardPrefix - guardPrefix.Length));
        }

        private EventEnum GetEventEnumFromLine(
            string line)
        {
            if (line.Contains(EventEnum.WakesUp.ToDescription()))
                return EventEnum.WakesUp;
            if (line.Contains(EventEnum.FallsAsleep.ToDescription()))
                return EventEnum.FallsAsleep;
            if (line.Contains(EventEnum.StartsShift.ToDescription()))
                return EventEnum.StartsShift;

            throw new Exception("DED!");
        }
    }

    public class GuardSleepTime
    {
        public int GuardNumber { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int SleepMinutes { get;set; }
        public List<BasicDateTime> MinutesAsleep {get; set; }
        public void CalculateSleepMinutes()
        {
            var timeSpan = End.Subtract(Start);

            SleepMinutes = (int)timeSpan.TotalMinutes;

            MinutesAsleep = new List<BasicDateTime>();

            for (var counterMinute = Start;
                counterMinute.CompareTo(End) <= 0;
                counterMinute = counterMinute.AddMinutes(1))
            {
                MinutesAsleep.Add(new BasicDateTime(counterMinute));
            }
        }
    }

    public class BasicDateTime
    {
        public BasicDateTime(
            DateTime counterMinute)
        {
            Year = counterMinute.Year;
            Month = counterMinute.Month;
            Day = counterMinute.Day;

            Minute = counterMinute.Hour * 60 + counterMinute.Minute;
        }

        public int Year { get; set; }
        public int Month {get; set; }
        public int Day { get; set; }

        public int Minute { get; set; }
    }

    public class LineData
    {
        public DateTime DateTime { get; set; }
        public int GuardNumber { get; set; }
        public EventEnum Event { get; set; }
    }

    public enum EventEnum
    {
        [Description("wakes up")]
        WakesUp,
        [Description("falls asleep")]
        FallsAsleep,
        [Description("begins shift")]
        StartsShift
    }

    public static class Extensions
    {
        public static string ToDescription(this Enum e)
        {
            if (e == null) return string.Empty;

            var field = e.GetType().GetField(e.ToString());
            if (field == null) return String.Empty;

            var descAttr = field.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>();
            return descAttr.Any() ? descAttr.First().Description : e.ToString();
        }
    }
}
