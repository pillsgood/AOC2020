using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AngleSharp.Common;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day23 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<int[]>(provider, Parse).Value);
        }

        private static int[] Parse(string input) => input.Trim().ToCharArray().Select(c => c.ToString()).Select(int.Parse).ToArray();

        [Part(1)]
        private string Part1(int[] cups)
        {
            var queue = new Queue<int>(cups);
            for (var round = 0; round < 100; round++)
            {
                var current = queue.Dequeue();
                var pickup = Enumerable.Range(0, 3).Select(_ => queue.Dequeue()).ToArray();
                var destination = queue.OrderBy(i => Mathf.Mod(current - i, cups.Max())).First();
                var prepend = queue.TakeWhile(i => i != destination).Append(destination).ToArray();
                queue = new Queue<int>(prepend.Concat(pickup).Concat(queue.Skip(prepend.Length)).Append(current));
            }

            while (queue.Peek() != 1) queue.Enqueue(queue.Dequeue());

            queue.Dequeue();
            var answer = string.Join("", queue);
            return answer;
        }

        private record Node(int Value, Node Next)
        {
            public Node Next { get; set; } = Next;
        }

        [Part(2)]
        private string Part2(int[] cups)
        {
            const int max = 1000000;
            const int maxRound = 10000000;
            var map = new Node[max];
            var first = cups
                .Concat(Enumerable.Range(cups.Max() + 1, max - cups.Length))
                .Reverse()
                .Aggregate<int, Node>(null, (next, val) => map[val - 1] = new Node(val, next));
            var current = map[^1].Next = first;
            for (var round = 0; round < maxRound; round++, current = current.Next)
            {
                long destination = current.Value - 1;
                var pickup = new[] { current.Next, current.Next.Next, current.Next.Next.Next };
                while (pickup.Any(n => n.Value == destination) || destination == 0) destination = Mathf.Mod(destination - 1, max + 1);
                current.Next = pickup[^1].Next;
                pickup[^1].Next = map[destination - 1].Next;
                map[destination - 1].Next = pickup[0];
            }

            var (_, (value, node)) = map[0];
            var answer = (long) value * node.Value;
            return answer.ToString();
        }
    }
}