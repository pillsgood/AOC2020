using System;

namespace AOC2020.DataTypes
{
    public struct Vector2Int : IComparable<Vector2Int>, IEquatable<Vector2Int>
    {
        public static Vector2Int zero = new Vector2Int(0, 0);

        private int _x;

        private int _y;

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


        public int CompareTo(Vector2Int other)
        {
            var xComparison = _x.CompareTo(other._x);
            return xComparison != 0 ? xComparison : _y.CompareTo(other._y);
        }

        public static bool operator <(Vector2Int left, Vector2Int right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Vector2Int left, Vector2Int right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Vector2Int left, Vector2Int right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Vector2Int left, Vector2Int right)
        {
            return left.CompareTo(right) >= 0;
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