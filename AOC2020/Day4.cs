using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day4 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPuzzleInput<IEnumerable<Dictionary<string, string>>>>(provider =>
                new PuzzleInput<IEnumerable<Dictionary<string, string>>>(provider, Parse));
        }

        private static readonly Regex PassportPattern = new(@".*?(?:\r?\n\r?\n|$)", RegexOptions.Singleline);
        private static readonly Regex FieldPattern = new(@"(?<Key>\w{3}):(?<Value>.*?)(?=\s|$)");

        private static IEnumerable<Dictionary<string, string>> Parse(string input)
        {
            return PassportPattern.Matches(input).Select(passportMatch => FieldPattern.Matches(passportMatch.Value)
                .Select(match => new KeyValuePair<string, string>(match.Groups["Key"].Value, match.Groups["Value"].Value))
                .ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        private readonly Dictionary<string, Regex> _passportFields = new()
        {
            ["byr"] = new Regex(@"^(?:19[2-9]\d|200[0-2])$"),
            ["iyr"] = new Regex(@"^20(?:1\d|20)$"),
            ["eyr"] = new Regex(@"^20(?:2\d|30)$"),
            ["hgt"] = new Regex(@"^1(?:[5-8][0-9]|9[0-3])cm$|^(?:59|6\d|7[0-6])in$"),
            ["hcl"] = new Regex(@"#[0-9,a-f]{6}"),
            ["ecl"] = new Regex(@"amb|blu|brn|gry|grn|hzl|oth"),
            ["pid"] = new Regex(@"^\d{9}$"),
            ["cid"] = new Regex(@".*")
        };

        [Part(1)]
        private string Part1(IPuzzleInput<IEnumerable<Dictionary<string, string>>> input)
        {
            var requiredFields = _passportFields.Keys.Where(s => s != "cid");
            var validCount = input.Value.Count(passport => requiredFields.All(passport.ContainsKey));
            var answer = validCount.ToString();
            return answer;
        }

        [Part(2)]
        private string Part2(IPuzzleInput<IEnumerable<Dictionary<string, string>>> input)
        {
            var requiredFields = _passportFields.Keys.Where(s => s != "cid");
            var validCount = input.Value.Count(passport =>
                requiredFields.All(passport.ContainsKey) &&
                passport.Keys.All(key => _passportFields[key].IsMatch(passport[key])));
            var answer = validCount.ToString();
            return answer;
        }
    }
}