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
    public class Day14 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
                new PuzzleInput<Dictionary<char[], Dictionary<long, long>>>(provider, Process).Value);
        }

        private static Dictionary<char[], Dictionary<long, long>> Process(string value)
        {
            var maskPattern = new Regex(@"mask.*?(?<Mask>[\d,X]+)");
            var valuePattern = new Regex(@"mem\[(?<Address>\d+)\].*?(?<Value>\d+)");
            var masks = new Dictionary<char[], Dictionary<long, long>>();
            var input = maskPattern.Split(value.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            for (var i = 0; i < input.Length; i++)
            {
                masks.Add(input[i].ToCharArray(), valuePattern.Matches(input[++i])
                    .ToDictionary(match => long.Parse(match.Groups["Address"].Value),
                        match => long.Parse(match.Groups["Value"].Value)));
            }

            return masks;
        }

        private static char[] ToBits(long value, int length)
        {
            var bits = new BitArray(BitConverter.GetBytes(value));
            return Enumerable.Range(0, length)
                .Select(i => i < bits.Length && bits[i] ? '1' : '0')
                .Reverse()
                .ToArray();
        }

        private static long ToLong(char[] value)
        {
            var bytes = value.Select(c => c == '1').Reverse().ToArray();
            var array = new int[2];
            var bitArray = new BitArray(bytes);
            bitArray.CopyTo(array, 0);
            return (uint) array[0] + ((long) (uint) array[1] << 32);
        }

        [Part(1)]
        private string Part1(Dictionary<char[], Dictionary<long, long>> input)
        {
            var memory = new Dictionary<long, long>();
            foreach (var (mask, values) in input)
            foreach (var (address, value) in values)
            {
                memory[address] = ToLong(ToBits(value, 36).Select((c, i) => mask[i] == 'X' ? c : mask[i]).ToArray());
            }

            var answer = memory.Values.Sum();
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(Dictionary<char[], Dictionary<long, long>> input)
        {
            var memory = new Dictionary<long, long>();
            foreach (var (mask, values) in input)
            foreach (var (addressValue, value) in values)
            {
                var floatingAddress = ToBits(addressValue, 36).Select((c, i) => mask[i] != '0' ? mask[i] : c).ToArray();
                var floatingCount = floatingAddress.Count(c => c == 'X');
                foreach (var address in Enumerable.Range(0, (int) Math.Pow(2, floatingCount)).Select(i =>
                {
                    var variation = ToBits(i, floatingCount);
                    var idx = 0;
                    return ToLong(floatingAddress.Select(c => c == 'X' ? variation[idx++] : c).ToArray());
                })) memory[address] = value;
            }

            var answer = memory.Values.Sum();
            return answer.ToString();
        }
    }
}