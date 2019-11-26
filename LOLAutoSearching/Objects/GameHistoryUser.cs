namespace LOLAutoSearching.Objects
{
    public class GameHistoryUser
    {
        public GameUserData data { get; set; }
        public ChampionData championData { get; set; }
        public ItemData itemData { get; set; }
        public SpellData spellData { get; set; }
        public string win { get; set; }
    }
}
