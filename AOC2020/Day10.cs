using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day10 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<IEnumerable<int>>>(provider =>
                new PuzzleInput<IEnumerable<int>>(provider, Process));
        }

        private static IEnumerable<int> Process(string value) =>
            value.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

        [Part(1)]
        private string Part1(IPuzzleInput<IEnumerable<int>> input)
        {
            var values = input.Value.OrderBy(i => i).Prepend(0).Append(input.Value.Max() + 3).ToArray();
            values = values[1..].Select((i, idx) => i - values[idx]).ToArray();
            var answer = values.Count(i => i == 1) * values.Count(i => i == 3);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IPuzzleInput<IEnumerable<int>> input)
        {
            var adapters = input.Value.OrderByDescending(i => i).Append(0).Prepend(input.Value.Max() + 3).ToArray();
            var permutations = new Dictionary<int, long>();
            foreach (var key in adapters.Skip(1))
            {
                permutations[key] = adapters.Where(i => Math.Abs(i - key) <= 3 && i > key)
                    .Sum(i => permutations.ContainsKey(i) ? permutations[i] : 1);
            }

            var answer = permutations[0];
            return answer.ToString();
        }
    }
}