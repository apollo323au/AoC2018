using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc2 = new Day7Calc();
            var result = calc2.Calc1(Resource.Input);

            Console.WriteLine($"Result 1: {result}");

            var result2 = calc2.Calc2(Resource.Input);
            Console.WriteLine($"Result2 : {result2}");
            Console.ReadLine();
        }
    }

    public class Day7Calc
    {
        public string Calc1(
            string input)
        {
            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var stepsWithRequiredSteps = GetStepsWithRequiredSteps(lines);

            var firstStep = stepsWithRequiredSteps.Where(s => s.RequiredSteps.Count == 0).OrderBy(i => i.Step).FirstOrDefault();

            if (firstStep == null) return "nup!";

            var result = new List<string>();
            var remainingSteps = new List<StepwithRequiresSteps>(stepsWithRequiredSteps);

            result.Add(firstStep.Step);
            remainingSteps.Remove(firstStep);

            while (remainingSteps.Any())
            {
                var next = remainingSteps.Where(i => i.RequiredSteps.All(rs => result.Contains(rs)))
                    .OrderBy(i => i.Step)
                    .FirstOrDefault();


                result.Add(next.Step);
                remainingSteps.Remove(next);
            }

            return result.Aggregate((
                    i,
                    j) => i + j);
        }

        private StepInfo[] GetSteps(
            string[] lines)
        {
            var steps = lines.Select(ExtractSteps).ToArray();
            return steps;
        }

        private static string[] GetDistinctSteps(
            StepInfo[] steps)
        {
            var distinctSteps = steps.Select(i => i.Step).Distinct().Union(steps.Select(i => i.RequiredStep)).ToArray();
            return distinctSteps;
        }

        private StepwithRequiresSteps[] GetStepsWithRequiredSteps(
            string[] lines)
        {
            var steps = GetSteps(lines);
            var distinctSteps = GetDistinctSteps(steps);
            var stepsWithRequiredSteps = distinctSteps.Select(s => CalculateRequireStepsForStep(s, steps)).ToArray();
            return stepsWithRequiredSteps;
        }

        private StepwithRequiresSteps CalculateRequireStepsForStep(
            string stepInfo,
            StepInfo[] stepsInfo)
        {
            var reqSteps = stepsInfo.Where(s => s.Step == stepInfo).Select(s => s.RequiredStep).ToList();
            return new StepwithRequiresSteps() { Step = stepInfo, RequiredSteps = reqSteps };
        }


        private StepInfo ExtractSteps(
            string line)
        {
            var steps = line.Replace("Step ", "")
                .Replace(" must be finished before step ", "")
                .Replace(" can begin.", "")
                .ToCharArray();

            return new StepInfo() { RequiredStep = new string(new[] { steps[0] }), Step = new string(new[] { steps[1] }) };
        }


        private List<string> _alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(i => i.ToString()).ToList();

        public int Calc2(
            string input)
        {
            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var stepsWithRequiredSteps = GetStepsWithRequiredSteps(lines);
            
            var remainingSteps = new List<StepwithRequiresSteps>(stepsWithRequiredSteps);
            var counter = 0;

            var worker1 = new Worker();
            var worker2 = new Worker();
            var worker3 = new Worker();
            var worker4 = new Worker();
            var worker5 = new Worker();

            var workers = new List<Worker>() {worker1, worker2, worker3, worker4, worker5};
            var finishedWork = new List<string>();
            
            while (remainingSteps.Any() || workers.Any(i => i.Step != null))
            {
                foreach (var worker in workers.Where(i => i.Step != null).ToArray())
                {
                    if (worker.TargetCounter != counter) continue;

                    finishedWork.Add(worker.Step.Step);
                    worker.Step = null;
                    worker.TargetCounter = -1;
                }

                var stepToProcess = workers.All(i => i.Step == null) && finishedWork.Any() == false
                    ? remainingSteps.Where(s => s.RequiredSteps.Count == 0).OrderBy(i => i.Step).ToList()
                    : remainingSteps.Where(i => i.RequiredSteps.All(rs => finishedWork.Contains(rs)))
                        .OrderBy(i => i.Step)
                        .ToList();

                foreach (var step in stepToProcess)
                {
                    var worker = workers.FirstOrDefault(i => i.Step == null);
                    if (worker == null) break;

                    worker.Step = step;
                    worker.TargetCounter = counter + _alpha.IndexOf(step.Step) + 61;

                    remainingSteps.Remove(step);
                }

                counter++;
            }

            return counter - 1;
        }
    }

    public class Worker
    {

        public Worker()
        {
            Step = null;
            TargetCounter = -1;
        }
        public StepwithRequiresSteps Step {get; set; }
        public int TargetCounter {get; set; }
    }

    public class StepInfo
    {
        public string RequiredStep { get; set; }
        public string Step { get; set; }
    }

    public class StepwithRequiresSteps
    {
        public List<string> RequiredSteps { get; set; } = new List<string>();
        public string Step { get; set; }
    }

}
