namespace AnkiDictionary
{
    public static class Utility
    {
        public static string FixFrontText(string front)
        {
            if(string.IsNullOrEmpty(front))
                return front;

            var original = front;
            try
            {
                while (front[0] == ' ')
                {
                    front = front.Substring(1, front.Length-1);
                }
                front = front.Replace("\n", "");
                front = front.Replace("\r", "");
                while (front[^1] == ' ')
                {
                    front = front.Substring(0, front.Length - 1);
                }
                front = front[0].ToString().ToUpper() + front.Substring(1, front.Length - 1);
                
                return front;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return original;
            }
        }

        public static string PrintSpaces(int prevWordLength, int maxWordLength=50)
        {
            string spaces = "";
            for (int i = 0; i < maxWordLength - prevWordLength; i++)
                spaces += " ";
            return spaces;
        }
        
        public static string AskOptions(bool isAsked)
        {
            if (!isAsked)
            {
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("1. Ask Gemini then copy note(s)");
                Console.WriteLine("2. Give me note(s) to add them to Anki");
                Console.WriteLine("3. Find needed items");
                Console.WriteLine("4. Update needed items using Gemini");
                Console.WriteLine("5. Update needed items using local saved cards");
                Console.WriteLine("6. Export needed items as a list");
                Console.WriteLine("7. Update needed items using JSON");
                Console.WriteLine("8. Update Gemini introduction");
                Console.WriteLine("Esc. Exit\n");
            }
            return Console.ReadKey(true).KeyChar.ToString();
        }

        public static bool AskTrueFalseQuestion(string question)
        {
            Console.WriteLine("\n____________\n");
            Console.Write($"{question} 1=YES, 0=NO ");
            var answer = Console.ReadKey().KeyChar.ToString();
            while (answer is not ("1" or "0"))
            {
                return AskTrueFalseQuestion(question);
            }
            return answer == "1";
        }

        public static int AskAnInteger(string question)
        {
            Console.WriteLine("\n____________\n");
            Console.Write(question+" ");
            var res = -1;
            while (true)
            {
                try
                {
                    res = Int32.Parse(Console.ReadLine()!);
                }
                catch (Exception e)
                {
                    continue;
                }
                break;
            }
            return res;
        }

        public static string AskAString(string question)
        {
            Console.WriteLine("\n____________\n");
            Console.WriteLine(question);
            var res = Console.ReadLine();
            while (res == null )
            {
                res = Console.ReadLine();
            }
            return res;
        }

        public static void DrawProgressBar(int progress, int total)
        {
            int barLength = 50; // Length of the progress bar
            double percentComplete = (double)progress / total;
            int progressBarFilled = (int)(percentComplete * barLength);

            Console.CursorLeft = 0; // Move the cursor to the beginning of the line
            Console.Write("["); // Start of the progress bar

            // Draw the filled part of the progress bar
            for (int i = 0; i < progressBarFilled; i++)
            {
                Console.Write("#");
            }

            // Draw the unfilled part of the progress bar
            for (int i = progressBarFilled; i < barLength; i++)
            {
                Console.Write("-");
            }

            Console.Write("] ");

            // Display the percentage complete
            Console.Write($"{percentComplete:P0}");
        }
        
        public static void ClearLastLines(int numberOfLines)
        {
            int currentLineCursor = Console.CursorTop;
        
            // Move the cursor to the line where clearing should start
            int clearStartLine = currentLineCursor - numberOfLines;
            if (clearStartLine < 0) clearStartLine = 0;

            for (int i = 0; i < numberOfLines; i++)
            {
                // Move the cursor to the start of the line
                Console.SetCursorPosition(0, clearStartLine + i);
                // Clear the line
                Console.Write(new string(' ', Console.WindowWidth));
            }

            // Move the cursor back to where it was originally
            Console.SetCursorPosition(0, clearStartLine);
        }

        public static async Task SaveAnError(string errorText, Exception? e = null)
        {
            e ??= new Exception();
            var error = new Dictionary<string, Exception> { { errorText, e } };
            var preErrors = await JsonFileHandler.ReadFromJsonFileAsync<List<Dictionary<string, Exception>>>("errors.json");
            preErrors!.Add(error);
            await JsonFileHandler.SaveToJsonFileAsync(preErrors, "errors.json");
        }
    }
}
