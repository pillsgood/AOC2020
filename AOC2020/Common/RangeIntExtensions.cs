namespace AOC2020.Common
{
    public static class RangeIntExtensions
    {
        public static bool Contains(this RangeInt range, int n) => n >= range.Min && n <= range.Max;

        public static int Wrap(this RangeInt rangeInt, int n) =>
            rangeInt.Min + (n - rangeInt.Min) % (rangeInt.Max + 1 - rangeInt.Min);
        
    }
}