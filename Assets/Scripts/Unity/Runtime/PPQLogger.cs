using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class PPQLogger : IGameLogger
    {
        private readonly bool _verbose;

        public PPQLogger(bool verbose)
        {
            _verbose = verbose;
        }

        public void Log(LogCategory category, string message)
        {
            if (!_verbose)
            {
                return;
            }

            Debug.Log($"[PotionPopQuest][{category}] {message}");
        }

        public void Warn(LogCategory category, string message)
        {
            Debug.LogWarning($"[PotionPopQuest][{category}] {message}");
        }
    }
}

