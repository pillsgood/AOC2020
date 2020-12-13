using System;
using System.Collections.Generic;
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
            services.AddSingleton<IPuzzleInput<KeyValuePair<int, int[]>>>(provider =>
                new PuzzleInput<KeyValuePair<int, int[]>>(provider, Process));
        }

        private static KeyValuePair<int, int[]> Process(string value)
        {
            var input = value.Split('\n');
            var arrival = int.Parse(input[0]);
            var buses = input[1].Split(',').Select(s => s != "x" ? s : "-1").Select(int.Parse);
            return new KeyValuePair<int, int[]>(arrival, buses.ToArray());
        }


        [Part(1)]
        private string Part1(IPuzzleInput<KeyValuePair<int, int[]>> input)
        {
            var buses = input.Value.Value.Where(i => i != -1).OrderBy(i => i - input.Value.Key % i).ToArray();
            var time = buses[0] - input.Value.Key % buses[0];
            var answer = buses[0] * time;
            return answer.ToString();
        }


        [Part(2)]
        private string Part2(IPuzzleInput<KeyValuePair<int, int[]>> input)
        {
            var ids = input.Value.Value;
            var mods = ids.Where(i => i != -1).ToArray();
            var remainders = ids.Select((i, idx) => i == -1 ? -1 : (i - idx) % i).Where(i => i != -1).ToArray();
            var product = mods.Aggregate<int, long>(1, (i, j) => i * j);
            var answer = mods.Select((i, idx) => product / mods[idx])
                .Select((p, idx) => remainders[idx] * ModInv(p, mods[idx]) * p).Sum() % product;
            return answer.ToString();

            static long ModInv(long a, int mod) =>
                Enumerable.Range(1, mod - 1).Cast<int?>().FirstOrDefault(i => a % mod * i % mod == 1) ?? 1;
        }
    }
}