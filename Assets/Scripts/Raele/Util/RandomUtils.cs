using System;

namespace Raele.Util
{
    public static class RandomUtils
    {
        private static Random rng = new Random();

        public static int NextInt(int maxValue) => rng.Next(maxValue);
    }
}
