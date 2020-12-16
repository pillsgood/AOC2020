using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day13 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<Input>(provider, Parse).Value);
        }

        private record Input(int Arrival, int[] BusIds);

        private static Input Parse(string input)
        {
            var lines = input.Split('\n');
            var arrival = int.Parse(lines[0]);
            var buses = lines[1].Split(',').Select(s => s != "x" ? s : "-1").Select(int.Parse);
            return new Input(arrival, buses.ToArray());
        }

        [Part(1)]
        private string Part1(Input input)
        {
            var buses = input.BusIds.Where(i => i != -1).OrderBy(i => i - input.Arrival % i).ToArray();
            var time = buses[0] - input.Arrival % buses[0];
            var answer = buses[0] * time;
            return answer.ToString();
        }


        [Part(2)]
        private string Part2(Input input)
        {
            var mods = input.BusIds.Where(i => i != -1).ToArray();
            var remainders = input.BusIds.Select((i, idx) => i == -1 ? -1 : (i - idx) % i).Where(i => i != -1)
                .ToArray();
            var product = mods.Aggregate<int, long>(1, (i, j) => i * j);
            var answer = mods.Select((i, idx) => product / mods[idx])
                .Select((p, idx) => remainders[idx] * ModInv(p, mods[idx]) * p).Sum() % product;
            return answer.ToString();

            static long ModInv(long a, int mod) =>
                Enumerable.Range(1, mod - 1).Cast<int?>().FirstOrDefault(i => a % mod * i % mod == 1) ?? 1;
        }
    }
}