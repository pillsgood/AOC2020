using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Text;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day21 : IPuzzle
    {
        private static readonly Regex RecipePattern = new(@"(?<Ingredients>.*?) \(contains (?<Allergens>.*?)\)");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<Recipe[]>(provider, Parse).Value);
        }

        private static Recipe[] Parse(string input) =>
            RecipePattern.Matches(input).Select(match =>
                new Recipe(match.Groups["Ingredients"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    match.Groups["Allergens"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))).ToArray();

        private record Recipe(string[] Ingredients, string[] Allergens);

        private static Dictionary<string, List<string>> FindCommonIngredients(Recipe[] recipes)
        {
            return recipes.SelectMany(recipe => recipe.Allergens).Distinct().ToDictionary(allergen => allergen, allergen =>
            {
                var contains = recipes.Where(recipe => recipe.Allergens.Contains(allergen)).Select(recipe => recipe.Ingredients).ToArray();
                return contains.Skip(1).Aggregate(contains.First().ToList(), (commons, ingredients) => commons.Intersect(ingredients).ToList());
            });
        }

        private static Dictionary<string, string> GetAllergenIngredients(Recipe[] recipes)
        {
            var allergens = recipes.SelectMany(recipe => recipe.Allergens).Distinct().ToDictionary(s => s, _ => string.Empty);
            var possibleAllergenIngredients = FindCommonIngredients(recipes);
            while (allergens.Any(pair => string.IsNullOrEmpty(pair.Value)))
            {
                var (allergen, ingredients) = possibleAllergenIngredients.First(pair => pair.Value.Count == 1);
                allergens[allergen] = ingredients[0];
                possibleAllergenIngredients.Remove(allergen);
                foreach (var (_, value) in possibleAllergenIngredients.Where(allergenIngredient => allergenIngredient.Value.Contains(allergens[allergen])))
                {
                    value.Remove(allergens[allergen]);
                }
            }

            return allergens;
        }

        [Part(1)]
        private string Part1(Recipe[] recipes)
        {
            var allergens = GetAllergenIngredients(recipes);
            var answer = recipes.SelectMany(recipe => recipe.Ingredients).Count(s => !allergens.Values.Contains(s));
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(Recipe[] recipes)
        {
            var allergens = GetAllergenIngredients(recipes);
            var answer = string.Join(',', allergens.OrderBy(pair => pair.Key).Select(pair => pair.Value));
            return answer;
        }
    }
}