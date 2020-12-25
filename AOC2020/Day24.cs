using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day24 : IPuzzle
    {
        private static readonly Regex DirectionPattern = new(@"ne|e|se|sw|w|nw");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<Direction[][]>(provider, Parse).Value);
            services.AddSingleton<HexMap>();
        }

        private static Direction[][] Parse(string input) =>
            input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => DirectionPattern.Matches(s)
                    .Select(match => Enum.TryParse<Direction>(match.Value.ToUpperInvariant(), out var result)
                        ? result
                        : throw new ArgumentException($"failed to parse {match.Value} as direction"))
                    .ToArray())
                .ToArray();

        private static IEnumerable<Direction> HexDirections
        {
            get
            {
                yield return Direction.NE;
                yield return Direction.E;
                yield return Direction.SE;
                yield return Direction.SW;
                yield return Direction.W;
                yield return Direction.NW;
            }
        }

        private sealed record HexMap : Map<bool>
        {
            public override bool this[Vector2Int key]
            {
                get => map.ContainsKey(key) && map[key];
                set => base[key] = value;
            }

            public IEnumerable<Vector2Int> GetNeighborPositions(Vector2Int position) => HexDirections.Select(direction => GetNeighborPosition(position, direction));

            private Vector2Int GetNeighborPosition(Vector2Int position, Direction direction) =>
                direction switch
                {
                    Direction.NE => position + new Vector2Int(0, 1),
                    Direction.E => position + new Vector2Int(1, 0),
                    Direction.SE => position + new Vector2Int(1, -1),
                    Direction.SW => position + new Vector2Int(0, -1),
                    Direction.W => position + new Vector2Int(-1, 0),
                    Direction.NW => position + new Vector2Int(-1, 1),
                    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                };

            public Vector2Int GetPositionFromPath(Vector2Int position, IEnumerable<Direction> path) =>
                path.Aggregate(position, GetNeighborPosition);

            public void Flip(Vector2Int position) => map[position] = !this[position];
        }

        private static void Simulate(HexMap map)
        {
            var currentState = map with {Entries = map};

            foreach (var position in currentState.Select(pair => pair.Key).SelectMany(currentState.GetNeighborPositions).Distinct()
                .Except(currentState.Select(pair => pair.Key)))
            {
                map[position] = false;
            }

            foreach (var (position, state) in map)
            {
                var activeNeighbors = currentState.GetNeighborPositions(position).Count(v => currentState[v]);
                map[position] = state switch
                {
                    true when activeNeighbors is (0 or >2) => false,
                    false when activeNeighbors is 2 => true,
                    _ => state
                };
            }
        }

        [Part(1)]
        private string Part1(Direction[][] input, HexMap map)
        {
            foreach (var path in input)
            {
                var position = map.GetPositionFromPath(Vector2Int.zero, path);
                map.Flip(position);
            }

            var answer = map.Count(pair => pair.Value);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(HexMap map)
        {
            for (var i = 0; i < 100; i++)
            {
                Simulate(map);
            }

            var answer = map.Count(pair => pair.Value);
            return answer.ToString();
        }
    }
}