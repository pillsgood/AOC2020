using System;
using System.Collections.Generic;
using System.Linq;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day11 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new PuzzleInput<SeatState[][]>(provider, Parse).Value);
            services.AddScoped<SeatMap>();
        }

        private static SeatState[][] Parse(string input)
        {
            return input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Select(s => s.Select(c => c switch
                {
                    'L' => SeatState.Unoccupied,
                    '#' => SeatState.Occupied,
                    '.' => SeatState.Floor,
                    _ => throw new Exception("Failed to parse seat state")
                }).ToArray()).ToArray();
        }

        private static bool Simulate(SeatMap map,
            Func<Vector2Int, IEnumerable<Vector2Int>> consideredSeatsImplementation, int occupyTolerance)
        {
            var currentState = map with {Entries = map};
            foreach (var (seatIndex, state) in currentState)
            {
                var neighbors = consideredSeatsImplementation.Invoke(seatIndex);
                map.SetSeatState(seatIndex, state switch
                {
                    SeatState.Occupied
                        when neighbors.Count(position =>
                            currentState[position] == SeatState.Occupied) >= occupyTolerance
                        => SeatState.Unoccupied,
                    SeatState.Unoccupied
                        when neighbors.All(position =>
                            currentState[position] == SeatState.Unoccupied)
                        => SeatState.Occupied,
                    _ => state
                });
            }

            return map.Equals(currentState) || Simulate(map, consideredSeatsImplementation, occupyTolerance);
        }

        private enum SeatState
        {
            Floor,
            Occupied,
            Unoccupied
        }

        private sealed record SeatMap : Map<SeatState>
        {
            public SeatMap(SeatState[][] input) : base(input)
            {
            }

            public int CountOccupied => map.Values.Count(state => state == SeatState.Occupied);


            public void SetSeatState(Vector2Int position, SeatState? state) => map[position] =
                state != SeatState.Floor && state.HasValue ? state.Value : map[position];

            public IEnumerable<Vector2Int> GetNeighborSeatIndices(Vector2Int position) =>
                Enumerable.Range(position.x - 1, 3)
                    .SelectMany(x => Enumerable.Range(position.y - 1, 3).Select(y => new Vector2Int(x, y)))
                    .Where(v => position != v && map.ContainsKey(v))
                    .Where(v => map[v] != SeatState.Floor);

            public IEnumerable<Vector2Int> GetClosestSeatIndices(Vector2Int position) =>
                Enum.GetValues(typeof(Direction)).Cast<Direction>()
                    .Select(direction => GetFirstSeatInDirection(position, direction))
                    .Where(v => v.HasValue).Cast<Vector2Int>();

            private Vector2Int? GetFirstSeatInDirection(Vector2Int position, Direction direction)
            {
                while (map.TryGetValue(position += direction.GetStep(), out var state))
                {
                    if (state != SeatState.Floor)
                    {
                        return position;
                    }
                }

                return null;
            }
        }

        [Part(1)]
        private string Part1(SeatMap map)
        {
            Simulate(map, map.GetNeighborSeatIndices, 4);
            var answer = map.CountOccupied;
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(SeatMap map)
        {
            Simulate(map, map.GetClosestSeatIndices, 5);
            var answer = map.CountOccupied;
            return answer.ToString();
        }
    }
}