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

        private const string Introduction = @"I want you to act as dictionary. I'll provide a text. give me in order the following things :

Text : the provided text
Type : the type of the provided text
Usage : the state of being formal or informal of the provided text
Definition : explain in a simple way the definition of the provided text
Image : a link of an image related to the provided text
Sentence: a sentence to learn the provided text in the context
Persian : Persian translate of the provided text
Collocations : list of top collocations of the provided text
Synonyms : synonyms of the provided text
Antonyms : antonyms of the provided text

Consider the word or phrase which I'll send as a one single item which should be provided the above list for it but consider that if it has multiple meaning provide all items of above list for each meaning. it's really important which you mustn't separate the provided phrase of word into multiple words while you want to provide above list for it and you should consider to not break the phrase into words in order to provide definition. It's really important that you have to create the sentence using the most used collocation of it and the context shouldn't provide the meaning or definition or explanation of it. I want you to use creation just like a native speaker. consider that the Persian translate must be created or matched from the definition in a very short form (a few words) of it. 
Consider when I separate a word or a group of words using comma It means you should create an answer for each of those.
Consider Whole answer must be in form of JSON.
consider when I separate a word or a group of words using comma the answer must be multiple JSON.
";
        
        public GeminiDictionaryConvertor(string apiKey)
        {
            _client = new GeminiClient(apiKey);
            _conversation = new ChatConversation();
            _completionOptions = new ChatCompletionOptions()
            {
                Model = ChatCompletionModels.Gemini.GeminiPro
            };
        }

        public async Task<string> MakeAnIntroduction(string? introduction)
        {
            introduction = string.IsNullOrEmpty(introduction) ? Introduction : introduction;
            return await AskGemini(introduction);

        }
        
        private async Task<string> AskGemini(string question)
        {
            Console.WriteLine("\n____________\n");
            Console.WriteLine($"Sending Message Below:\n{question}");
            var chatQuestion = new ChatMessage()
            {
                Content = question
            };
            _conversation.Messages.Add(chatQuestion);
            var response = await _client.CompleteAsync(_conversation,_completionOptions);
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