using System;

namespace Raele.Util
{
    [Serializable]
    public struct Range<T> where T : struct, IComparable<T>
    {
        public T Min;
        public T Max;

        public bool Contains(T value)
            => value.CompareTo(this.Min) >= 0 && value.CompareTo(this.Max) <= 0;

        public T Clamp(T value)
            => value.CompareTo(this.Min) < 0 ? this.Min
                : value.CompareTo(this.Max) > 0 ? this.Max
                : value;
    }

    public static class Range
    {
        // TODO Remove this and this class
        public static bool IsSqrWithin(this float sqrValue, Range<float> range, float tolerance = 0.0f)
            => sqrValue - range.Min * range.Min >= -tolerance && sqrValue - range.Max * range.Max <= tolerance;
    }
}