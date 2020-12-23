using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AOC2020.Common
{
    public static class DirectionExtensions
    {
        static DirectionExtensions()
        {
            DirectionStepNormalized = new Dictionary<Direction, Vector2>();
            foreach (var (key, value) in DirectionStep)
            {
                DirectionStepNormalized.Add(key, value.Normalize());
            }
        }

        private static readonly Dictionary<Direction, Vector2> DirectionStepNormalized;

        private static readonly Dictionary<Direction, Vector2Int> DirectionStep =
            new Dictionary<Direction, Vector2Int>
            {
                { Direction.N, new Vector2Int(0, 1) },
                { Direction.NE, new Vector2Int(1, 1) },
                { Direction.E, new Vector2Int(1, 0) },
                { Direction.SE, new Vector2Int(1, -1) },
                { Direction.S, new Vector2Int(0, -1) },
                { Direction.SW, new Vector2Int(-1, -1) },
                { Direction.W, new Vector2Int(-1, 0) },
                { Direction.NW, new Vector2Int(-1, 1) }
            };

        public static Vector2Int GetStep(this Direction direction) => DirectionStep[direction];
        public static Vector2 GetNormalizedStep(this Direction direction) => DirectionStepNormalized[direction];

        public static Direction RotateByDegree(this Direction direction, int rotation)
        {
            var len = ((int) Direction.NW + 1);
            var dir = ((int) direction - rotation % 360 / 45) % len;
            return (Direction) (dir < 0 ? dir + len : dir);
        }

        public static IEnumerable<Direction> NESW() => Enumerable.Range(0, 4).Select(i => (Direction) (i * 2));

        public static Direction Opposite(this Direction direction) => (Direction) ((int) (direction + 4) % 8);
    }
}