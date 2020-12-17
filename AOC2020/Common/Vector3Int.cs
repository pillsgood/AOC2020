using System;

namespace AOC2020.Common
{
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        public static Vector3Int zero = new Vector3Int(0, 0, 0);

        private int _x;

        private int _y;

        private int _z;

        public static explicit operator Vector3Int(int[] ints)
        {
            Array.Resize(ref ints, 3);
            return new Vector3Int(ints[0], ints[1], ints[2]);
        }

        public Vector3Int(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
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

        public int z
        {
            get => _z;
            set => _z = value;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static Vector3Int operator +(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3Int operator -(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3Int operator *(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3Int operator /(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3Int operator *(Vector3Int a, int b)
        {
            return new Vector3Int(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3Int operator /(Vector3Int a, int b)
        {
            return new Vector3Int(a.x / b, a.y / b, a.z / b);
        }

        public bool Equals(Vector3Int other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y, _z);
        }

        public static bool operator ==(Vector3Int left, Vector3Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3Int left, Vector3Int right)
        {
            return !left.Equals(right);
        }
    }
}