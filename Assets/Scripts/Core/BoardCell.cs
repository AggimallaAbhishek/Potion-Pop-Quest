namespace PotionPopQuest.Core
{
    public sealed class BoardCell
    {
        public BoardCell()
        {
        }

        public BoardCell(IngredientType ingredient, PotionType potion, ObstacleType obstacle, int obstacleHealth)
        {
            Ingredient = ingredient;
            Potion = potion;
            Obstacle = obstacle;
            ObstacleHealth = obstacleHealth;
        }

        public IngredientType Ingredient { get; set; }
        public PotionType Potion { get; set; }
        public ObstacleType Obstacle { get; set; }
        public int ObstacleHealth { get; set; }

        public bool HasIngredient => Ingredient != IngredientType.None;
        public bool HasPotion => Potion != PotionType.None;
        public bool HasObstacle => Obstacle != ObstacleType.None;
        public bool BlocksIngredientSpace => Obstacle == ObstacleType.WoodenBox || Obstacle == ObstacleType.StoneBlock;
        public bool LocksIngredient => Obstacle == ObstacleType.FrozenIngredient || Obstacle == ObstacleType.MagicChain;
        public bool AcceptsIngredient => !BlocksIngredientSpace;
        public bool CanMoveIngredient => HasIngredient && AcceptsIngredient && !LocksIngredient;

        public BoardCell Clone()
        {
            return new BoardCell(Ingredient, Potion, Obstacle, ObstacleHealth);
        }

        public void ClearIngredient()
        {
            Ingredient = IngredientType.None;
            Potion = PotionType.None;
        }

        public override string ToString()
        {
            var potion = Potion == PotionType.None ? string.Empty : $"+{Potion}";
            var obstacle = Obstacle == ObstacleType.None ? string.Empty : $" [{Obstacle}:{ObstacleHealth}]";
            return $"{Ingredient}{potion}{obstacle}";
        }
    }
}

