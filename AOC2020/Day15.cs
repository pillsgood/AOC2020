using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day15 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<IEnumerable<int>>>(provider =>
                new PuzzleInput<IEnumerable<int>>(provider, Process));
        }

        private static IEnumerable<int> Process(string value)
        {
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
        }

        [Part(1)]
        private string Part1(IPuzzleInput<IEnumerable<int>> input)
        {
            var memory = input.Value.ToList();
            for (int idx = memory.Count; idx < 2020; idx++)
            {
                var turn = memory.LastIndexOf(memory[idx - 1], idx - 2, idx - 1);
                memory.Add(idx - 1 - (turn == -1 ? idx - 1 : turn));
            }

            var answer = memory[2020 - 1];
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IPuzzleInput<IEnumerable<int>> input)
        {
            var start = input.Value.ToArray();
            var memoized = start[..^1].Zip(Enumerable.Range(0, start.Length - 1))
                .ToDictionary(tuple => tuple.First, tuple => tuple.Second);
            var answer = Enumerable.Range(start.Length - 1, 30000000 - start.Length)
                .Aggregate(start.Last(), (key, idx) =>
                {
                    var turn = memoized.ContainsKey(key) ? memoized[key] : idx;
                    memoized[key] = idx;
                    return idx - turn;
                });

            return answer.ToString();
        }
    }
}