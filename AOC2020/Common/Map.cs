using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020.Common
{
    public abstract record Map<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _map = new();

        protected Map()
        {
        }

        protected Map(Dictionary<TKey, TValue> map)
        {
            _map = map;
        }

        protected Dictionary<TKey, TValue> map
        {
            get => _map;
            set => _map = value;
        }

        public virtual TValue this[TKey key]
        {
            get => map[key];
            set => map[key] = value;
        }


        public IEnumerable<KeyValuePair<TKey, TValue>> Entries
        {
            init => _map = value.ToArray().ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _map).GetEnumerator();
    }

    public record Map<T> : Map<Vector2Int, T>
    {
        public Map(IReadOnlyCollection<T[]> array) =>
            map = array.SelectMany((row, j) => row.Select<T, (Vector2Int pos, T value)>((c, i) => (new Vector2Int(i, array.Count - j), c)))
                .ToDictionary(tuple => tuple.pos, tuple => tuple.value);

        public Map(Dictionary<Vector2Int, T> map) => this.map = map;

        protected Map()
        {
        }



        public RangeInt XRange { get; private set; } = new(0, 0);
        public RangeInt YRange { get; private set; } = new(0, 0);

        protected new Dictionary<Vector2Int, T> map
        {
            get => base.map;
            set
            {
                base.map = value;
                UpdateRange();
                Normalize();
            }
        }

        public T this[int x, int y] => map[new Vector2Int(x, y)];

        public IEnumerable<T> Corners => new[]
        {
            map[new Vector2Int(XRange.Min, YRange.Min)],
            map[new Vector2Int(XRange.Max, YRange.Min)],
            map[new Vector2Int(XRange.Max, YRange.Max)],
            map[new Vector2Int(XRange.Min, YRange.Max)],
        };

        public virtual bool Equals(Map<T> other)
        {
            return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || this.SequenceEqual(other));
        }



        public override IEnumerator<KeyValuePair<Vector2Int, T>> GetEnumerator() =>
            base.map.OrderByDescending(pair => pair.Key.y).ThenBy(pair => pair.Key.x).GetEnumerator();

        private void UpdateRange()
        {
            XRange = new RangeInt(base.map.Min(pair => pair.Key.x), base.map.Max(pair => pair.Key.x));
            YRange = new RangeInt(base.map.Min(pair => pair.Key.y), base.map.Max(pair => pair.Key.y));
        }

        private void Normalize()
        {
            base.map = base.map.ToDictionary(pair => pair.Key + (Vector2Int.zero - new Vector2Int(XRange.Min, YRange.Min)), pair => pair.Value);
            UpdateRange();
        }

        public override int GetHashCode() => base.map != null ? base.map.GetHashCode() : 0;
    }
}