using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class ScoreManager
    {
        public int CalculateMatchScore(IReadOnlyList<MatchGroup> matches, int cascadeIndex)
        {
            var baseScore = matches.Sum(match => match.Positions.Count * 50);
            var specialBonus = matches.Sum(match =>
            {
                switch (match.Kind)
                {
                    case MatchKind.Line:
                        return 150;
                    case MatchKind.Bomb:
                        return 250;
                    case MatchKind.Lightning:
                        return 350;
                    default:
                        return 0;
                }
            });
            return (baseScore + specialBonus) * (cascadeIndex + 1);
        }

        public int CalculatePotionScore(int clearedTiles, PotionType potionType)
        {
            var bonus = potionType == PotionType.Mega ? 500 : 200;
            return clearedTiles * 60 + bonus;
        }
    }
}

