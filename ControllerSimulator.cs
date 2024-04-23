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
        
        private static void CtrlA()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_A);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
            ShortPause();
        }
        
        private static void CtrlC()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_C);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
            ShortPause();
        }
        
        private static void CtrlShiftI()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            Simulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_I);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
            ShortPause();
            Simulator.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            LongPause();
        }
        
        private static void CtrlShiftX()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            Simulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_X);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
            ShortPause();
            Simulator.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
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
        
        public static bool OpenAddNewWindow(string ankiWindowTitle)
        {
            var wasFound = WindowsManager.FocusOn(ankiWindowTitle);
            if (!wasFound) return wasFound;
            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
            LongPause();
            return wasFound;
        }
        
        public static bool OpenBrowseWindow(string ankiWindowTitle)
        {
            var wasFound = WindowsManager.FocusOn(ankiWindowTitle);
            if (!wasFound) return wasFound;
            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_B);
            LongPause();
            return wasFound;
        }
        
        public static Dictionary<string,string> StartSeparatingParts(string filter, int recordsCount, int skips)
        {
            var unfinishedCards = new Dictionary<string,string>();

            // Apply filter
            LongPause();
            WriteText(filter);
            ClickKey(VirtualKeyCode.RETURN);
            ClickKey(VirtualKeyCode.TAB);
            for (var i = 0; i < recordsCount; i++)
            {

                // Select record
                if (i == 0)
                {
                    for (var j = 0; j < skips; j++)
                    {
                        ClickKey(VirtualKeyCode.DOWN);
                        ClickKey(VirtualKeyCode.DOWN);
                    }
                }
                else
                {
                    ClickKey(VirtualKeyCode.DOWN);
                    ClickKey(VirtualKeyCode.DOWN);
                }

                
                for (var j = 0; j < 4; j++)
                {
                    ClickKey(VirtualKeyCode.TAB);
                }

                // Read Front field                     
                CtrlA();
                CtrlC();
                var front = ClipboardManager.GetText();
                var actualFront = front;
                if (front.Contains('['))
                {
                    var splitFront = front.Split('[');
                    actualFront = splitFront[0];
                    actualFront = actualFront.Replace("\n", "");
                    actualFront = actualFront.Replace("\r", "");
                    var sound = "["+splitFront[1];

                    // edit
                    WriteText(actualFront);
                    ClickKey(VirtualKeyCode.TAB);
                    CtrlA();
                    WriteText(sound);
                }
                else
                {
                    ClickKey(VirtualKeyCode.TAB);
                }

                // check TypeGroup
                ClickKey(VirtualKeyCode.TAB);
                CtrlA();
                ClickKey(VirtualKeyCode.RIGHT);
                ClickKey(VirtualKeyCode.VK_A);
                CtrlA();
                CtrlC();

                var needInfo = false;
                var typeGroup = ClipboardManager.GetText();
                if (typeGroup.ToLower().Equals("a"))
                    needInfo = true;

                
                ClickKey(VirtualKeyCode.RIGHT);
                ClickKey(VirtualKeyCode.BACK);

                // check definition if contains image
                ClickKey(VirtualKeyCode.TAB);
                ClickKey(VirtualKeyCode.TAB);
                CtrlShiftX();
                CtrlA();
                CtrlC();
                var definitionText = ClipboardManager.GetText();
                if (definitionText.Contains("<img"))
                {
                    var startDef = definitionText.IndexOf("<img", StringComparison.Ordinal);
                    var endDef = definitionText.IndexOf(">", startDef, StringComparison.Ordinal);
                    var actualDef = definitionText.Substring(0, startDef);
                    actualDef = actualDef.Replace("<br>", "");
                    actualDef = actualDef.Replace("<br/>", "");
                    var image = definitionText.Substring(startDef, endDef-startDef);

                    // fix
                    WriteText(actualDef);
                    CtrlShiftX();
                    ClickKey(VirtualKeyCode.TAB);
                    CtrlShiftX();
                    WriteText(image);
                    CtrlShiftX();

                }

                for (var j = 0; j < 8; j++)
                {
                    ClickKey(VirtualKeyCode.TAB);
                }

                if (!needInfo || unfinishedCards.ContainsKey(actualFront)) continue;
                CtrlShiftI();
                ClickKey(VirtualKeyCode.TAB);
                CtrlA();
                CtrlC();
                var cardInfo = ClipboardManager.GetText();
                var start = cardInfo.IndexOf("Note ID", StringComparison.Ordinal);
                var end = cardInfo.IndexOf("\n", start, StringComparison.Ordinal);
                var noteId = cardInfo.Substring(start, end-start);
                    
                noteId = noteId.Replace("Note ID", "");
                noteId = noteId.Replace("\n", "");
                noteId = noteId.Replace("\r", "");
                noteId = noteId.Replace("\t", "");

                unfinishedCards.Add(actualFront, noteId);
                    
                ClickKey(VirtualKeyCode.ESCAPE);

            }
            
            ClickKey(VirtualKeyCode.ESCAPE);
            return unfinishedCards;
        }

        public static void UpdateNotes(List<AnkiNote> ankiNotes)
        {
            var dictionary = DictionaryJsonUtility.ImportDictionaryFromJson();

            foreach (var item in dictionary)
            {
                Console.WriteLine(item.Key);

                var note = ankiNotes.FirstOrDefault(note => item.Key.ToLower().Contains(note.Text.ToLower()));
                if (note == null) continue;

                // find the note
                LongPause();
                
                CtrlA();
                WriteText("nid:"+item.Value);
                ClickKey(VirtualKeyCode.RETURN);
                ClickKey(VirtualKeyCode.TAB);

                // go to first field
                for (var i = 0; i < 4; i++)
                {
                    ClickKey(VirtualKeyCode.TAB);
                }

                // update data

                // Front
                CtrlA();
                WriteText(note.Text);
                ClickKey(VirtualKeyCode.TAB);
        
                // Pronunciation
                ClickKey(VirtualKeyCode.TAB);
        
                // Type
                CtrlA();
                WriteText(note.Type);
                ClickKey(VirtualKeyCode.TAB);

                // Usage
                CtrlA();
                WriteText(note.Usage);
                ClickKey(VirtualKeyCode.TAB);

                // Definition
                CtrlA();
                WriteText(note.Definition);
                ClickKey(VirtualKeyCode.TAB);
        
                // Image
                //WriteText(note.Image);
                ClickKey(VirtualKeyCode.TAB);
        
                // Sentence
                CtrlA();
                WriteText(note.Sentence);
                ClickKey(VirtualKeyCode.TAB);

                // Persian
                CtrlA();
                WriteText(note.Persian);
                ClickKey(VirtualKeyCode.TAB);

                // Collocation
                CtrlA();
                WriteText(GetListOf(note.Collocations));
                ClickKey(VirtualKeyCode.TAB);

                // Synonyms
                CtrlA();
                WriteText(GetListOf(note.Synonyms));
                ClickKey(VirtualKeyCode.TAB);

                // Antonyms
                CtrlA();
                WriteText(GetListOf(note.Antonyms));
                ClickKey(VirtualKeyCode.TAB);

                dictionary.Remove(item.Key);
                DictionaryJsonUtility.ExportDictionaryToJson(dictionary);
            }

            ClickKey(VirtualKeyCode.ESCAPE);
        }
    }
}
