using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
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

            Simulator.Keyboard.TextEntry(fixText ? Utility.FixText(text) : text);
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
        
        public static async Task AddNewNote(JObject note, List<string> tags)
        {

            Console.Write($"> {note.Properties().First().Name}");

            LongPause();

            foreach (var parameter in note)
            {
                // Front
                WriteText(parameter.Value.ToString());
                ClickKey(VirtualKeyCode.TAB);
            }
            
            // Tags
            CtrlShiftT();
            CtrlA();
            ClickKey(VirtualKeyCode.BACK);
            ClickKey(VirtualKeyCode.TAB);
            CtrlShiftT();
            foreach (var tag in tags)
            {
                WriteText(tag);
                ClickKey(VirtualKeyCode.RETURN);
            }

            // Ctrl + Enter
            CtrlEnter();
            
            Console.WriteLine($"{Utility.PrintSpaces(note.Properties().First().Name.Length)}\tAdded.\t✓");

            CtrlH();
            ClickKey(VirtualKeyCode.DOWN);
            ClickKey(VirtualKeyCode.RETURN);
            note["NoteID"] = ReadNoteId();
            ClickKey(VirtualKeyCode.ESCAPE);
            var database = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("database.json") ?? new List<JObject>();
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

        public static async Task UpdateNotes(List<JObject> ankiNotes, List<string> tagList)
        {
            var dictionary = await JsonFileHandler.ReadFromJsonFileAsync<Dictionary<string, string>>("cardsInNeed.json");
            if(dictionary  == null) return;

            var remaining = dictionary.Count;
                var counter = 0;
            foreach (var item in dictionary)
            {
                try
                {
                    var note = ankiNotes.FirstOrDefault(note => item.Key.ToLower().Contains(note.Properties().First().Name.ToLower()));
                    if (note == null)
                        continue;
                    var checker = Utility.FixText(note.Properties().First().Name.ToString()).Equals(Utility.FixText(item.Key));
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
                        Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length)}\t✓\tWRONG nid");
                        continue;
                    }

                    if (!duplicateCheckerText.ToLower().Equals(item.Key.ToLower()))
                    {
                        Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length)}\t✓\tDUPLICATE");
                        continue;
                    }


                    // 
                    foreach (var parameter in note)
                    {
                        // Front
                        WriteText(parameter.Value.ToString());
                        ClickKey(VirtualKeyCode.TAB);
                    }

                    // Tags
                    CtrlShiftT();
                    foreach (var tag in tagList)
                    {
                        WriteText(tag);
                        ClickKey(VirtualKeyCode.RETURN);
                    }
                    remaining--;
                    
                    dictionary.Remove(item.Key);
                    await JsonFileHandler.SaveToJsonFileAsync(dictionary, "cardsInNeed.json");
                    
                    var savedNotes = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json");
                    savedNotes?.RemoveAll(x => x["Front"].ToString().ToLower().Equals(item.Key.ToLower()));
                    await JsonFileHandler.SaveToJsonFileAsync(savedNotes, "saved.json");

                    Console.WriteLine($"{Utility.PrintSpaces(item.Key.Length)}\t✓\trem:{remaining}");
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
