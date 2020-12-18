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
    public class Day18 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<IEnumerable<string>>(provider, Parse).Value);
        }

        private static IEnumerable<string> Parse(string input) => input.Split('\n',
            StringSplitOptions.RemoveEmptyEntries);

        private const string ElementPattern =
            @"(?<Element>\d+|\((?>\((?<DEPTH>)|\)(?<-DEPTH>)|[^()]+)*\)(?(DEPTH)(?!)))";

        private static readonly Regex ParenthesesPattern = new(@"\((.*)\)");

        private static readonly Regex OperationPattern =
            new($@"(?<Left>{ElementPattern})\s?(?<Operator>[\*,\+])\s?(?<Right>{ElementPattern})");

        private static readonly Regex AddOperationPattern =
            new($@"(?<!\()(?<Left>{ElementPattern})\s?(?<Operator>\+)\s?(?<Right>{ElementPattern})");

        private class OperationParser
        {
            private readonly bool _additionFirst;

            public OperationParser(bool additionFirst = false)
            {
                _additionFirst = additionFirst;
            }

            private long EvaluateElement(string element, out Match match) =>
                (match = ParenthesesPattern.Match(element)).Success
                    ? EvaluateExpression(match.Groups[1].Value)
                    : long.Parse(element);

            public long EvaluateExpression(string expression)
            {
                long value = 0;
                while (Match(expression, out var match))
                {
                    var left = EvaluateElement(match.Groups["Left"].Value, out _);
                    var right = EvaluateElement(match.Groups["Right"].Value, out _);
                    var @operator = match.Groups["Operator"].Value;
                    value = @operator switch
                    {
                        "+" => left + right,
                        "*" => left * right,
                        _ => throw new ArgumentException($"failed to evaluate '{@operator}' in \"{expression}\"")
                    };
                    expression = expression.ReplaceFirst(match.Value, value.ToString());
                }

                return value;
            }

            private bool Match(string expression, out Match match) =>
                (match = _additionFirst
                    ? !(match = AddOperationPattern.Match(expression)).Success
                        ? OperationPattern.Match(expression)
                        : match
                    : OperationPattern.Match(expression)).Success;
        }

        [Part(1)]
        private string Part1(IEnumerable<string> expressions)
        {
            var math = new OperationParser();
            var answer = expressions.Sum(math.EvaluateExpression);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(IEnumerable<string> expressions)
        {
            var math = new OperationParser(true);
            var answer = expressions.Sum(math.EvaluateExpression);
            return answer.ToString();
        }
    }
}