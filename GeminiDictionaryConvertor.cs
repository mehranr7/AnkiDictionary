using ChatAIze.GenerativeCS.Constants;
using ChatAIze.GenerativeCS.Clients;
using ChatAIze.GenerativeCS.Models;
using ChatAIze.GenerativeCS.Options.Gemini;
using Newtonsoft.Json;

namespace AnkiDictionary
{
    public class GeminiDictionaryConvertor
    {
        private readonly ChatCompletionOptions _completionOptions;
        private readonly GeminiClient _client;
        private readonly ChatConversation _conversation;
        private readonly string _introduction;
        
        public GeminiDictionaryConvertor(string apiKey, string introduction)
        {
            _introduction = introduction;
            _client = new GeminiClient(apiKey);
            _conversation = new ChatConversation();
            _completionOptions = new ChatCompletionOptions()
            {
                Model = ChatCompletionModels.Gemini.GeminiPro
            };
        }

        public async Task<string> MakeAnIntroduction(string? introduction)
        {
            introduction = string.IsNullOrEmpty(introduction) ? _introduction : introduction;
            return await AskGemini(introduction);

        }
        
        private async Task<string> AskGemini(string question, bool isEssential = true)
        {
            Console.WriteLine("\n____________\n");
            Console.WriteLine($"Sending Message Below:\n{question}");
            var failureCounter = 0;
            var response = "";
            var chatQuestion = new ChatMessage()
            {
                Content = question
            };
            _conversation.Messages.Add(chatQuestion);
            while (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Please wait...");
                try
                {
                    response = await _client.CompleteAsync(_conversation,_completionOptions);
                    Console.WriteLine("\n____________\n");
                }
                catch (Exception e)
                {
                    if (e.Message.ToLower().Contains("key"))
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("We've reached Gemini's limit.");
                        Console.Write("Counting down ");
                        for (var i = 0; i < 5; i++)
                        {
                            Console.Write(i+1);
                            Thread.Sleep(1000);  
                        }
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine($"A problem posed while asking Gemini.\n{e}");
                    }

                    if (failureCounter > 2 && !isEssential)
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Failed to get a response!");
                        break;
                    }

                    failureCounter++;

                }
            }
            if(!string.IsNullOrEmpty(response))
                Console.WriteLine($"Response is:\n {response}");
            Console.WriteLine("\n____________\n");
            return response;
        }

        public static List<AnkiNote> StringToAnkiNotes(string input)
        {
            input += "     ";
            var ankiNotes = new List<AnkiNote>();
            try
            {
                var start = input.IndexOf("{", StringComparison.Ordinal);
                var end = input.IndexOf("}", StringComparison.Ordinal);
                while (start >= 0 && end >= 0)
                {
                    var ankiString = input.Substring(start, end - start + 1);
                    try
                    {
                        var note = JsonConvert.DeserializeObject<AnkiNote>(ankiString);
                        ankiNotes.Add(note);
                        start = input.IndexOf("{", end, StringComparison.Ordinal);
                        end = input.IndexOf("}", end + 1, StringComparison.Ordinal);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error :\n{e}\nConfronted a problem while converting the string below :\n{ankiString}");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error :\n{e}\nConfronted a problem at the beginning of converting this input :\n{input}");
            }
            return ankiNotes;
        }

        public async Task<List<AnkiNote>> AskUntilCoverList(string askedString)
        {
            var notes = new List<AnkiNote>();
            var askedListInvalid = askedString.Split(",").ToList();
            var askedList = new List<string>();
            foreach (var note in askedListInvalid)
            {
                var fixedNote = Utility.FixFrontText(note);
                askedList.Add(fixedNote);
            }
            var notesStringToAsk = String.Join(", ", askedList.ToArray());
            var remainingNotes = askedList.Count;
            
            Console.WriteLine("\n____________\n");
            Console.WriteLine($"Start Asking {askedList.Count} notes");
            var carryOn = true;
            var notesString = "";
            while (remainingNotes>0 && carryOn && !string.IsNullOrEmpty(notesStringToAsk))
            {
                var newNotesString = await AskGemini(notesStringToAsk);
                notesString += newNotesString;
                ClipboardManager.SetText(notesString);
                var newNotes = StringToAnkiNotes(newNotesString);
                foreach (var newNote in newNotes)
                {
                    askedList.Remove(Utility.FixFrontText(newNote.Text));
                    notes.Add(newNote);
                    Console.WriteLine($"‣ {newNote.Text}{Utility.PrintSpaces(newNote.Text.Length)}Rem:{askedList.Count}");
                }
                remainingNotes = askedList.Count;
                notesStringToAsk = String.Join(", ", askedList.ToArray());
                Console.WriteLine("\n____________\n");
                Console.WriteLine($"Remaining : {notesStringToAsk}");
                if (string.IsNullOrEmpty(notesStringToAsk))
                {
                    carryOn = false;
                }
                else
                {
                    Console.WriteLine("Carry on asking? (1 means yes anything else means no)");
                    carryOn = Console.ReadKey().KeyChar.ToString() == "1";
                }
            }
            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            return notes;
        }
    }
}