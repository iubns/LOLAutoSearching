using LOLAutoSearching.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace LOLAutoSearching.Models
{
    public static class ChampionSearch
    {
        private readonly static int time = 9 * 9;
        private readonly static int rang = 6;

        private static float[,,,] ChampionColorDatas = null;
        
        public static Champion GetChapion(BitmapSource bitmapSource)
        {
            if (ChampionColorDatas == null)
            {
                MakeChampionsColorData();
            }
            Bitmap bitmap = GetBitmapFromBitmapSource(bitmapSource);

            List<Champion> champions = ChampionManager.CreateChampionList();
            for (int way = 0; way < time; way++)
            {
                foreach (Champion champion in champions)
                {
                    if (CheckChampionImage(champion, way, bitmap))
                    {
                        champion.point++;
                    }
                }

                if (way == time - 1)
                {
                    for (int point = time; 0 < point; point--)
                    {
                        foreach (Champion champion in champions)
                        {
                            if (champion.point == point)
                            {
                                //champion.nameKr += champion.point;
                                return champion;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static Bitmap GetBitmapFromBitmapSource(BitmapSource bitmapSource)
        {
            Bitmap bitmap;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(memoryStream);
                
                bitmap = new Bitmap(memoryStream);
            }
            return bitmap;
        }


        private static bool CheckChampionImage(Champion champion, int way, Bitmap tempBitmap)
        {
            Color tempColor;

            var resultPosion = GetDotPosition(way, false);
            tempColor = tempBitmap.GetPixel(resultPosion[0], resultPosion[1]);

            for (int rangIndex = 0; rangIndex < rang * rang; rangIndex++)
            {
                if (Check(tempColor.GetHue(), ChampionColorDatas[champion.index, way, rangIndex, 0], 30)
                && Check(tempColor.R, ChampionColorDatas[champion.index, way, rangIndex, 1], 60)
                && Check(tempColor.G, ChampionColorDatas[champion.index, way, rangIndex, 2], 60)
                && Check(tempColor.B, ChampionColorDatas[champion.index, way, rangIndex, 3], 60))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Check(float first, float second, float gap)
        {
            if(second < first + gap && first < second + gap / 3)
            {
                return true;
            }
            return false;
        }
        
        private static void MakeChampionsColorData()
        {
            var championList = ChampionManager.CreateChampionList();
            ChampionColorDatas = new float[championList.Count, time, rang * rang, 4];

            foreach (Champion champion in championList)
            {
                Bitmap championBitmap = new Bitmap(@"LOLChampions\" + champion.nameEn + ".png");
                for (int way = 0; way < time; way++)
                {
                    var resultPosion = GetDotPosition(way, true);
                    int startX = resultPosion[0];
                    int startY = resultPosion[1];

                    startX -= rang / 2;
                    startY -= rang / 2;
                    int endX = startX + rang,
                        endY = startY + rang;
                    int rangIndex = 0;
                    for (int x = startX; x < endX; x++)
                    {
                        for (int y = startY; y < endY; y++)
                        {
                            Color championColor = championBitmap.GetPixel(x, y);
                            ChampionColorDatas[champion.index, way, rangIndex, 0] = championColor.GetHue();
                            ChampionColorDatas[champion.index, way, rangIndex, 1] = championColor.R;
                            ChampionColorDatas[champion.index, way, rangIndex, 2] = championColor.G;
                            ChampionColorDatas[champion.index, way, rangIndex, 3] = championColor.B;
                            rangIndex++;
                        }
                    }
                }
            }
        }

        private static int[] GetDotPosition(int dotNumber, bool IsSavedImage)
        {
            int bigFlag;
            int smallFlag;
            int returnX;
            int returnY;

            if (IsSavedImage)
            {
                bigFlag = 17;
                smallFlag = 9;
                returnX = 60;
                returnY = 60;
            }
            else
            {
                bigFlag = 10;
                smallFlag = 5;
                returnX = 30;
                returnY = 30;
            }


            switch (dotNumber % 9)
            {
                case 0:
                    returnX -= bigFlag;
                    returnY -= bigFlag;
                    break;
                case 1:
                    returnX += 0;
                    returnY -= bigFlag;
                    break;
                case 2:
                    returnX += bigFlag;
                    returnY -= bigFlag;
                    break;
                case 3:
                    returnX -= bigFlag;
                    returnY += 0;
                    break;
                case 4:
                    returnX += 0;
                    returnY += 0;
                    break;
                case 5:
                    returnX += bigFlag;
                    returnY += 0;
                    break;
                case 6:
                    returnX -= bigFlag;
                    returnY += bigFlag;
                    break;
                case 7:
                    returnX += 0;
                    returnY += bigFlag;
                    break;
                case 8:
                    returnX += bigFlag;
                    returnY += bigFlag;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            switch (dotNumber / 9)
            {
                case 0:
                    returnX -= smallFlag;
                    returnY -= smallFlag;
                    break;
                case 1:
                    returnX += 0;
                    returnY -= smallFlag;
                    break;
                case 2:
                    returnX += smallFlag;
                    returnY -= smallFlag;
                    break;
                case 3:
                    returnX -= smallFlag;
                    returnY += 0;
                    break;
                case 4:
                    returnX += 0;
                    returnY += 0;
                    break;
                case 5:
                    returnX += smallFlag;
                    returnY += 0;
                    break;
                case 6:
                    returnX -= smallFlag;
                    returnY += smallFlag;
                    break;
                case 7:
                    returnX += 0;
                    returnY += smallFlag;
                    break;
                case 8:
                    returnX += smallFlag;
                    returnY += smallFlag;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return new int[] { returnX, returnY};
        }
    }
}
