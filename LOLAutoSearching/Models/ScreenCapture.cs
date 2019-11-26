using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using LOLAutoSearching.Objects;
using PeteBrown.ScreenCapture.Win32Api;

namespace LOLAutoSearching.Models
{
    public static class ScreenCapture
    {
        public static BitmapSource GetUserImage(int gameWindowX, int gameWindowY, GameManager.GameType gameType, int UserNumber)
        {
            if (gameType == GameManager.GameType.비공개_선택)
            {
                gameWindowX += 123;
            }
            else if (gameType == GameManager.GameType.랭크_게임)
            {
                gameWindowX += 80;
            }
            else
            {
                gameWindowX += 123;
            }
            return CaptureRegion(gameWindowX, gameWindowY + 120 + UserNumber * 80, 130, 40, false);
        }

        public static async Task<BitmapSource> GetUsersAllImageAsync(int gameWindowX, int gameWindowY, GameManager.GameType gameType)
        {
            using (Bitmap bitmap = new Bitmap(130, 200, PixelFormat.Format32bppRgb))
            {
                Graphics graphics = Graphics.FromImage(bitmap);
                if (gameType == GameManager.GameType.비공개_선택)
                {
                    gameWindowX += 123;
                }
                else if (gameType == GameManager.GameType.랭크_게임)
                {
                    gameWindowX += 80;
                }
                else
                {
                    gameWindowX += 123;
                }
                await Task.Delay(1000);
                for (int userNumber = 0; userNumber < 5; userNumber++)
                {
                    var temp = CaptureRegion(gameWindowX, gameWindowY + 120 + userNumber * 80, 130, 40, false);
                    graphics.DrawImage(BitmapFromSource(temp), 0, userNumber * 40, 130, 40);
                }
                return ConvertBitmap(bitmap);
            }
        }

        public static BitmapSource GetUserChampionOnNomal(int gameWindowX, int gameWindowY, int UserNumber)
        {
            gameWindowX += 55;
            return CaptureRegion(gameWindowX, gameWindowY + 107 + UserNumber * 80, 60, 60, false);
        }

        public static BitmapSource GetUserChampionOnRank(int gameWindowX, int gameWindowY, GameUser user)
        {
            if(user.IsSelect)
            {
                gameWindowX += 60;
            }
            else if(ImageSearchClass.UserIsSelect(gameWindowY, user.index))
            {
                user.IsSelect = true;
                gameWindowX += 60;
            }
            else
            {
                gameWindowX += 22;
            }
            return CaptureRegion(gameWindowX, gameWindowY + 107 + user.index * 80, 60, 60, false);
        }

        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public static BitmapSource GetGameType(int gameWindowX, int gameWindowY)
        {

            BitmapSource bitmapSource = CaptureRegion(gameWindowX + 1010, gameWindowY + 665, 130, 50, false);
            return CaptureRegion(gameWindowX + 1010, gameWindowY + 665, 130, 40, false);
        }

        private static BitmapSource CaptureRegion(int x, int y, int width, int height, bool addToClipboard)
        {
            return CaptureRegion(User32.GetDesktopWindow(), x, y, width, height, addToClipboard);
        }

        private static BitmapSource CaptureRegion(IntPtr hWnd, int x, int y, int width, int height, bool addToClipboard)
        {
            IntPtr sourceDC = IntPtr.Zero;
            IntPtr targetDC = IntPtr.Zero;
            IntPtr compatibleBitmapHandle = IntPtr.Zero;
            BitmapSource bitmap = null;

            sourceDC = User32.GetDC(User32.GetDesktopWindow());
            targetDC = Gdi32.CreateCompatibleDC(sourceDC);

            compatibleBitmapHandle = Gdi32.CreateCompatibleBitmap(sourceDC, width, height);

            Gdi32.SelectObject(targetDC, compatibleBitmapHandle);

            Gdi32.BitBlt(targetDC, 0, 0, width, height, sourceDC, x, y, Gdi32.SRCCOPY);

            bitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                compatibleBitmapHandle, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (addToClipboard)
            {
                IDataObject data = new DataObject();
                data.SetData(DataFormats.Dib, bitmap, false);
                Clipboard.SetDataObject(data, false);
            }
            Gdi32.DeleteObject(compatibleBitmapHandle);

            User32.ReleaseDC(IntPtr.Zero, sourceDC);
            User32.ReleaseDC(IntPtr.Zero, targetDC);

            return bitmap;
        }
    }
}
