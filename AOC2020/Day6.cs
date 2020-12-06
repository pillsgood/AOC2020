using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public partial class Day6 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<IEnumerable<char[][]>>>(provider =>
                new PuzzleInput<IEnumerable<char[][]>>(provider, Process));
        }

        private static IEnumerable<char[][]> Process(string value)
        {
            return value.Split("\n\n", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Select(group => group.Split('\n').Select(person => person.ToArray()).ToArray());
        }

        [Part(1)]
        private string Part1(IPuzzleInput<IEnumerable<char[][]>> input)
        {
            var answer = input.Value
                .Select(group => group.SelectMany(chars => chars.Select(c => c)).Distinct().ToArray())
                .Aggregate(0, (i, chars) => i + chars.Length);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IPuzzleInput<IEnumerable<char[][]>> input)
        {
            var answer = input.Value
                .Select(group => group.SelectMany(person => person.Select(c => c))
                    .Distinct()
                    .Count(c => group.All(person => person.Contains(c))))
                .Aggregate((i, i1) => i + i1);
            return answer.ToString();
        }
    }
}