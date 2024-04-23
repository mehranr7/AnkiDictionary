namespace AnkiDictionary
{
    public static class ProgramHandler
    {
        public static string AskOptions()
        {
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Ask Gemini then copy note(s)");
            Console.WriteLine("2. Give me note(s) to add them to Anki");
            Console.WriteLine("3. Exit\n");
            return Console.ReadKey().KeyChar.ToString();
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
            Console.WriteLine("\n____________\n");
            Console.Write("Looking for Anki window.");
            var ankiWindow = WindowsManager.GetTitleThatContains("- anki");
            while (ankiWindow==null)
            {
                Console.Write(".");
                ankiWindow = WindowsManager.GetTitleThatContains("- anki");
            }

            Console.WriteLine("\n Founded. Please do Not close it.");

            Console.WriteLine("\n____________\n");
            Console.Write("Focusing on Anki window.");
            var isAnkiOpened = ControllerSimulator.OpenNewWindow(ankiWindow);
            while (!isAnkiOpened)
            {
                Console.Write(".");
                isAnkiOpened = ControllerSimulator.OpenNewWindow(ankiWindow);
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
    }
}
