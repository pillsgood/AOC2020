using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day19 : IPuzzle
    {
        private static readonly Regex RulePattern = new(@"(?<Id>\d+):\s""?(?<Content>[^""\n]+)");
        private static readonly Regex MessagePattern = new(@"(?<!"")[a-z]+(?!"")");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(provider =>
                Rule.FactoryMethod(new PuzzleInput<Dictionary<int, string>>(provider, ParseRules).Value));
            services.AddSingleton(provider => new PuzzleInput<string[]>(provider, ParseMessages).Value);
        }

        private static string[] ParseMessages(string input) =>
            MessagePattern.Matches(input).Select(match => match.Value).ToArray();

        private static Dictionary<int, string> ParseRules(string input) =>
            RulePattern.Matches(input).ToDictionary(match => int.Parse(match.Groups["Id"].Value),
                match => match.Groups["Content"].Value);

        private record Rule : IEnumerable<Rule[]>
        {
            private readonly Dictionary<int, Rule> _rules;

            private readonly int _id;
            private readonly char? _value = null;
            private readonly int[][] _subRules;
            public bool HasValue => _value.HasValue;
            public char Value => _value ?? throw new ArgumentException();

            public string Content
            {
                init => ParseContent(value, out _value, out _subRules);
            }

            public override string ToString() => $"Rule: {_id}";

            public static Dictionary<int, Rule> FactoryMethod(Dictionary<int, string> inputRules)
            {
                var dict = new Dictionary<int, Rule>();
                foreach (var (key, value) in inputRules)
                {
                    dict.Add(key, new Rule(key, value, dict));
                }

                return dict;
            }

            private Rule(int id, string content, Dictionary<int, Rule> rules)
            {
                _id = id;
                _rules = rules;
                ParseContent(content, out _value, out _subRules);
            }

            private static void ParseContent(string content, out char? value, out int[][] subRules)
            {
                var isLetter = char.IsLetter(content[0]);
                value = isLetter ? content[0] : null;
                subRules = isLetter
                    ? null
                    : content.Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                        .ToArray();
            }

            public IEnumerator<Rule[]> GetEnumerator() =>
                _subRules.Select(rules => rules.Select(i => _rules[i]).ToArray()).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _subRules.GetEnumerator();
        }

        private static bool Validate(string message, Queue<Rule> queue)
        {
            if (queue.Count > message.Length)
                return false;

            if (message.Length == 0 || queue.Count == 0)
                return message.Length == 0 && queue.Count == 0;

            var rule = queue.Dequeue();
            return rule.HasValue
                ? message[0] == rule.Value && Validate(message[1..], new Queue<Rule>(queue))
                : rule.Any(rules => Validate(message, new Queue<Rule>(rules.Concat(queue))));
        }

        [Part(1)]
        private string Part1(Dictionary<int, Rule> rules, string[] messages)
        {
            var answer = messages.Count(message =>
                Validate(message, new Queue<Rule>(rules[0].First())));
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(Dictionary<int, Rule> rules, string[] messages)
        {
            rules[8] = rules[8] with {Content = "42 | 42 8"};
            rules[11] = rules[11] with {Content = "42 31 | 42 11 31"};
            var answer = messages.Count(message =>
                Validate(message, new Queue<Rule>(rules[0].First())));
            return answer.ToString();
        }
    }
}