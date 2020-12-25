using System;
using System.Collections.Generic;
using System.Linq;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day17 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<bool[][]>>(provider => new PuzzleInput<bool[][]>(provider, Parse));
            services.AddScoped<ActivityMap3D>();
            services.AddScoped<ActivityMap4D>();
        }

        private static bool[][] Parse(string input)
        {
            return input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Select(c => c == '#').ToArray())
                .ToArray();
        }

        private abstract record ActivityMap<T> : Map<T, bool>
        {
            protected ActivityMap(IPuzzleInput<bool[][]> input)
            {
                map = Enumerable.Range(0, input.Value.Length)
                    .SelectMany(j => Enumerable.Range(0, input.Value[j].Length)
                        .Select(i => new KeyValuePair<T, bool>(Construct(i, j), input.Value[j][i])))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            protected abstract T Construct(int i, int j);

            public abstract IEnumerable<T> GetNeighbors(T position);

            public override bool this[T key]
            {
                get => map.ContainsKey(key) && map[key];
                set => base[key] = value;
            }
        }

        private sealed record ActivityMap3D : ActivityMap<Vector3Int>
        {
            public ActivityMap3D(IPuzzleInput<bool[][]> input) : base(input)
            {
            }

            protected override Vector3Int Construct(int i, int j) => new(i, j, 0);

            public override IEnumerable<Vector3Int> GetNeighbors(Vector3Int position) =>
                Enumerable.Range(-1, 3).SelectMany(i =>
                        Enumerable.Range(-1, 3).SelectMany(j =>
                            Enumerable.Range(-1, 3).Select(k => position + new Vector3Int(i, j, k))))
                    .Where(v => v != position);
        }

        private sealed record ActivityMap4D : ActivityMap<Vector4Int>
        {
            public ActivityMap4D(IPuzzleInput<bool[][]> input) : base(input)
            {
            }

            protected override Vector4Int Construct(int i, int j) => new(i, j, 0, 0);

            public override IEnumerable<Vector4Int> GetNeighbors(Vector4Int position) =>
                Enumerable.Range(-1, 3).SelectMany(i =>
                        Enumerable.Range(-1, 3).SelectMany(j =>
                            Enumerable.Range(-1, 3).SelectMany(k =>
                                Enumerable.Range(-1, 3).Select(h => position + new Vector4Int(i, j, k, h)))))
                    .Where(v => v != position);
        }

        private static void Simulate<T>(ActivityMap<T> activityMap)
        {
            var currentState = activityMap with {Entries = activityMap};
            foreach (var position in currentState.Select(pair => pair.Key).SelectMany(currentState.GetNeighbors).Distinct()
                .Except(currentState.Select(pair => pair.Key)))
            {
                activityMap[position] = false;
            }

            foreach (var (position, state) in activityMap)
            {
                var activeNeighbors = currentState.GetNeighbors(position).Count(v => currentState[v]);
                activityMap[position] = state switch
                {
                    true when activeNeighbors is not (2 or 3) => false,
                    false when activeNeighbors is 3 => true,
                    _ => state
                };
            }
        }

        [Part(1)]
        private string Part1(ActivityMap3D activityMap)
        {
            for (int i = 0; i < 6; i++)
            {
                Simulate(activityMap);
            }

            var answer = activityMap.Count(pair => pair.Value);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(ActivityMap4D activityMap)
        {
            for (int i = 0; i < 6; i++)
            {
                Simulate(activityMap);
            }

            var answer = activityMap.Count(pair => pair.Value);
            return answer.ToString();
        }
    }
}