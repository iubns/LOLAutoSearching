namespace LOLAutoSearching.Objects
{
    public class SpellData
    {
        public static string GetSpellName(int spellID)
        {
            string spellName = string.Empty;
            switch (spellID)
            {
                case 21:
                    spellName = "SummonerBarrier";
                    break;
                case 1:
                    spellName = "SummonerBoost";
                    break;
                case 35:
                    spellName = "SummonerDarkStarChampSelect1";
                    break;
                case 36:
                    spellName = "SummonerDarkStarChampSelect2";
                    break;
                case 14:
                    spellName = "SummonerDot";
                    break;
                case 3:
                    spellName = "SummonerExhaust";
                    break;
                case 4:
                    spellName = "SummonerFlash";
                    break;
                case 6:
                    spellName = "SummonerHaste";
                    break;
                case 7:
                    spellName = "SummonerHeal";
                    break;
                case 13:
                    spellName = "SummonerMana";
                    break;
                case 52:
                    spellName = "SummonerOdysseyFlash";
                    break;
                case 120:
                    spellName = "SummonerOdysseyGhost";
                    break;
                case 50:
                    spellName = "SummonerOdysseyRevive";
                    break;
                case 30:
                    spellName = "SummonerPoroRecall";
                    break;
                case 31:
                    spellName = "SummonerPoroThrow";
                    break;
                case 33:
                    spellName = "SummonerSiegeChampSelect1";
                    break;
                case 34:
                    spellName = "SummonerSiegeChampSelect2";
                    break;
                case 11:
                    spellName = "SummonerSmite";
                    break;
                case 39:
                    spellName = "SummonerSnowURFSnowball_Mark";
                    break;
                case 32:
                    spellName = "SummonerSnowball";
                    break;
                case 12:
                    spellName = "SummonerTeleport";
                    break;
            }
            return spellName;
        }

        public string spell1ID { get; set; }
        public string spell2ID { get; set; }    
}
}
