using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day25 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => new PuzzleInput<PublicKeys>(provider, Parse).Value);
        }

        private static PublicKeys Parse(string input)
        {
            var keys = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            return new PublicKeys(keys[0], keys[1]);
        }

        private record PublicKeys(long Card, long Door);

        private const long Divider = 20201227L;

        private static long Transform(long subjectNumber, long loopSize)
        {
            var key = 1L;
            for (var i = 0; i < loopSize; i++)
            {
                key *= subjectNumber;
                key %= Divider;
            }

            return key;
        }

        private static long FindSecretLoopSize(long publicKey, int subjectNumber)
        {
            var key = 1L;
            var loopSize = 0;
            while (key != publicKey)
            {
                loopSize++;
                key *= subjectNumber;
                key %= Divider;
            }

            return loopSize;
        }

        [Part(1)]
        private string Part1(PublicKeys keys)
        {
            var cardLoopSize = FindSecretLoopSize(keys.Card, 7);
            var doorLoopSize = FindSecretLoopSize(keys.Door, 7);
            var cardEncryptionKey = Transform(keys.Door, cardLoopSize);
            var doorEncryptionKey = Transform(keys.Card, doorLoopSize);
            return cardEncryptionKey == doorEncryptionKey ? cardEncryptionKey.ToString() : null;
        }
    }
}