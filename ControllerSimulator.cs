﻿using Microsoft.Extensions.Configuration;
using WindowsInput;
using WindowsInput.Native;

namespace AnkiDictionary
{
    public static class ControllerSimulator
    {
        
        private static readonly InputSimulator Simulator = new InputSimulator();

        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        private static readonly int ShortPauseValue = Int32.Parse(Config["Speed:shortPause"]!);
        private static readonly int LongPauseValue = Int32.Parse(Config["Speed:longPause"]!);

        public static void ShortPause()
        {
            Thread.Sleep(ShortPauseValue);
        }

        public static void LongPause()
        {
            Thread.Sleep(LongPauseValue);
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
        
        private static void CtrlDelete()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.DELETE);
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
        
        public static void OpenAddNewWindow()
        {
            WindowsManager.WaitUntilTheWindowAppeared("- anki", "Anki");
            LongPause();
            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
            WindowsManager.WaitUntilTheWindowAppeared("Add", "Anki add window");
        }
        
        public static void OpenBrowseWindow()
        {
            WindowsManager.WaitUntilTheWindowAppeared("- anki", "Anki");
            LongPause();
            Simulator.Keyboard.KeyPress(VirtualKeyCode.VK_B);
        }
        
        public static void AddNewNote(AnkiNote note)
        {
            
            Console.Write($"‣ {note.Text}");

            LongPause();

            // Front
            WriteText(Utility.FixFrontText(note.Text));
            ClickKey(VirtualKeyCode.TAB);

            // US
            // Ctrl + T
            CtrlT();
            WindowsManager.WaitUntilTheWindowAppeared("AwesomeTTS: Add TTS Audio to Note", "AwesomeTTS");
            WriteText(Utility.FixFrontText(note.Text));

            for (var i = 0; i < 5; i++)
            {
                ClickKey(VirtualKeyCode.TAB);
            }
            ClickKey(VirtualKeyCode.UP);
            ClickKey(VirtualKeyCode.UP);
            ClickKey(VirtualKeyCode.DOWN);
            for (var i = 0; i < 5; i++)
            {
                ClickKey(VirtualKeyCode.TAB);
            }

            // Ctrl + Enter
            CtrlEnter();
            WindowsManager.WaitUntilTheWindowClosed("AwesomeTTS: Add TTS Audio to Note", "AwesomeTTS");
            ClickKey(VirtualKeyCode.TAB);

            // UK
            // Ctrl + T
            CtrlT();
            WindowsManager.WaitUntilTheWindowAppeared("AwesomeTTS: Add TTS Audio to Note", "AwesomeTTS");
            WriteText(Utility.FixFrontText(note.Text));

            for (var i = 0; i < 5; i++)
            {
                ClickKey(VirtualKeyCode.TAB);
            }
            ClickKey(VirtualKeyCode.UP);
            ClickKey(VirtualKeyCode.UP);
            for (var i = 0; i < 5; i++)
            {
                ClickKey(VirtualKeyCode.TAB);
            }

            // Ctrl + Enter
            CtrlEnter();
            WindowsManager.WaitUntilTheWindowClosed("AwesomeTTS: Add TTS Audio to Note", "AwesomeTTS");
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
            
            Console.WriteLine($"{Utility.PrintSpaces(note.Text.Length,50)}\tAdded.\t✓");

        }
        
        public static void StartSeparatingParts(int recordsCount, int skips, string? filter = null, bool doubleDown = false)
        {
            var missedDictionary = DictionaryJsonUtility.ImportDictionaryFromJson();
            
            var frontList = "";
            foreach (var card in missedDictionary)
                frontList += card.Key + ", ";

            if (frontList.Length > 0)
            {
                frontList = frontList.Substring(1, frontList.Length - 3);
            }
            
            // Apply filter
            LongPause();
            if (filter != null)
            {
                WriteText(filter);
                ClickKey(VirtualKeyCode.RETURN);
            }
            ClickKey(VirtualKeyCode.TAB);
            for (var i = 0; i < recordsCount; i++)
            {
                try {
                    Console.Write(i+1+".");
                    // Select record
                    if (i == 0)
                    {
                        for (var j = 0; j < skips; j++)
                        {
                            if(doubleDown)
                                ClickKey(VirtualKeyCode.DOWN);
                            ClickKey(VirtualKeyCode.DOWN);
                        }
                    }
                    else
                    {
                        if(doubleDown)
                            ClickKey(VirtualKeyCode.DOWN);
                        ClickKey(VirtualKeyCode.DOWN);
                    }

                    
                    for (var j = 0; j < 4; j++)
                    {
                        ClickKey(VirtualKeyCode.TAB);
                    }

                    // check duplicate
                    CtrlA();
                    ClickKey(VirtualKeyCode.RIGHT);
                    ClickKey(VirtualKeyCode.VK_A);
                    ClickKey(VirtualKeyCode.VK_A);
                    CtrlA();
                    CtrlC();

                    var front = "";
                    
                    var duplicateCheckerText = ClipboardManager.GetText();
                    if (!duplicateCheckerText.ToLower().EndsWith("aa"))
                    {
                        ClickKey(VirtualKeyCode.TAB);
                        CtrlA();
                        CtrlC();
                        front = ClipboardManager.GetText();
                        Console.Write($"{front}{Utility.PrintSpaces(front.Length,50)}DUPLICATE");

                        for (var j = 0; j < 12; j++)
                        {
                            ClickKey(VirtualKeyCode.TAB);
                        }
                        Console.Write("\tleaved");
                        Console.WriteLine("\t✓");
                        continue;
                    }
                    else
                    {
                        ClickKey(VirtualKeyCode.BACK);
                        front = duplicateCheckerText.Substring(0, duplicateCheckerText.Length - 2);
                    }


                    // Read Front field
                    //var actualFront = front;
                    //var needFrontChange = front.Contains('[');
                    //if (needFrontChange)
                    //{
                    //    var splitFront = front.Split('[');
                    //    actualFront = splitFront[0];
                    //    actualFront = Utility.FixFrontText(actualFront);
                    //    var sound = "["+splitFront[1];

                    //    // edit
                    //    WriteText(actualFront);
                    //    ClickKey(VirtualKeyCode.TAB);
                    //    CtrlA();
                    //    WriteText(sound);
                    //}
                    //else
                    //{
                    //    actualFront = Utility.FixFrontText(actualFront);
                    //    WriteText(actualFront);
                    //    ClickKey(VirtualKeyCode.TAB);
                    //}

                    //Console.Write($"{actualFront}{Utility.PrintSpaces(actualFront.Length,50)}\tFront:{needFrontChange}");
                    ClickKey(VirtualKeyCode.TAB);
                    var actualFront = front;
                    Console.Write($"{actualFront}{Utility.PrintSpaces(actualFront.Length,50)}");

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
                    
                    Console.Write($"\tInfo:{needInfo}");
                    
                    ClickKey(VirtualKeyCode.RIGHT);
                    ClickKey(VirtualKeyCode.BACK);

                    // check definition if contains image
                    ClickKey(VirtualKeyCode.TAB);
                    ClickKey(VirtualKeyCode.TAB);
                    CtrlShiftX();
                    CtrlA();
                    CtrlC();
                    CtrlShiftX();
                    var definitionText = ClipboardManager.GetText();
                    var needImageChange = definitionText.Contains("<img");

                    Console.Write($"\tImage:{needImageChange}");

                    if (needImageChange)
                    {
                        var startDef = definitionText.IndexOf("<img", StringComparison.Ordinal);
                        var endDef = definitionText.IndexOf(">", startDef, StringComparison.Ordinal);
                        var actualDef = definitionText.Substring(0, startDef);
                        actualDef = actualDef.Replace("<br>", "");
                        actualDef = actualDef.Replace("<br/>", "");
                        actualDef = actualDef.Replace("</br>", "");
                        var image = definitionText.Substring(startDef, endDef-startDef+1);

                        // fix
                        CtrlA();
                        WriteText(actualDef);
                        ClickKey(VirtualKeyCode.TAB);
                        CtrlShiftX();
                        CtrlA();
                        WriteText(image);
                        CtrlShiftX();

                    }
                    else
                    {
                        ClickKey(VirtualKeyCode.TAB);
                    }

                    for (var j = 0; j < 7; j++)
                    {
                        ClickKey(VirtualKeyCode.TAB);
                    }
                    
                    Console.WriteLine("\t✓");

                    if (!needInfo || missedDictionary.ContainsKey(actualFront)) continue;
                    CtrlShiftI();
                    WindowsManager.WaitUntilTheWindowAppeared("Current Card", "CardInfo");
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
                    
                    missedDictionary.Add(actualFront, noteId);

                    frontList += ", "+actualFront;
                        
                    ClickKey(VirtualKeyCode.ESCAPE);
                    WindowsManager.WaitUntilTheWindowClosed("Current Card", "CardInfo");
                
                    ClipboardManager.SetText(frontList, false);
                    DictionaryJsonUtility.ExportDictionaryToJson(missedDictionary);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("\n____________\n");
                    Console.WriteLine("Restarting");
                    Console.WriteLine("\n____________\n");
                    ClickKey(VirtualKeyCode.ESCAPE);
                    ProgramHandler.SeparateImageAndPronunciation(recordsCount - i + 1,skips + i + 1, filter);
                    return;
                }
                
            }
            
            ClickKey(VirtualKeyCode.ESCAPE);
            Console.WriteLine("\n____________\n");
        }

        public static void UpdateNotes(List<AnkiNote> ankiNotes)
        {
            var dictionary = DictionaryJsonUtility.ImportDictionaryFromJson();
            var remaining = dictionary.Count;
                var counter = 0;
            foreach (var item in dictionary)
            {
                var note = ankiNotes.FirstOrDefault(note => item.Key.ToLower().Contains(note.Text.ToLower()));
                if (note == null)
                    continue;
                var checker = Utility.FixFrontText(note.Text).Equals(Utility.FixFrontText(item.Key));
                if(!checker)
                    continue;
                
                counter++;
                Console.Write($"{counter}.{item.Key}");

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

                // check duplicate
                CtrlA();
                CtrlC();
                var duplicateCheckerText = ClipboardManager.GetText();

                if (duplicateCheckerText.ToLower().Contains("nid:"))
                {
                    Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length,50)}\t✓\tWRONG nid");
                    continue;
                }

                if (!duplicateCheckerText.ToLower().Equals(item.Key.ToLower()))
                {
                    
                    Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length,50)}\t✓\tDUPLICATE");
                    for (var j = 0; j < 12; j++)
                    {
                        ClickKey(VirtualKeyCode.TAB);
                    }
                    continue;
                }

                // Front
                WriteText(Utility.FixFrontText(item.Key));
                ClickKey(VirtualKeyCode.TAB);
        
                // US
                ClickKey(VirtualKeyCode.TAB);

                // UK
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
                //CtrlA();
                //WriteText(note.Persian);
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
                
                remaining--;
                dictionary.Remove(item.Key);
                DictionaryJsonUtility.ExportDictionaryToJson(dictionary);
                Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length,50)}\t✓\trem:{remaining}");
            }
            
            ClickKey(VirtualKeyCode.ESCAPE);
        }
    }
}
