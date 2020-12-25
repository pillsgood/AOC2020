using System;

namespace AOC2020.Common
{
    public static class Mathf
    {
        public static long Mod(long a, long b) => (Math.Abs(a * b) + a) % b;
        public static int Mod(int a, int b) => (Math.Abs(a * b) + a) % b;
    }
}