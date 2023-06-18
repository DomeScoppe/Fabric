using System;

namespace Loom.Core
{
    public static class ID
    {
        public static ulong INVALID_ID => UInt64.MaxValue;
        public static bool IsValid(ulong id) => id != INVALID_ID;
    }

    public static class MathUtilities
    {
        public static float Epsilon => 0.00001f;

        public static bool IsTheSameAs(this float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        public static bool IsTheSameAs(this float? a, float? b)
        {
            if (!a.HasValue || !b.HasValue) return false;

            return Math.Abs(a.Value - b.Value) < Epsilon;
        }
    }
}
