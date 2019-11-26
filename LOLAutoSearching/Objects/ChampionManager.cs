using System;
using System.Collections.Generic;
using System.IO;

namespace LOLAutoSearching.Objects
{
    public static class ChampionManager
    {
        private static string[] championsStringArray;
        public static List<Champion> CreateChampionList()
        {
            if(championsStringArray == null)
            {
                StreamReader streamReader = new StreamReader(File.OpenRead("./champion.csv"));
                championsStringArray = streamReader.ReadToEnd().Split( new string[] { "\r\n" }, StringSplitOptions.None);
            }
            List<Champion> champions = new List<Champion>();
            int index = 0;
            foreach(string championDataString in championsStringArray)
            {
                champions.Add(new Champion()
                {
                    nameEn = championDataString.Split(',')[0],
                    code = int.Parse(championDataString.Split(',')[1]),
                    nameKr = championDataString.Split(',')[2],
                    index = index++,
                    point = 0
                });
            }
            return champions;
        }

        public static Champion GetNullChampion()
        {
            Champion champion = new Champion();
            champion.nameKr = "미선택";
            champion.nameEn = "None";
            champion.code = 0;
            champion.index = -1;
            champion.point = 0;
            return champion;
        }
    }
}
