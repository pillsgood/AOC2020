using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day22 : IPuzzle
    {
        private static readonly Regex DeckPattern = new(@"Player (?<ID>\d):\r?\n?(?<Cards>(?:\d+\r?\n?)+)");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(provider => new PuzzleInput<Player[]>(provider, Parse).Value);
            services.AddScoped<CombatGame>();
            services.AddScoped<RecursiveCombatGame>();
        }

        private static Player[] Parse(string input) =>
            DeckPattern.Matches(input).Select(match => new Player(int.Parse(match.Groups["ID"].Value), new Deck(match.Groups["Cards"].Value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse).ToArray()))).ToArray();

        private record Player(int Id, Deck Deck)
        {
            public string Serialize() => $"{Id}: {Deck}";
        }

        private readonly struct Deck : IEquatable<Deck>
        {
            private readonly Queue<int> _cards;
            public Deck(IEnumerable<int> input) => _cards = new Queue<int>(input);

            public Deck(Deck deck, int count) : this(deck._cards.Take(count))
            {
            }

            public override string ToString() => string.Join(", ", _cards);

            public int Count => _cards.Count;
            public int Draw() => _cards.Dequeue();

            public void Take(IEnumerable<int> cards)
            {
                foreach (var card in cards) _cards.Enqueue(card);
            }

            public bool CanDraw => _cards.Count > 0;

            public long CalculateScore()
            {
                var score = 0;
                for (var i = _cards.Count - 1; i >= 0; i--) score += (i + 1) * _cards.Dequeue();
                return score;
            }

            public bool Equals(Deck other) => Equals(_cards, other._cards) || _cards.SequenceEqual(other._cards);
        }

        private class CombatGame
        {
            protected readonly Player[] Players;
            public CombatGame(Player[] players) => Players = players;

            public virtual void Play(out Player winner)
            {
                while (Players.All(player => player.Deck.CanDraw))
                {
                    var playerMoves = Players.Select<Player, (Player player, int card)>(player => (player, player.Deck.Draw()))
                        .OrderByDescending(tuple => tuple.card)
                        .ToArray();
                    playerMoves[0].player.Deck.Take(playerMoves.Select(tuple => tuple.card));
                }

                winner = Players.First(player => player.Deck.CanDraw);
            }
        }

        private class RecursiveCombatGame : CombatGame
        {
            private readonly HashSet<string> _previousDecks = new();

            public RecursiveCombatGame(Player[] players) : base(players)
            {
            }

            private static (Player player, int card)[] PlaySubGame((Player player, int card)[] playerMoves)
            {
                var game = new RecursiveCombatGame(playerMoves.Select(tuple => tuple.player with {Deck = new Deck(tuple.player.Deck, tuple.card)}).ToArray());
                game.Play(out var winner);
                return playerMoves.OrderByDescending(tuple => tuple.player.Id == winner.Id).ToArray();
            }

            public override void Play(out Player winner)
            {
                while (Players.All(player => player.Deck.CanDraw))
                {
                    var state = string.Join("/", Players.Select(player => player.Serialize()));
                    if (_previousDecks.Contains(state))
                    {
                        winner = Players.First(player => player.Id == 1);
                        return;
                    }

                    _previousDecks.Add(state);
                    var playerMoves = Players.Select<Player, (Player player, int card)>(player => (player, player.Deck.Draw())).ToArray();
                    playerMoves = playerMoves.All(tuple => tuple.player.Deck.Count >= tuple.card)
                        ? PlaySubGame(playerMoves)
                        : playerMoves.OrderByDescending(tuple => tuple.card).ToArray();
                    playerMoves[0].player.Deck.Take(playerMoves.Select(tuple => tuple.card));
                }

                winner = Players.First(player => player.Deck.CanDraw);
            }
        }

        [Part(1)]
        private string Part1(CombatGame game)
        {
            game.Play(out var winner);
            var answer = winner.Deck.CalculateScore();
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(RecursiveCombatGame game)
        {
            game.Play(out var winner);
            var answer = winner.Deck.CalculateScore();
            return answer.ToString();
        }
    }
}