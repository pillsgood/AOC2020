using System;

namespace AOC2020.Common
{
    public readonly struct RangeInt : IEquatable<RangeInt>
    {
        private readonly int _start;
        private readonly int _end;

        public int Start => _start;
        public int End => _end;

        public RangeInt(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public override string ToString()
        {
            return $"Range: {_start}-{_end}";
        }

        public bool Equals(RangeInt other)
        {
            return _start == other._start && _end == other._end;
        }

        public override bool Equals(object obj)
        {
            return obj is RangeInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_start, _end);
        }

        public static bool operator ==(RangeInt left, RangeInt right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RangeInt left, RangeInt right)
        {
            return !left.Equals(right);
        }
    }
}