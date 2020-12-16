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
            services.AddSingleton(provider => new PuzzleInput<IEnumerable<Seat>>(provider, Parse).Value);
        }

        private static IEnumerable<Seat> Parse(string input)
        {
            return input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Select(line => new Seat(line));
        }

        private class Seat
        {
            public Seat(string partition)
            {
                var x = Convert.ToInt32(partition[^3..].Replace('L', '0').Replace('R', '1'), 2);
                var y = Convert.ToInt32(partition[..7].Replace('F', '0').Replace('B', '1'), 2);

                SeatId = y * 8 + x;
            }

            public int SeatId { get; }
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
            var seatIds = seats.Select(seat => seat.SeatId).OrderBy(i => i).ToArray();
            var answer = seatIds.FirstOrDefault(i =>
                !seatIds.Contains(i) && seatIds.Contains(i + 1) && seatIds.Contains(i - 1));
            return answer.ToString();
        }
    }
}