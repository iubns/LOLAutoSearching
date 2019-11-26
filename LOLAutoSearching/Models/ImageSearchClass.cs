using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static LOLAutoSearching.Models.GameManager;

namespace LOLAutoSearching.Models
{
    static class ImageSearchClass
    {
        [DllImport("ImageSearchDLL.dll")]
        private static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPTStr)]string imagePath);
        
        private static readonly int screenWindowX = 1920;//넓이 알아내는 알고리즘 필요
        private static readonly int screenWindowY = 1080;

        public static async Task<int[]> WaitCatchGame()
        {
            while (true)
            {
                int[] returnValue = new int[3];
                int[] result = ImgSearch.UseImageSearch(0, 0, screenWindowX, screenWindowY, @"Image\SelectChampion.png");

                if (ImgSearch.UseImageSearch(0, 0, screenWindowX, screenWindowY, @"Image\FindNomalGame.png") != null)
                {
                    gameType = GameType.비공개_선택;
                }
                if (ImgSearch.UseImageSearch(0, 0, screenWindowX, screenWindowY, @"Image\FindRandomGame.png") != null)
                {
                    gameType = GameType.무작위_총력전;
                }

                if (result != null)
                {
                    returnValue[0] = result[0];
                    returnValue[1] = result[1];
                    returnValue[2] = (int)gameType;
                    return returnValue;
                }
                await Task.Delay(1000);
            }
        }

        public static bool NewGame()
        {
            while (true)
            {
                int[] result = ImgSearch.UseImageSearch(0, 0, screenWindowX, screenWindowY, @"Image\FindNomalGame.png");

                if (result != null)
                {
                    return true;
                }

                result = ImgSearch.UseImageSearch(0, 0, screenWindowX, screenWindowY, @"Image\FindRandomGame.png");

                if (result != null)
                {
                    return true;
                }
                return false;
            }
        }

        public static int[] GameWindowPostion()
        {
            int[] result = ImgSearch.UseImageSearch(0, 0, screenWindowX, screenWindowY, @"Image\SelectChampion.png");

            if (result == null)
            {
                result = new int[]{ -1, -1 };
            }
            else
            {
                result[0] -= 433;
                result[1] -= 678;
            }
            return result;
        }

        private static string[] SpellNames = { "Flash", "Heal", "Ignite", "Smite", "Barrier" };
        public static bool UserIsSelect(int windowsYPostion, int userNumber)
        {
            int YAsXisGap = 120 + windowsYPostion + userNumber * 80;
            for (int SpellIndex = 0; SpellIndex < SpellNames.Length; SpellIndex++)
            {
                int[] result = ImgSearch.UseImageSearch(0, YAsXisGap, screenWindowX, screenWindowY, @"Image\" + SpellNames[SpellIndex] + ".png");
                if (result == null)
                {
                    continue;
                }
                else if (result[1] > YAsXisGap && result[1] < YAsXisGap + 80)
                {
                    return true;
                }
            }
            return false;
        }

        public static GameType GetGameType(BitmapSource bitmapSource)
        {
            Bitmap bitmap = ScreenCapture.BitmapFromSource(bitmapSource);

            int point = 0;

            for (int x = 0; x < 130; x++)
            {
                for(int y = 0; y < 40; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    if ((CheckBlack(color)))
                    {
                        point++;
                    }
                }
            }

            if (point <= 4900 && point > 4500)
            {
                return GameType.랭크_게임;
            }
            return GameType.비공개_선택;
        }

        private static bool CheckBlack(Color color)
        {
            if (color.R < 100 && color.G < 100 && color.B < 100)
            {
                return true;
            }
            return false;
        }

        public static byte[] BufferFromImage(BitmapSource image)
        {
            byte[] imageByte = null;

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            BitmapFrame bf = BitmapFrame.Create(image);
            bf.Freeze();
            encoder.Frames.Add(bf);

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                imageByte = ms.GetBuffer();
            }
            return imageByte;
        }
    }

    public class API
    {
        [DllImport("ImageSearchDLL.dll")]
        public static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)]string imagePath);

        [DllImport("ImageSearchDLL.dll")]
        public static extern IntPtr ImageSearch_img(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)]string bimagePath, [MarshalAs(UnmanagedType.LPStr)]string imagePath);
    }

    public class ImgSearch
    {
        public static int[] UseImageSearch(int VecX, int VecY, int VecX2, int VecY2, string imgPath)
        {
            IntPtr result = API.ImageSearch(VecX, VecY, VecX2, VecY2, imgPath);
            string res = Marshal.PtrToStringAnsi(result);

            if (res[0] == '0')
            {
                return null;
            }
            //찾지 못함

            string[] data = res.Split('|'); //0->찾음, 1->x, 2->y, 3->이미지 넓이, 4->이미지 높이;        

            int[] parse = new int[2];

            int.TryParse(data[1], out parse[0]);
            int.TryParse(data[2], out parse[1]);

            return parse; //x, y 좌표 반환
        }

        public static int[] UseImageSearch_img(int VecX, int VecY, int VecX2, int VecY2, string aImage, string imgPath)
        {
            IntPtr result = API.ImageSearch_img(VecX, VecY, VecX2, VecY2, aImage, imgPath);
            string res = Marshal.PtrToStringAnsi(result);

            //res에 이미지서치 결과 반환
            //실패할시 0, 찾았을 시엔 1|x|y|넓이|높이 반환

            if (res[0] == '0') return null;//찾지 못함


            string[] data = res.Split('|'); //찾은 결과값에서 x, y 좌표 추출을 위해 스피릿
                                            //0->찾음, 1->x, 2->y, 3->이미지 넓이, 4->이미지 높이;        


            int[] parse = new int[2];

            int.TryParse(data[1], out parse[0]);
            int.TryParse(data[2], out parse[1]);

            return parse; //x, y 좌표 반환
        }
    }
}