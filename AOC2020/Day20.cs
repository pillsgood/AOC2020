using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AOC2020.Common;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day20 : IPuzzle
    {
        private static readonly Regex TilePattern =
            new(@"Tile (?<Id>\d+):\n(?<MapInput>(?:[#,\.]+\n)+)");

        private static readonly string[] MonsterPattern =
        {
            "..................#.",
            "#....##....##....###",
            ".#..#..#..#..#..#...",
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(provider => new PuzzleInput<IEnumerable<Image>>(provider, Parse).Value.ToList());
            services.AddSingleton(new TaskCompletionSource<Map<Image>>());
            services.AddSingleton(provider =>
                provider.GetRequiredService<TaskCompletionSource<Map<Image>>>().Task.Result);
        }

        private static IEnumerable<Image> Parse(string input)
        {
            return TilePattern.Matches(input).Select(match => new Image(int.Parse(match.Groups["Id"].Value),
                match.Groups["MapInput"].Value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(s => s.ToArray())
                    .ToArray()));
        }

        private static void ScanSurrounding(List<Image> images, Dictionary<Vector2Int, Image> imageMap, Vector2Int key)
        {
            foreach (var (position, neighbor) in DirectionExtensions.NESW()
                .Where(direction => !imageMap.ContainsKey(direction.GetStep() + key))
                .Select(direction => key + direction.GetStep())
                .Select<Vector2Int, (Vector2Int position, Image neighbor)>(v => (v, FindNeighbor(images, imageMap, v)))
                .Where(tuple => tuple.neighbor != null))
            {
                imageMap[position] = neighbor;
                images.Remove(neighbor);
            }
        }

        private static Image FindNeighbor(List<Image> images, Dictionary<Vector2Int, Image> imageMap,
            Vector2Int position)
        {
            var neighbors = DirectionExtensions.NESW()
                .Where(direction => imageMap.ContainsKey(direction.GetStep() + position))
                .Select<Direction, (Direction direction, Image value)>(direction =>
                    (direction, imageMap[direction.GetStep() + position])).ToArray();
            return images.FirstOrDefault(image => FindCombinationWith(image,
                () => neighbors.All(tuple => image.GetBorder(tuple.direction)
                    .SequenceEqual(tuple.value.GetBorder(tuple.direction.Opposite())))));
        }


        private static Image StitchImages(Map<Image> mapImage)
        {
            var result = new Dictionary<Vector2Int, char>();
            foreach (var (position, image) in mapImage)
            {
                image.Crop();
                foreach (var (pixel, value) in image)
                {
                    result.Add(position * new Vector2Int(image.XRange.Length, image.YRange.Length) + pixel, value);
                }
            }

            return new Image(result);
        }

        private static bool FindCombinationWith(Image image, Func<bool> predicate)
        {
            for (var i = 0; i < 2; i++, image.FlipVertical())
            for (var j = 0; j < 2; j++, image.FlipHorizontal())
            for (var k = 0; k < 4; k++, image.Rotate())
            {
                if (predicate.Invoke())
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetRoughness(Image image)
        {
            var charArray = image.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var count = 0;
            for (var j = 0; j < charArray.Length - (MonsterPattern.Length - 1); j++)
            for (var i = 0; i < charArray[j].Length - (MonsterPattern[0].Length - 1); i++)
            {
                if (MonsterPattern.Select((s, idx) =>
                {
                    var input = charArray[j + idx][i..(i + MonsterPattern[0].Length)];
                    var b = Regex.IsMatch(input, s);
                    return b;
                }).All(b => b))
                {
                    count++;
                }
            }

            return count > 0
                ? charArray.SelectMany(s => s.ToCharArray()).Count(s => s == '#') - count * MonsterPattern.SelectMany(s => s.ToCharArray()).Count(c => c == '#')
                : 0;
        }


        private record Image : Map<char>
        {
            public Image(Dictionary<Vector2Int, char> dict) : base(dict)
            {
            }

            public Image(int id, char[][] input) : base(input) => Id = id;
            public int Id { get; }

            public override string ToString() =>
                string.Join("\n", YRange.Reverse().Select(i => new string(this.Where(pair => pair.Key.y == i).Select(pair => pair.Value).ToArray())));

            public void Rotate() => map = map.ToDictionary(pair => pair.Key.RotateBy(90), pair => pair.Value);

            public void FlipHorizontal() =>
                map = map.ToDictionary(pair => new Vector2Int(pair.Key.x + (XRange.Mid - pair.Key.x) * 2, pair.Key.y),
                    pair => pair.Value);

            public void FlipVertical() => map = map.ToDictionary(
                pair => new Vector2Int(pair.Key.x, pair.Key.y + (YRange.Mid - pair.Key.y) * 2),
                pair => pair.Value);

            public char[] GetBorder(Direction direction) =>
                direction switch
                {
                    Direction.N => XRange.Select(i => map[new Vector2Int(i, YRange.Max)]).ToArray(),
                    Direction.E => YRange.Select(j => map[new Vector2Int(XRange.Max, j)]).ToArray(),
                    Direction.S => XRange.Select(i => map[new Vector2Int(i, YRange.Min)]).ToArray(),
                    Direction.W => YRange.Select(j => map[new Vector2Int(XRange.Min, j)]).ToArray(),
                    _ => throw new ArgumentException("{0} is an invalid direction for border", nameof(direction))
                };

            public void Crop() =>
                map = map.Where(pair => !(pair.Key.x == XRange.Min || pair.Key.x == XRange.Max || pair.Key.y == YRange.Min || pair.Key.y == YRange.Max))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        [Part(1)]
        private string Part1(List<Image> images, TaskCompletionSource<Map<Image>> completionSource)
        {
            var imageMap = new Dictionary<Vector2Int, Image>();
            var image = images[0];
            images.Remove(image);
            imageMap.Add(Vector2Int.zero, image);
            while (images.Count != 0)
            {
                foreach (var key in imageMap.Keys.ToArray())
                {
                    ScanSurrounding(images, imageMap, key);
                }
            }

            var finalImage = new Map<Image>(imageMap);
            completionSource.SetResult(finalImage);
            var corners = finalImage.Corners;
            var answer = corners.Aggregate(1L, (l, im) => l * im.Id);
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(Map<Image> mapImage)
        {
            var image = StitchImages(mapImage);
            var roughness = 0;
            if (!FindCombinationWith(image, () =>
            {
                roughness = GetRoughness(image);
                return roughness > 0;
            })) return null;
            var answer = roughness;
            return answer.ToString();
        }
    }
}