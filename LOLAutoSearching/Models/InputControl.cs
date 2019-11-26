using System.Runtime.InteropServices;
using System.Threading;

namespace LOLAutoSearching.Models
{
    public static class InputControl
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);

        public static void ChatContentCopy(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(0x0002, 0, 0, 0, 0);

            keybd_event(162, 0, 0, 0);
            keybd_event(65, 0, 0, 0);
            keybd_event(65, 0, 2, 0);

            Thread.Sleep(100);

            keybd_event(67, 0, 0, 0);
            keybd_event(67, 0, 2, 0);
            keybd_event(162, 0, 2, 0);

            mouse_event(0x0004, 0, 0, 0, 0);

            Thread.Sleep(1000);

            SetCursorPos(x, y + 40);
            mouse_event(0x0002, 0, 0, 0, 0);
            mouse_event(0x0004, 0, 0, 0, 0);
        }

        private static void KeyClick(int mKey)
        {
            keybd_event((byte)mKey, 0, 0, 0);
            keybd_event((byte)mKey, 0, 2, 0);
        }

        public static void MouseClick(int x ,int y)
        {
            SetCursorPos(x, y);
            mouse_event(0x0002, 0, 0, 0, 0);
            mouse_event(0x0004, 0, 0, 0, 0);
        }
    }
}
