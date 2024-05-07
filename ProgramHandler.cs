namespace AnkiDictionary
{
    public static class ProgramHandler
    {
        public static string AskOptions(bool isAsked)
        {
            if (!isAsked)
            {
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("1. Ask Gemini then copy note(s)");
                Console.WriteLine("2. Give me note(s) to add them to Anki");
                Console.WriteLine("3. Separate Image and Pronunciation fields and validation");
                Console.WriteLine("4. Update dictionary items using JSON");
                Console.WriteLine("5. Export cards which need information");
                Console.WriteLine("Esc. Exit\n");
            }
            return Console.ReadKey(true).KeyChar.ToString();
        }
        
        public static async Task Introduction(string? introduction, GeminiDictionaryConvertor geminiDictionaryConvertor)
        {
            // Introduction
            await geminiDictionaryConvertor.MakeAnIntroduction(introduction);
        }
        
        public static async Task<List<AnkiNote>> AskGeminiAnkiNotes(string words, GeminiDictionaryConvertor geminiDictionaryConvertor)
        {
            // Asking
            var ankiNotes = await geminiDictionaryConvertor.AskUntilCoverList(words);

            return ankiNotes;
        }
        
        public static void StartAddingNotes(List<AnkiNote> notes)
        {
            // Fire button
            Console.WriteLine("\n____________\n");
            Console.WriteLine("I'm ready! Press any key to start adding new notes.");
            Console.ReadKey();
            
            // Going for Anki window
            ControllerSimulator.OpenAddNewWindow();
            
            // Adding
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Adding new notes.");
            foreach (var note in notes)
            {
                ControllerSimulator.AddNewNote(note);
            }
        }


        public static void SeparateImageAndPronunciation(int recordCount, int skips, string? filter = null, bool doubleDown = false)
        {
            ControllerSimulator.OpenBrowseWindow();
            ControllerSimulator.StartSeparatingParts(recordCount, skips, filter, doubleDown);
        }

        public static void UpdateNotes(List<AnkiNote> ankiNotes)
        {
            ControllerSimulator.OpenBrowseWindow();
            ControllerSimulator.UpdateNotes(ankiNotes);
        }
    }
}
