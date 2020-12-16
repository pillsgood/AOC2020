namespace AOC2020.Common
{
    public static class RangeIntExtensions
    {
        public static bool Contains(this RangeInt range, int n) => n >= range.Start && n <= range.End;
    }
}