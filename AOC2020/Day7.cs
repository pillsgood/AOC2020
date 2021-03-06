using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day7 : IPuzzle
    {
        private const string Match = "shiny gold";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<List<Bag>>(provider, Parse).Value);
            services.AddSingleton(provider => provider.GetRequiredService<List<Bag>>()
                .Find(bag => bag.color.Equals(Match, StringComparison.OrdinalIgnoreCase)));
        }

        private static List<Bag> Parse(string input)
        {
            var bags = new List<Bag>();
            foreach (var line in input.Split("\n", StringSplitOptions.RemoveEmptyEntries))
            {
                var bag = GetOrCreate(Regex.Match(line, @"(^.*?(?=\s?bags?))").Value);
                foreach (Match match in Regex.Matches(line, @"(\d).*?\s?(.*?(?=\s?bags?))"))
                {
                    var key = GetOrCreate(match.Groups[2].Value);
                    var value = int.Parse(match.Groups[1].Value);
                    bag.nestedBags.Add(key, value);
                }
            }

            return bags;

            Bag GetOrCreate(string bagColor)
            {
                var bag = bags.FirstOrDefault(b => b.color.Equals(bagColor, StringComparison.OrdinalIgnoreCase)) 
                          ?? new Bag(bagColor, bags);
                return bag;
            }
        }

        private class Bag
        {
            public readonly string color;
            public readonly Dictionary<Bag, int> nestedBags;

            public Bag(string color, ICollection<Bag> bags)
            {
                this.color = color;
                nestedBags = new Dictionary<Bag, int>();
                bags.Add(this);
            }
            
            public bool CanHold(Bag match) =>
                nestedBags.Keys.Any(b => b.Equals(match)) || nestedBags.Keys.Any(b => b.CanHold(match));

            public int RecursiveCount() =>
                nestedBags.Aggregate(0, (i, pair) => i + pair.Value + pair.Value * pair.Key.RecursiveCount());
        }

        [Part(1)]
        private string Part1(List<Bag> bags, Bag shinyGold)
        {
            var answer = bags.Count(bag => bag.CanHold(shinyGold));
            return answer.ToString();
        }


        [Part(2)]
        private string Part2(Bag shinyGold)
        {
            var answer = shinyGold.RecursiveCount();
            return answer.ToString();
        }
    }
}