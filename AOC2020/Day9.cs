using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day9 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<long[]>>(provider =>
                new PuzzleInput<long[]>(provider, Process));
        }

        private static long[] Process(string value) =>
            value.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s.Trim())).ToArray();

        private static IEnumerable<Tuple<long, long>> Pair(IEnumerable<long> source) => Pair(source, arg => arg);

        private static IEnumerable<Tuple<T, T>> Pair<T, TValue>(IEnumerable<T> source, Func<T, TValue> selector)
            where TValue : IComparable<TValue>
        {
            var enumerable = source as T[] ?? source.ToArray();
            return from item1 in enumerable
                from item2 in enumerable
                where selector(item1).CompareTo(selector(item2)) < 0
                select Tuple.Create(item1, item2);
        }

        [Part(1)]
        private string Part1(IPuzzleInput<long[]> input)
        {
            var values = input.Value.Zip(Enumerable.Range(0, input.Value.Length)).ToArray();
            var answer = _part1Answer = values[25..]
                .FirstOrDefault(tuple =>
                    Pair(values[(tuple.Second - 25)..tuple.Second].Select(valueTuple => valueTuple.First))
                        .All(pair => pair.Item1 + pair.Item2 != tuple.First)).First;
            return answer.ToString();
        }

        private long _part1Answer;

        [Part(2)]
        private string Part2(IPuzzleInput<long[]> input)
        {
            for (long result = 0, index = 0; index < input.Value.Length; index++, result = 0)
            {
                var range = input.Value[(int) index..].TakeWhile(l => (result += l) <= _part1Answer).ToArray();
                if (range.Sum() == _part1Answer)
                {
                    var answer = range.Min() + range.Max();
                    return answer.ToString();
                }
            }

            return null;
        }
    }
}