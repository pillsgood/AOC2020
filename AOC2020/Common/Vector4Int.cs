using System;

namespace AOC2020.Common
{
    public struct Vector4Int : IEquatable<Vector4Int>
    {
        public static Vector4Int zero = new Vector4Int(0, 0, 0, 0);

        private int _x;

        private int _y;

        private int _z;

        private int _w;

        public static explicit operator Vector4Int(int[] ints)
        {
            Array.Resize(ref ints, 4);
            return new Vector4Int(ints[0], ints[1], ints[2], ints[4]);
        }

        public Vector4Int(int x, int y, int z, int w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
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

        public int w
        {
            get => _w;
            set => _w = value;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }

        public static Vector4Int operator +(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4Int operator -(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4Int operator *(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        public static Vector4Int operator /(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
        }

        public static Vector4Int operator *(Vector4Int a, int b)
        {
            return new Vector4Int(a.x * b, a.y * b, a.z * b, a.w + b);
        }

        public static Vector4Int operator /(Vector4Int a, int b)
        {
            return new Vector4Int(a.x / b, a.y / b, a.z / b, a.w / b);
        }

        public bool Equals(Vector4Int other)
        {
            return _x == other._x && _y == other._y && _z == other._z && _w == other._w;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y, _z, _w);
        }

        public static bool operator ==(Vector4Int left, Vector4Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4Int left, Vector4Int right)
        {
            return !left.Equals(right);
        }
    }
}