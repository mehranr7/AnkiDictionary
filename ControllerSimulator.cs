using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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

        private static void WriteText(string text, bool fixText = true)
        {
            if(string.IsNullOrEmpty(text))
                return;

            Simulator.Keyboard.TextEntry(fixText ? Utility.FixFrontText(text) : text);
            ShortPause();
        }
        
        private static void CtrlF()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_F);
            Simulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL);
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
        
        private static void CtrlH()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_H);
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
        
        private static void CtrlShiftT()
        {
            Simulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
            ShortPause();
            Simulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
            ShortPause();
            ClickKey(VirtualKeyCode.VK_T);
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
            if (list == null || list.Count == 0) return "";
            var collocations = "";
            foreach (var collocation in list)
            {
                if(collocations.Length > 0)
                    collocations += Environment.NewLine;
                if(collocation.Length > 0)
                    collocations += $@"- {Utility.FixFrontText(collocation)}";
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
        
        public static async Task AddNewNote(AnkiNote note, List<string> tags)
        {
            
            Console.Write($"‣ {note.Text}");

            LongPause();

            // Front
            WriteText(note.Text);
            ClickKey(VirtualKeyCode.TAB);

            // US
            // Ctrl + T
            CtrlT();
            WindowsManager.WaitUntilTheWindowAppeared("AwesomeTTS: Add TTS Audio to Note", "AwesomeTTS");
            WriteText(note.Text);

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
            WriteText(note.Text);

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
            
            // Level
            WriteText(note.Level);
            ClickKey(VirtualKeyCode.TAB);

            // Band
            WriteText(note.Band);
            ClickKey(VirtualKeyCode.TAB);
            
            // Frequency
            WriteText(note.Frequency.ToString());
            ClickKey(VirtualKeyCode.TAB);
            
            // American Phonetic
            WriteText(note.AmericanPhonetic);
            ClickKey(VirtualKeyCode.TAB);
            
            // British Phonetic
            WriteText(note.BritishPhonetic);
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
            WriteText(GetListOf(note.Collocations),false);
            ClickKey(VirtualKeyCode.TAB);

            // Synonyms
            WriteText(GetListOf(note.Synonyms),false);
            ClickKey(VirtualKeyCode.TAB);

            // Antonyms
            WriteText(GetListOf(note.Antonyms),false);
            ClickKey(VirtualKeyCode.TAB);

            // Verb
            if(note.Verb!=null)
                WriteText(note.Verb);
            ClickKey(VirtualKeyCode.TAB);

            // Noun
            if(note.Noun!=null)
                WriteText(note.Noun);
            ClickKey(VirtualKeyCode.TAB);

            // Adjective
            if(note.Adjective!=null)
                WriteText(note.Adjective);
            ClickKey(VirtualKeyCode.TAB);

            // Adverb
            if(note.Adverb!=null)
                WriteText(note.Adverb);
            
            // Tags
            CtrlShiftT();
            CtrlA();
            ClickKey(VirtualKeyCode.BACK);
            ClickKey(VirtualKeyCode.TAB);
            CtrlShiftT();
            note.Categories.AddRange(tags);
            foreach (var tag in note.Categories)
            {
                WriteText(tag);
                ClickKey(VirtualKeyCode.RETURN);
            }

            // Ctrl + Enter
            CtrlEnter();
            
            Console.WriteLine($"{Utility.PrintSpaces(note.Text.Length,50)}\tAdded.\t✓");

            CtrlH();
            ClickKey(VirtualKeyCode.DOWN);
            ClickKey(VirtualKeyCode.RETURN);
            note.NoteId = ReadNoteId();
            ClickKey(VirtualKeyCode.ESCAPE);
            var database = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("database.json") ?? new List<AnkiNote>();
            database.Add(note);
            await JsonFileHandler.SaveToJsonFileAsync(database, "database.json");

        }
        
        public static async Task FindNeededItems(int neededFieldIndex, int recordsCount, int skips, string? filter = null, bool doubleDown = false, string? mark = null)
        {
            var missedDictionary = await JsonFileHandler.ReadFromJsonFileAsync<Dictionary<string, string>>("cardsInNeed.json");
            if(missedDictionary  == null) return;

            var frontList = "";
            foreach (var card in missedDictionary)
                frontList += card.Key + ", ";

            if (frontList.Length > 3)
            {
                frontList = frontList.Substring(1, frontList.Length - 3);
            }
            
            // Apply filter
            LongPause();
            CtrlF();
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

                    // to first field
                    for (var j = 0; j < 4; j++)
                    {
                        ClickKey(VirtualKeyCode.TAB);
                    }
                    CtrlA();
                    CtrlC();
                    var front = ClipboardManager.GetText();

                    Console.Write(front);
                    
                    var needInfo = false;
                    if (neededFieldIndex > 0)
                    {
                        // check needed item
                        for (var j = 0; j < neededFieldIndex-1; j++)
                        {
                            ClickKey(VirtualKeyCode.TAB);
                        }
                        CtrlA();
                        ClickKey(VirtualKeyCode.RIGHT);
                        ClickKey(VirtualKeyCode.VK_A);
                        ClickKey(VirtualKeyCode.VK_A);
                        CtrlA();
                        CtrlC();

                        var neededItem = ClipboardManager.GetText();
                        if (neededItem.ToLower().Equals("aa"))
                            needInfo = true;
                    
                        Console.Write($"\tInfo:{needInfo}");
                    
                        ClickKey(VirtualKeyCode.RIGHT);
                        ClickKey(VirtualKeyCode.BACK);
                        ClickKey(VirtualKeyCode.BACK);

                        if (needInfo && mark!=null)
                        {
                            WriteText(mark);
                        }
                        
                    }
                    else
                    {
                        if (mark != null)
                        {
                            for (var j = 0; j < 5; j++)
                            {
                                ClickKey(VirtualKeyCode.TAB);
                            }
                            WriteText(mark);
                        }
                        needInfo = true;
                        Console.Write($"\tInfo:{needInfo}");
                    }

                    // go on the current note
                    CtrlF();
                    ClickKey(VirtualKeyCode.TAB);
                    
                    // finish if no needed item founded
                    if (!needInfo || missedDictionary.ContainsKey(front))
                    {
                        
                        Console.Write("\tAlready Exists");
                        Console.WriteLine("\t✓");
                        continue;
                    }

                    // read the needed item details
                    var noteId = ReadNoteId();

                    missedDictionary.Add(front, noteId);

                    if (frontList.Length > 0)
                    {
                        frontList += ", "+front;
                    }
                    else
                    {
                        frontList += front;
                    }
                
                    ClipboardManager.SetText(frontList, false);
                    await JsonFileHandler.SaveToJsonFileAsync(missedDictionary, "cardsInNeed.json");

                    Console.WriteLine("\t✓");
                }
                catch (Exception e)
                {
                    await Utility.SaveAnError("Line 453 in Controller", e);

                    Console.WriteLine("\n____________\n");
                    Console.WriteLine("Restarting");
                    Console.WriteLine("\n____________\n");
                    ClickKey(VirtualKeyCode.ESCAPE);
                    
                    OpenBrowseWindow();
                    await FindNeededItems(neededFieldIndex, recordsCount - i + 1,skips + i + 1, filter);
                    return;
                }
                
            }
            
            ClickKey(VirtualKeyCode.ESCAPE);
            Console.WriteLine("\n____________\n");
        }

        private static string ReadNoteId()
        {
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
                        
            ClickKey(VirtualKeyCode.ESCAPE);
            WindowsManager.WaitUntilTheWindowClosed("Current Card", "CardInfo");

            return noteId;
        }

        public static async Task UpdateNotes(List<AnkiNote> ankiNotes)
        {
            var dictionary = await JsonFileHandler.ReadFromJsonFileAsync<Dictionary<string, string>>("cardsInNeed.json");
            if(dictionary  == null) return;

            var remaining = dictionary.Count;
                var counter = 0;
            foreach (var item in dictionary)
            {
                try
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
                    CtrlF();
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
                        continue;
                    }

                    // Front
                    WriteText(item.Key);
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

                    // Level
                    CtrlA();
                    WriteText(note.Level);
                    ClickKey(VirtualKeyCode.TAB);

                    // Band
                    CtrlA();
                    WriteText(note.Band);
                    ClickKey(VirtualKeyCode.TAB);

                    // Frequency
                    CtrlA();
                    WriteText(note.Frequency.ToString());
                    ClickKey(VirtualKeyCode.TAB);

                    // American Phonetic
                    CtrlA();
                    WriteText(note.AmericanPhonetic);
                    ClickKey(VirtualKeyCode.TAB);

                    // British Phonetic
                    CtrlA();
                    WriteText(note.BritishPhonetic);
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
                    ClickKey(VirtualKeyCode.RIGHT);
                    ClickKey(VirtualKeyCode.SPACE);
                    WriteText(note.Sentence);
                    ClickKey(VirtualKeyCode.TAB);

                    // Persian
                    //CtrlA();
                    //WriteText(note.Persian);
                    ClickKey(VirtualKeyCode.TAB);

                    // Collocation
                    CtrlA();
                    WriteText(GetListOf(note.Collocations), false);
                    ClickKey(VirtualKeyCode.TAB);

                    // Synonyms
                    CtrlA();
                    WriteText(GetListOf(note.Synonyms), false);
                    ClickKey(VirtualKeyCode.TAB);

                    // Antonyms
                    CtrlA();
                    WriteText(GetListOf(note.Antonyms), false);
                    ClickKey(VirtualKeyCode.TAB);
                    
                    // Verb
                    if (note.Verb != null)
                    {
                        CtrlA();
                        WriteText(note.Verb);
                    }
                    ClickKey(VirtualKeyCode.TAB);
                    
                    // Noun
                    if (note.Noun != null)
                    {
                        CtrlA();
                        WriteText(note.Noun);
                    }
                    ClickKey(VirtualKeyCode.TAB);
                    
                    // Adjective
                    if (note.Adjective != null)
                    {
                        CtrlA();
                        WriteText(note.Adjective);
                    }
                    ClickKey(VirtualKeyCode.TAB);
                    
                    // Adverb
                    if (note.Adverb != null)
                    {
                        CtrlA();
                        WriteText(note.Adverb);
                    }

                    // Tags
                    CtrlShiftT();
                    foreach (var tag in note.Categories)
                    {
                        WriteText(tag);
                        ClickKey(VirtualKeyCode.RETURN);
                    }
                    remaining--;
                    
                    dictionary.Remove(item.Key);
                    await JsonFileHandler.SaveToJsonFileAsync(dictionary, "cardsInNeed.json");
                    
                    var savedNotes = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("saved.json");
                    savedNotes?.RemoveAll(x => x.Text.ToLower().Equals(item.Key.ToLower()));
                    await JsonFileHandler.SaveToJsonFileAsync(savedNotes, "saved.json");

                    Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length,50)}\t✓\trem:{remaining}");
                }
                catch (Exception e)
                {
                    await Utility.SaveAnError("Line 660 in Controller", e);
                    continue;
                }
            }
        }
    }
}
