using System;

namespace PotionPopQuest.Core
{
    public interface IRandomSource
    {
        int Range(int minInclusive, int maxExclusive);
    }

    public sealed class SystemRandomSource : IRandomSource
    {
        private readonly Random _random;

        public SystemRandomSource(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }
    }

    public sealed class DeterministicRandomSource : IRandomSource
    {
        private int _next;

        public DeterministicRandomSource(int start = 0)
        {
            _next = start;
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                return minInclusive;
            }

            var span = maxExclusive - minInclusive;
            var value = minInclusive + Math.Abs(_next % span);
            _next++;
            return value;
        }
    }
}

