using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day5 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<IEnumerable<Seat>>(provider, Process).Value);
        }

        private static IEnumerable<Seat> Process(string value)
        {
            return value.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Select(line => new Seat(line));
        }

        private class Seat
        {
            private readonly int _seatId;

            public Seat(string partition)
            {
                var x = Convert.ToInt32(partition[^3..].Replace('L', '0').Replace('R', '1'), 2);
                var y = Convert.ToInt32(partition[..7].Replace('F', '0').Replace('B', '1'), 2);

                _seatId = y * 8 + x;
            }

            public int SeatId => _seatId;
        }


        [Part(1)]
        private string Part1(IEnumerable<Seat> seats)
        {
            var answer = seats.Max(seat => seat.SeatId);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<Seat> seats)
        {
            var sorted = seats.Select(seat => seat.SeatId).ToList();
            sorted.Sort();
            var foundSeat = new List<int>();
            for (var i = sorted.First(); i < sorted.Last(); i++)
            {
                if (!sorted.Contains(i) && sorted.Contains(i + 1) && sorted.Contains(i - 1))
                {
                    foundSeat.Add(i);
                }
            }

            if (foundSeat.Count == 1)
            {
                var answer = foundSeat[0];
                return answer.ToString();
            }

            return null;
        }
    }
}