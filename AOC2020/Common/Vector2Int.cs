using System;

namespace AOC2020.Common
{
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        public static Vector2Int zero = new Vector2Int(0, 0);

        private int _x;

        private int _y;

        public static explicit operator Vector2Int(int[] ints)
        {
            Array.Resize(ref ints, 2);
            return new Vector2Int(ints[0], ints[1]);
        }

        public Vector2Int(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int x
        {
            get => _x;
            set => _x = value;
        }

        public int y
        {
            get => _y;
            set => _y = value;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static Vector2Int operator *(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x * b.x, a.y * b.y);
        }

        public static Vector2Int operator /(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x / b.x, a.y / b.y);
        }

        public static Vector2Int operator *(Vector2Int a, int b)
        {
            return new Vector2Int(a.x * b, a.y * b);
        }

        public static Vector2Int operator /(Vector2Int a, int b)
        {
            return new Vector2Int(a.x / b, a.y / b);
        }

        public static Vector2Int operator %(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int((a.x % b.x + b.x) % b.x, (a.y % b.y + b.y) % b.y);
        }


        public static Vector2Int operator %(Vector2Int a, int b)
        {
            return new Vector2Int((a.x % b + b) % b, (a.y % b + b) % b);
        }

        public bool Equals(Vector2Int other)
        {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y);
        }

        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !left.Equals(right);
        }
    }
}