namespace PotionPopQuest.Core
{
    public enum IngredientType
    {
        None = 0,
        RedHerb = 1,
        BlueCrystal = 2,
        GreenLeaf = 3,
        YellowStarDust = 4,
        PurpleMushroom = 5,
        OrangeFireDrop = 6
    }

    public enum PotionType
    {
        None = 0,
        LineHorizontal = 1,
        LineVertical = 2,
        Bomb = 3,
        Lightning = 4,
        Mega = 5
    }

    public enum ObstacleType
    {
        None = 0,
        WoodenBox = 1,
        StoneBlock = 2,
        DarkTile = 3,
        FrozenIngredient = 4,
        MagicChain = 5
    }

    public enum GoalType
    {
        CollectIngredient = 0,
        BreakObstacle = 1,
        ClearTile = 2,
        CreatePotion = 3,
        RestorePotionLab = 4
    }

    public enum MatchKind
    {
        Basic = 0,
        Line = 1,
        Bomb = 2,
        Lightning = 3
    }

    public enum GameSessionState
    {
        Ready = 0,
        Playing = 1,
        Won = 2,
        Lost = 3
    }

    public enum LogCategory
    {
        Board,
        Swap,
        Match,
        Drop,
        Goals,
        Score,
        Potion,
        Obstacle,
        Save,
        UI
    }
}

