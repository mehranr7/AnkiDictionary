using System.Runtime.InteropServices;
using System.Text;

namespace AnkiDictionary
{
    public static class ClipboardManager
    {
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("Kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr hMem);

        private const uint cfUnicodeText = 13;

        public static bool SetText(string text, bool shouldPrint = true)
        {
            if (!OpenClipboard(IntPtr.Zero))
                return false;

            EmptyClipboard();
            var bytes = Encoding.Unicode.GetBytes(text);
            var ptr = Marshal.AllocHGlobal(bytes.Length + 2);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteInt16(ptr + bytes.Length, 0);
            SetClipboardData(cfUnicodeText, ptr);
            CloseClipboard();

            if (!shouldPrint) return true;
            Console.WriteLine("\n____________\n");
            Console.WriteLine(text+" Copied.");
            Console.WriteLine("\n____________\n");
            return true;
        }

        public static string GetText()
        {
            if (!OpenClipboard(IntPtr.Zero))
                return null;

            IntPtr clipboardData = GetClipboardData(cfUnicodeText);
            if (clipboardData == IntPtr.Zero)
            {
                CloseClipboard();
                return null;
            }

            IntPtr globalMemoryHandle = GlobalLock(clipboardData);
            if (globalMemoryHandle == IntPtr.Zero)
            {
                CloseClipboard();
                return null;
            }

            string copiedText = Marshal.PtrToStringUni(globalMemoryHandle);

            GlobalUnlock(globalMemoryHandle);
            CloseClipboard();

            return copiedText;
        }
    }
}
