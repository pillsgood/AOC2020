using System;
using System.Collections;
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
            services.AddScoped<Map3D>();
            services.AddScoped<Map4D>();
        }

        private static bool[][] Parse(string input)
        {
            return input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Select(c => c == '#').ToArray())
                .ToArray();
        }

        private abstract record Map<T> : IEnumerable<T>
        {
            private readonly Dictionary<T, bool> _activity = new();

            protected Map(IPuzzleInput<bool[][]> input)
            {
                _activity = Enumerable.Range(0, input.Value.Length)
                    .SelectMany(j => Enumerable.Range(0, input.Value[j].Length)
                        .Select(i => new KeyValuePair<T, bool>(Construct(i, j), input.Value[j][i])))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            public IDictionary<T, bool> Activity
            {
                get => _activity;
                init => _activity = new Dictionary<T, bool>(value);
            }

            public IEnumerator<T> GetEnumerator() => _activity.Keys.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            protected abstract T Construct(int i, int j);

            public abstract IEnumerable<T> GetNeighbors(T position);

            public bool GetActive(T v) => _activity.ContainsKey(v) && _activity[v];
            public void SetActive(T v, bool active) => _activity[v] = active;
        }

        private sealed record Map3D : Map<Vector3Int>
        {
            public Map3D(IPuzzleInput<bool[][]> input) : base(input)
            {
            }

            protected override Vector3Int Construct(int i, int j) => new(i, j, 0);

            public override IEnumerable<Vector3Int> GetNeighbors(Vector3Int position) =>
                Enumerable.Range(-1, 3).SelectMany(i =>
                        Enumerable.Range(-1, 3).SelectMany(j =>
                            Enumerable.Range(-1, 3).Select(k => position + new Vector3Int(i, j, k))))
                    .Where(v => v != position);
        }

        private sealed record Map4D : Map<Vector4Int>
        {
            public Map4D(IPuzzleInput<bool[][]> input) : base(input)
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

        private void Simulate<T>(Map<T> map)
        {
            var currentState = map with {Activity = map.Activity};
            foreach (var neighbor in currentState.SelectMany(currentState.GetNeighbors).Distinct().Except(currentState))
            {
                map.SetActive(neighbor, false);
            }

            foreach (var position in map)
            {
                var state = currentState.GetActive(position);
                var activeNeighbors = currentState.GetNeighbors(position).Where(currentState.GetActive).ToArray();
                map.SetActive(position, state switch
                {
                    true when activeNeighbors.Length is not (2 or 3) => false,
                    false when activeNeighbors.Length is 3 => true,
                    _ => state
                });
            }
        }

        [Part(1)]
        private string Part1(Map3D map)
        {
            for (int i = 0; i < 6; i++)
            {
                Simulate(map);
            }

            var answer = map.Count(map.GetActive);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(Map4D map)
        {
            for (int i = 0; i < 6; i++)
            {
                Simulate(map);
            }

            var answer = map.Count(map.GetActive);
            return answer.ToString();
        }
    }
}