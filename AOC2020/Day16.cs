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
    public class Day16 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new PuzzleInput<IEnumerable<Field>>(provider, ParseFields).Value);
            services.AddSingleton(provider => new PuzzleInput<int[]>(provider, ParsePlayerTicket).Value);
            services.AddSingleton(provider =>
                new PuzzleInput<IEnumerable<int[]>>(provider, ParseNearbyTickets).Value);
        }

        private static IEnumerable<Field> ParseFields(string input)
        {
            var namePattern = new Regex(@"^([\w,\s]+):");
            var rangePattern = new Regex(@"(?<Start>\d+)-(?<End>\d+)");
            return input.Split("your ticket").First().Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s =>
                new Field(namePattern.Match(s).Value,
                    rangePattern.Matches(s).Select(match =>
                        new RangeInt(int.Parse(match.Groups["Start"].Value), int.Parse(match.Groups["End"].Value)))));
        }

        private static int[] ParsePlayerTicket(string input)
        {
            var pattern = new Regex(@"your ticket:\n?([\d,\,]+)");
            return pattern.Match(input).Groups[1].Value.Split(',').Select(int.Parse).ToArray();
        }

        private static IEnumerable<int[]> ParseNearbyTickets(string input)
        {
            var pattern = new Regex(@"nearby tickets:\n?([\d,\,,\n?]+)");
            return pattern.Match(input).Groups[1].Value.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(',').Select(int.Parse).ToArray());
        }

        private record Field(string Name, IEnumerable<RangeInt> Ranges);

        [Part(1)]
        private string Part1(IEnumerable<Field> fields, IEnumerable<int[]> nearbyTickets)
        {
            var invalids = nearbyTickets.SelectMany(ints => ints.Select(i => i)).Where(i =>
                fields.SelectMany(field => field.Ranges.Select(rangeInt => rangeInt))
                    .All(rangeInt => !rangeInt.Contains(i)));
            var answer = invalids.Sum();
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<Field> fields, IEnumerable<int[]> nearbyTickets, int[] playerTicket)
        {
            var validTickets = nearbyTickets.Where(ticket =>
                    ticket.All(i => fields.SelectMany(field => field.Ranges.Select(r => r)).Any(r => r.Contains(i))))
                .Append(playerTicket).ToArray();
            var possibleFieldIndices = fields.ToDictionary(field => field,
                field => Enumerable.Range(0, playerTicket.Length)
                    .Where(idx => validTickets.Select(ints => ints[idx])
                        .All(i => field.Ranges.Any(rangeInt => rangeInt.Contains(i)))).ToList());

            var definiteFieldIndices = new Dictionary<Field, int>();
            while (possibleFieldIndices.Values.Any(ints => ints.Count != 1))
            {
                var field = possibleFieldIndices.Keys.First(key => possibleFieldIndices[key].Count == 1);
                definiteFieldIndices[field] = possibleFieldIndices[field][0];
                possibleFieldIndices.Remove(field);
                _ = possibleFieldIndices.Values.All(ints => ints.Remove(definiteFieldIndices[field]));
            }

            var answer = definiteFieldIndices
                .Where(pair => pair.Key.Name.StartsWith("departure", StringComparison.OrdinalIgnoreCase))
                .Select(pair => playerTicket[pair.Value]).Aggregate(1L, (i, j) => i * j);
            return answer.ToString();
        }
    }
}