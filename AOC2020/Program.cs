using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode;
using Pillsgood.AdventOfCode.Client;
using Pillsgood.AdventOfCode.Console;

namespace AOC2020
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Environment.ExitCode = 1;

            var aoc = AdventOfCode.Build(config => config
                .ConfigureServices(ConfigureServices)
                .LoadCallingAssembly()
                .AddConsole()
                .AddClient());

            // adding console will force aggregate results of puzzles, otherwise iterate through the return value of runner.Run()
            aoc.Run();

            // foreach (var (puzzle, results) in aoc.Run())
            // foreach (var result in results)
            // {
            // }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
        }
    }
}