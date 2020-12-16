using System;
using System.Collections.Generic;
using System.Linq;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day12 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new PuzzleInput<IEnumerable<KeyValuePair<char, int>>>(provider, Parse).Value);
        }

        private static IEnumerable<KeyValuePair<char, int>> Parse(string input) =>
            input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new KeyValuePair<char, int>(s[0], int.Parse(new string(s[1..]))));

        [Part(1)]
        private string Part1(IEnumerable<KeyValuePair<char, int>> input)
        {
            var position = new Vector2Int(0, 0);
            var currentDirection = Direction.E.GetStep();

            foreach (var (key, value) in input)
            {
                switch (key)
                {
                    case 'F':
                        position += currentDirection * value;
                        break;
                    case 'L':
                        currentDirection = currentDirection.RotateBy(value);
                        break;
                    case 'R':
                        currentDirection = currentDirection.RotateBy(-value);
                        break;
                    default:
                        var direction = Enum.Parse<Direction>(key.ToString(), true);
                        position += direction.GetStep() * value;
                        break;
                }
            }

            position = position.Abs();
            var answer = position.x + position.y;
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<KeyValuePair<char, int>> input)
        {
            var position = new Vector2Int(0, 0);
            var waypoint = new Vector2Int(10, 1);
            foreach (var (key, value) in input)
            {
                switch (key)
                {
                    case 'F':
                        position += waypoint * value;
                        break;
                    case 'L':
                        waypoint = waypoint.RotateBy(value);
                        break;
                    case 'R':
                        waypoint = waypoint.RotateBy(-value);
                        break;
                    default:
                    {
                        var direction = Enum.Parse<Direction>(key.ToString(), true);
                        waypoint += direction.GetStep() * value;
                        break;
                    }
                }
            }

            position = position.Abs();
            var answer = position.x + position.y;
            return answer.ToString();
        }
    }
}