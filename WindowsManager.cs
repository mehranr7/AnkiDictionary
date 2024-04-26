using System.Runtime.InteropServices;
using System.Text;

namespace AnkiDictionary
{
    public static class WindowsManager
    {
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc callback, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        private delegate bool EnumWindowsProc(int hWnd, int lParam);

        private static List<string> GetOpenWindowTitles()
        {
            var windowTitles = new List<string>();

            EnumWindows(delegate (int hWnd, int lParam)
            {
                var sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, sb.Capacity);
                var title = sb.ToString();
                if (!string.IsNullOrEmpty(title))
                {
                    windowTitles.Add(title);
                }
                return true;
            }, 0);

            return windowTitles;
        }

        public static string? GetTitleThatContains(string text)
        {
            var windowTitles = GetOpenWindowTitles();

            return windowTitles.FirstOrDefault(title => title.ToLower().Contains(text.ToLower()));
        }

        public static IntPtr GetWindow(string windowTitle)
        {
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            
            return FindWindow(null, windowTitle);
        }

        public static bool FocusOn(string windowTitle)
        {
            
            [DllImport("user32.dll")]
            static extern bool SetForegroundWindow(IntPtr hWnd);

            IntPtr hWnd = GetWindow(windowTitle);

            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
                Thread.Sleep(500);
                return true;
            }
            else
                return false;
        }
        
        public static void WaitUntilTheWindowAppeared(string windowsName, string printTitle = "", bool showLog = false)
        {
            if (printTitle.Equals(""))
                printTitle = windowsName;
            if (showLog)
            {
                Console.WriteLine("\n____________\n");
                Console.Write($"Looking for {printTitle} window.");
            }
            var window = GetTitleThatContains(windowsName);
            while (window==null)
            {
                Console.Write(".");
                ControllerSimulator.ShortPause();
                window = GetTitleThatContains(windowsName);
            }
            if (showLog)
            {
                Console.WriteLine("\nFounded. Please do Not close it.");
                Console.WriteLine("\n____________\n");

                Console.WriteLine("\n____________\n");
                Console.Write($"Focusing on {printTitle} and Opening Browse window.");
            }
            
            var wasFound = FocusOn(window);
            while (!wasFound)
            {
                Console.Write(".");
                ControllerSimulator.ShortPause();
                wasFound = FocusOn(window);
            }
            if (showLog)
            {
                Console.WriteLine("\nFocused. Please be patient until it finish.");
                Console.WriteLine("\n____________\n");
            }
        }
        
        public static void WaitUntilTheWindowClosed(string windowsName, string printTitle = "", bool showLog = false)
        {
            if (printTitle.Equals(""))
                printTitle = windowsName;
            
            if (showLog)
            {
                Console.WriteLine("\n____________\n");
                Console.Write($"Waiting for {printTitle} window to close.");
            }
            var window = GetTitleThatContains(windowsName);
            while (window!=null)
            {
                Console.Write(".");
                ControllerSimulator.ShortPause();
                window = GetTitleThatContains(windowsName);
            }
            if (showLog)
            {
                Console.WriteLine("\nClosed. Please do Not close it.");
                Console.WriteLine("\n____________\n");
            }
        }
    }
}
