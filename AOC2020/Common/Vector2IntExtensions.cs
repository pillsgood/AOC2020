using System;
using System.Numerics;

namespace AOC2020.Common
{
    public static class Vector2IntExtensions
    {
        public static Vector2Int Abs(this Vector2Int v)
        {
            return new Vector2Int(Math.Abs(v.x), Math.Abs(v.y));
        }

        public static Vector2Int RotateBy(this Vector2Int v, int rotation)
        {
            var ca = Math.Cos(rotation * Math.PI / 180);
            var sa = Math.Sin(rotation * Math.PI / 180);
            var vX = Math.Round(ca * v.x - sa * v.y);
            var vY = Math.Round(sa * v.x + ca * v.y);
            return new Vector2Int((int) vX, (int) vY);
        }

        public static float MagnitudeSquared(this Vector2Int v) => v.ToVector2().LengthSquared();

        public static Vector2 Normalize(this Vector2Int v) => Vector2.Normalize(v.ToVector2());

        public static Vector2 ToVector2(this Vector2Int v) => new Vector2(v.x, v.y);
    }
}