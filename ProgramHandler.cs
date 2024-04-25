namespace AnkiDictionary
{
    public static class ProgramHandler
    {
        public static string AskOptions()
        {
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Ask Gemini then copy note(s)");
            Console.WriteLine("2. Give me note(s) to add them to Anki");
            Console.WriteLine("3. Separate Image and Pronunciation fields and validation");
            Console.WriteLine("4. Update dictionary items using JSON");
            Console.WriteLine("5. Export cards which need information");
            Console.WriteLine("Esc. Exit\n");
            return Console.ReadKey(true).KeyChar.ToString();
        }
        
        public static async Task Introduction(string? introduction, GeminiDictionaryConvertor geminiDictionaryConvertor)
        {
            // Introduction
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Introducing..");
            Console.WriteLine(await geminiDictionaryConvertor.MakeAnIntroduction(introduction));
            Console.WriteLine("Done.");
        }
        
        public static async Task<List<AnkiNote>> AskGeminiAnkiNotes(string words, GeminiDictionaryConvertor geminiDictionaryConvertor)
        {
            // Asking
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Asking from Gemini for the dictionary.");
            var ankiNotes = await geminiDictionaryConvertor.GeminiTransformer(words);

            Console.WriteLine("\n____________\n");
            Console.WriteLine("Got it! All notes copied to the clipboard.");

            return ankiNotes;
        }
        
        public static void StartAddingNotes(List<AnkiNote> notes)
        {
            // Fire button
            Console.WriteLine("\n____________\n");
            Console.WriteLine("I'm ready! Press any key to start adding new notes.");
            Console.ReadKey();
            
            // Going for Anki window
            var ankiWindow = FindAnkiWindow();

            Console.WriteLine("\n____________\n");
            Console.Write("Focusing on Anki and Opening Add new window.");
            var isAnkiOpened = ControllerSimulator.OpenAddNewWindow(ankiWindow);
            while (!isAnkiOpened)
            {
                Console.Write(".");
                isAnkiOpened = ControllerSimulator.OpenAddNewWindow(ankiWindow);
            }
            Console.WriteLine("\n Focused. Please be patient until it finish.");
            
            // Adding
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Adding new notes.");
            foreach (var note in notes)
            {
                Console.WriteLine(note.Text);
                ControllerSimulator.AddNewNote(note);
            }
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Done.");
        }

        private static string FindAnkiWindow()
        {
            Console.WriteLine("\n____________\n");
            Console.Write("Looking for Anki window.");
            var ankiWindow = WindowsManager.GetTitleThatContains("- anki");
            while (ankiWindow==null)
            {
                Console.Write(".");
                ankiWindow = WindowsManager.GetTitleThatContains("- anki");
            }
            Console.WriteLine("\nFounded. Please do Not close it.");
            return ankiWindow;
        }

        public static void SeparateImageAndPronunciation(int recordCount, int skips, string? filter = null)
        {
            var ankiWindow = FindAnkiWindow();
            
            Console.WriteLine("\n____________\n");
            Console.Write("Focusing on Anki and Opening Browse window.");
            var isAnkiOpened = ControllerSimulator.OpenBrowseWindow(ankiWindow);
            while (!isAnkiOpened)
            {
                Console.Write(".");
                isAnkiOpened = ControllerSimulator.OpenBrowseWindow(ankiWindow);
            }
            Console.WriteLine("\nFocused. Please be patient until it finish.");
            Console.WriteLine("\n____________\n");

            ControllerSimulator.StartSeparatingParts(recordCount, skips, filter);
        }

        public static void UpdateNotes(List<AnkiNote> ankiNotes)
        {
            var ankiWindow = FindAnkiWindow();
            
            Console.WriteLine("\n____________\n");
            Console.Write("Focusing on Anki and Opening Browse window.");
            var isAnkiOpened = ControllerSimulator.OpenBrowseWindow(ankiWindow);
            while (!isAnkiOpened)
            {
                Console.Write(".");
                isAnkiOpened = ControllerSimulator.OpenBrowseWindow(ankiWindow);
            }
            Console.WriteLine("\n Focused. Please be patient until it finish.");

            ControllerSimulator.OpenBrowseWindow(ankiWindow);

            ControllerSimulator.UpdateNotes(ankiNotes);
        }
    }
}
