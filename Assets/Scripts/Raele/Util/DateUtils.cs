using System;

namespace Raele.Util
{
    public static class DateUtils
    {
        public static long GetEpochTimeMs()
            => DateTime.UtcNow.Ticks / 10000;
    }
}
