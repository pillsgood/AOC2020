using System;
using System.Linq;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day3 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<char[][]>(provider, Parse).Value);
            services.AddScoped<TreeMap>();
        }

        private static char[][] Parse(string input)
        {
            return input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(line => line.ToArray())
                .ToArray();
        }

        private record TreeMap : Map<char>
        {
            public TreeMap(char[][] input) : base(input)
            {
            }

            public long TraverseMapAndCount(Vector2Int slope)
            {
                var treeCount = 0;
                var position = new Vector2Int(XRange.Min, YRange.Max) + slope;
                while (map.ContainsKey(position))
                {
                    var value = map[position];
                    if (value == '#')
                    {
                        treeCount++;
                    }

                    position += slope;
                    position.x = XRange.Wrap(position.x);
                }

                return treeCount;
            }
        }


        [Part(1)]
        private string Part1(TreeMap map)
        {
            var answer = map.TraverseMapAndCount(new Vector2Int(3, -1)).ToString();
            return answer;
        }

        [Part(2)]
        private string Part2(TreeMap map)
        {
            var slopes = new[]
            {
                new Vector2Int(1, -1),
                new Vector2Int(3, -1),
                new Vector2Int(5, -1),
                new Vector2Int(7, -1),
                new Vector2Int(1, -2)
            };

            var answer = slopes.Select(map.TraverseMapAndCount).Aggregate(1L, (i, j) => i * j);
            return answer.ToString();
        }
    }
}