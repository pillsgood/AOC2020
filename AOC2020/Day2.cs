using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOC2020.Common;
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
                new PuzzleInput<IEnumerable<Password>>(provider, Parse).Value);
        }

        private static IEnumerable<Password> Parse(string input)
        {
            var pattern = new Regex(@"(?<Start>\d+)-(?<End>\d+) (?<Char>\w): (?<Password>\w+)");
            return pattern.Matches(input).Select(match =>
                new Password(
                    new Policy(
                        new RangeInt(int.Parse(match.Groups["Start"].Value), int.Parse(match.Groups["End"].Value)),
                        match.Groups["Char"].Value[0]),
                    match.Groups["Password"].Value));
        }

        private record Password(Policy Policy, string Value);

        private record Policy(RangeInt Range, char Character);

        [Part(1)]
        private string Part1(IEnumerable<Password> passwords)
        {
            var validCounts = passwords.Count(password =>
            {
                var charCount = password.Value.Count(c => c.Equals(password.Policy.Character));
                return password.Policy.Range.Contains(charCount);
            });

            return validCounts.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<Password> passwords)
        {
            var validCounts = passwords.Count(password =>
            {
                var characters = new[]
                    { password.Value[password.Policy.Range.Start - 1], password.Value[password.Policy.Range.End - 1] };
                return characters.Count(c => c.Equals(password.Policy.Character)) == 1;
            });

            return validCounts.ToString();
        }
    }
}