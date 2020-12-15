using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public partial class Day2 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(provider =>
                new PuzzleInput<IEnumerable<Password>>(provider, Process).Value);
        }

        private static IEnumerable<Password> Process(string value)
        {
            var pattern = new Regex(@"(?<Start>\d+)-(?<End>\d+) (?<Char>\w): (?<Password>\w+)");
            return pattern.Matches(value).Select(match =>
                new Password(
                    new Policy(
                        int.Parse(match.Groups["Start"].Value),
                        int.Parse(match.Groups["End"].Value),
                        match.Groups["Char"].Value[0]),
                    match.Groups["Password"].Value));
        }

        private record Password(Policy Policy, string Value);

        private record Policy(int RangeStart, int RangeEnd, char Character);

        [Part(1)]
        private string Part1(IEnumerable<Password> passwords)
        {
            var validCounts = passwords.Count(password =>
            {
                var charCount = password.Value.Count(c => c.Equals(password.Policy.Character));
                return charCount >= password.Policy.RangeStart && charCount <= password.Policy.RangeEnd;
            });

            return validCounts.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<Password> passwords)
        {
            var validCounts = passwords.Count(password =>
            {
                var characters = new[]
                    { password.Value[password.Policy.RangeStart - 1], password.Value[password.Policy.RangeEnd - 1] };
                return characters.Count(c => c.Equals(password.Policy.Character)) == 1;
            });

            return validCounts.ToString();
        }
    }
}