using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

namespace AnkiDictionary
{
    public static class ControllerSimulator
    {
        
        private static readonly InputSimulator Simulator = new InputSimulator();

        private static void ShortPause()
        {
            Thread.Sleep(200);
        }

        private static void LongPause()
        {
            Thread.Sleep(1000);
        }

        private static void ClickKey(VirtualKeyCode keyCode)
        {
            Simulator.Keyboard.KeyPress(keyCode);
            ShortPause();
        }

        private static void WriteText(string text)
        {
            if(string.IsNullOrEmpty(text))
                return;
            Simulator.Keyboard.TextEntry(text);
            ShortPause();
        }
        
        private static void CtrlT()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_T);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
            ShortPause();
        }

        private static void CtrlEnter()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.RETURN);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
            ShortPause();
        }

        private static string GetListOf(List<string> list)
        {
            var collocations = "";
            foreach (var collocation in list)
            {
                if(collocations.Length > 0)
                    collocations += Environment.NewLine;
                collocations += $@"- {collocation}";
            }

            return collocations;
        }

        public static void AddNewNote(AnkiNote note)
        {
            LongPause();

            // Front
            WriteText(note.Text);
            ClickKey(VirtualKeyCode.TAB);
            
            // Pronunciation
            // Ctrl + T
            CtrlT();
            WriteText(note.Text);

            // Ctrl + Enter
            CtrlEnter();

            var hWnd = WindowsManager.GetWindow("AwesomeTTS: Add TTS Audio to Note");
            while (hWnd != IntPtr.Zero)
            {
                Thread.Sleep(500);
                hWnd = WindowsManager.GetWindow("AwesomeTTS: Add TTS Audio to Note");
            }

            ClickKey(VirtualKeyCode.TAB);
            
            // Type
            WriteText(note.Type);
            ClickKey(VirtualKeyCode.TAB);

            // Usage
            WriteText(note.Usage);
            ClickKey(VirtualKeyCode.TAB);

            // Definition
            WriteText(note.Definition);
            ClickKey(VirtualKeyCode.TAB);
            
            // Image
            //WriteText(note.Image);
            ClickKey(VirtualKeyCode.TAB);
            
            // Sentence
            WriteText(note.Sentence);
            ClickKey(VirtualKeyCode.TAB);

            // Persian
            WriteText(note.Persian);
            ClickKey(VirtualKeyCode.TAB);

            // Collocation
            WriteText(GetListOf(note.Collocations));
            ClickKey(VirtualKeyCode.TAB);

            // Synonyms
            WriteText(GetListOf(note.Synonyms));
            ClickKey(VirtualKeyCode.TAB);

            // Antonyms
            WriteText(GetListOf(note.Antonyms));
            ClickKey(VirtualKeyCode.TAB);

            // Ctrl + Enter
            CtrlEnter();

        }
        
        public static bool OpenNewWindow(string ankiWindowTitle)
        {
            var wasFound = WindowsManager.FocusOn(ankiWindowTitle);
            if (!wasFound) return wasFound;
            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
            LongPause();
            return wasFound;
        }

        public static void Copy(string textToCopy)
        {
            [DllImport("user32.dll")]
            static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [DllImport("user32.dll")]
            static extern bool CloseClipboard();

            [DllImport("user32.dll")]
            static extern bool EmptyClipboard();

            [DllImport("user32.dll")]
            static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

            const uint cfUniCodeText = 13;
            
            if (!OpenClipboard(IntPtr.Zero)) return;

            EmptyClipboard();
            var bytes = System.Text.Encoding.Unicode.GetBytes(textToCopy);
            var ptr = Marshal.AllocHGlobal(bytes.Length + 2);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteInt16(ptr + bytes.Length, 0);
            SetClipboardData(cfUniCodeText, ptr);
            CloseClipboard();
        }
    }
}
