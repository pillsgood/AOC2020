using System;
using System.Collections;
using System.Collections.Generic;

namespace AOC2020.Common
{
    public readonly struct RangeInt : IEquatable<RangeInt>, IEnumerable<int>
    {
        private readonly int _start;
        private readonly int _end;

        public int Min => Math.Min(_start, _end);
        public int Max => Math.Max(_start, _end);

        public int Length => (_end - _start) + 1;
        public int Mid => _start + Length / 2;

        public RangeInt(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public override string ToString()
        {
            return $"Range: {_start}-{_end}";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(RangeInt other)
        {
            return _start == other._start && _end == other._end;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = _start; i <= _end; i++)
            {
                yield return i;
            }
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