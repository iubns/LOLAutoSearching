namespace LOLAutoSearching.Objects
{
    public class GameUser
    {
        public int index { get; set; }
        public GameUserData data { get; set; }
        public ChampionData championData { get; set; }
        public Champion champion { get; set; } = ChampionManager.GetNullChampion();
        public bool IsSelect { get; set; } = false;
    }
}
