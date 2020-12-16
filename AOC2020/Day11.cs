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
            services.AddScoped<SeatMap>();
        }

        private enum SeatState
        {
            Floor,
            Occupied,
            Unoccupied
        }

        private sealed record SeatMap
        {
            private readonly SeatState[][] _map;

            private readonly int _width;

            private readonly int _height;

            public SeatMap(IPuzzleInput input)
            {
                _map = input.Value.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                    .Select(s => s.Select(c => c switch
                    {
                        'L' => SeatState.Unoccupied,
                        '#' => SeatState.Occupied,
                        '.' => SeatState.Floor,
                        _ => throw new Exception("Failed to parse seat state")
                    }).ToArray()).ToArray();
                _height = _map.Length;
                _width = _map[0].Length;
                if (_map.Any(states => states.Length != _width))
                    throw new Exception("width is not the same across all rows");
            }

            public SeatMap From
            {
                init => _map = value._map.Select(states => states.ToArray()).ToArray();
            }

            public SeatState? GetSeatState(Vector2Int position) =>
                position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height
                    ? _map[position.y][position.x]
                    : (SeatState?) null;

            public void SetSeatState(Vector2Int position, SeatState? state) => _map[position.y][position.x] =
                state != SeatState.Floor && state.HasValue ? state.Value : _map[position.y][position.x];

            public int CountOccupied => _map.Sum(states => states.Count(state => state == SeatState.Occupied));

            public IEnumerable<Vector2Int> GetNeighborSeatIndices(Vector2Int position) =>
                Enumerable.Range(position.x - 1, 3)
                    .SelectMany(x => Enumerable.Range(position.y - 1, 3).Select(y => new Vector2Int(x, y)))
                    .Where(v => position != v && v.x >= 0 && v.x < _width && v.y >= 0 && v.y < _height)
                    .Where(v => GetSeatState(v) != SeatState.Floor);

            public IEnumerable<Vector2Int> GetClosestSeatIndices(Vector2Int position) =>
                Enum.GetValues(typeof(Direction)).Cast<Direction>()
                    .Select(direction => GetFirstSeatInDirection(position, direction))
                    .Where(v => v.HasValue).Cast<Vector2Int>();

            public IEnumerable<Vector2Int> GetAllSeatIndices() =>
                Enumerable.Range(0, _width).SelectMany(x => Enumerable.Range(0, _height)
                    .Select(y => new Vector2Int(x, y))).Where(v => GetSeatState(v) != SeatState.Floor);

            private Vector2Int? GetFirstSeatInDirection(Vector2Int position, Direction direction)
            {
                SeatState? state;
                do
                {
                    position += direction.GetStep();
                    state = GetSeatState(position);
                } while (state.HasValue && state.Value == SeatState.Floor);

                return state.HasValue ? position : (Vector2Int?) null;
            }

            public bool Equals(SeatMap other) => !ReferenceEquals(null, other) && (ReferenceEquals(this, other) ||
                _map.Select((states, i) => states.SequenceEqual(other._map[i])).All(b => b));

            public override int GetHashCode() => _map != null ? _map.GetHashCode() : 0;
        }

        private static bool Simulate(SeatMap map,
            Func<Vector2Int, IEnumerable<Vector2Int>> consideredSeatsImplementation, int occupyTolerance)
        {
            var currentState = map with {From = map};
            foreach (var seatIndex in currentState.GetAllSeatIndices())
            {
                var state = currentState.GetSeatState(seatIndex);
                var neighbors = consideredSeatsImplementation.Invoke(seatIndex);
                map.SetSeatState(seatIndex, state switch
                {
                    SeatState.Occupied
                        when neighbors.Count(position =>
                            currentState.GetSeatState(position) == SeatState.Occupied) >= occupyTolerance
                        => SeatState.Unoccupied,
                    SeatState.Unoccupied
                        when neighbors.All(position =>
                            currentState.GetSeatState(position) == SeatState.Unoccupied)
                        => SeatState.Occupied,
                    _ => state
                });
            }

            return map.Equals(currentState) || Simulate(map, consideredSeatsImplementation, occupyTolerance);
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