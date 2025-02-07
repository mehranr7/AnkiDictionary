﻿using System.Diagnostics;
using System.Text.Json;

namespace AnkiDictionary
{
    public static class Utility
    {
        public static string FixText(string input)
        {
            if(string.IsNullOrEmpty(input))
                return input;

            var original = input;
            try
            {
                // remove the space at the beginning
                while (input[0] == ' ')
                {
                    input = input.Substring(1, input.Length-1);
                }

                // preventing error by removing "
                input = input.Replace("\"", "'");


                // Capitalize the first letter
                while (input[^1] == ' ')
                {
                    input = input.Substring(0, input.Length - 1);
                }
                input = input[0].ToString().ToUpper() + input.Substring(1, input.Length - 1);
                
                return input;
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
                Console.WriteLine("9. Copy All Card's Front");
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
        
        public static string GetListOf(List<string> list)
        {
            if (list == null || list.Count == 0) return "";
            var output = "";
            foreach (var collocation in list)
            {
                if(output.Length > 0)
                    output += "<br>";
                if(collocation.Length > 0)
                    output += $@"- {FixText(collocation)}";
            }

            return output;
        }

        public static string ReplaceSpaces(string? input)
        {
            if (input == null)
                return "";
            input = input.Replace("\r", "");
            input = input.Replace("\n", "<br>");
            input = input.Replace(Environment.NewLine, "<br>");
            return input;
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
    
        public static string ReplaceDetectedList(string input, bool isForFields)
        {
            if (input.StartsWith("[") && input.EndsWith("]"))
            {
                try
                {
                    List<string>? list = JsonSerializer.Deserialize<List<string>>(input);
                    if (list != null)
                    {
                        if (isForFields)
                            return GetListOf(list);

                        var output = "";
                        foreach (var item in list)
                            output += "\"" + FixText(item) + "\",\n";
                        output = output.Substring(0, output.Length - 2);
                        return output;
                    }
                    else
                    {
                        return FixText(input);
                    }
                }
                catch
                {
                    return FixText(input);
                }
            }
            else
            {
                return FixText(input);
            }
        }
    }
}
