using System.Text.Json;
using RestSharp;
using WindowsInput.Native;
namespace AnkiDictionary
{
    public static class AnkiConnect
    {
        public static async Task AddNewNote(AnkiNote note, List<string> tags, string deckName, string modelName)
        {
            var tagsString = @"""needSound""";
            foreach (var tag in tags)
            {
                tagsString += @",""" + tag + @"""";
            }

            Console.Write($"‣ {note.Text}");

            var options = new RestClientOptions()
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:8765", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var body = 
            @"{
                ""action"": ""addNote"",
                ""version"": 6,
                ""params"": {
                    ""note"": {
                        ""deckName"": """+Utility.ReplaceSpaces(deckName)+@""",
                        ""modelName"": """+Utility.ReplaceSpaces(modelName)+@""",
                        ""fields"": {
                            ""Front"": """+Utility.ReplaceSpaces(note.Text)+@""",
                            ""US"": """",
                            ""UK"": """",
                            ""TypeGroup"": """+Utility.ReplaceSpaces(note.Type)+@""",
                            ""Usage"": """+Utility.ReplaceSpaces(note.Usage)+@""",
                            ""Level"": """+Utility.ReplaceSpaces(note.Level)+@""",
                            ""Band"": """+Utility.ReplaceSpaces(note.Band)+@""",
                            ""Frequency"": """+Utility.ReplaceSpaces(note.Frequency.ToString())+@""",
                            ""American Phonetic"": """+Utility.ReplaceSpaces(note.AmericanPhonetic)+@""",
                            ""British Phonetic"": """+Utility.ReplaceSpaces(note.BritishPhonetic)+@""",
                            ""Definition"": """+Utility.ReplaceSpaces(note.Definition)+@""",
                            ""Image"": """",
                            ""Sentence"": """+Utility.ReplaceSpaces(note.Sentence)+@""",
                            ""Persian"": """+Utility.ReplaceSpaces(note.Persian)+@""",
                            ""Collocation"": """+Utility.ReplaceSpaces(Utility.GetListOf(note.Collocations))+@""",
                            ""Synonyms"": """+Utility.ReplaceSpaces(Utility.GetListOf(note.Synonyms))+@""",
                            ""Antonyms"": """+Utility.ReplaceSpaces(Utility.GetListOf(note.Antonyms))+@""",
                            ""Verb"": """+Utility.ReplaceSpaces(note.Verb)+@""",
                            ""Noun"": """+Utility.ReplaceSpaces(note.Noun)+@""",
                            ""Adjective"": """+Utility.ReplaceSpaces(note.Adjective)+@""",
                            ""Adverb"": """+Utility.ReplaceSpaces(note.Adverb)+@""",
                            ""DefinitionSound"": """"
                            },
                        ""options"": {
                            ""allowDuplicate"": false,
                            ""duplicateScope"": ""deck"",
                            ""duplicateScopeOptions"": {
                                ""deckName"": ""Default"",
                                ""checkChildren"": false,
                                ""checkAllModels"": false
                            }
                        },
                        ""tags"": [
                            "+tagsString+@"
                        ]
                    }
                }
            }";

            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            
            var res = new AnkiConnectResponse();

            try
            {
                res = JsonSerializer.Deserialize<AnkiConnectResponse>(response.Content!);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Utility.PrintSpaces(note.Text.Length,50)}\tError.\t✓");
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine($"{Utility.PrintSpaces(note.Text.Length,50)}\tAdded.\t✓");
            
            note.NoteId =res!.result.ToString();

            var database = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("database.json") ?? new List<AnkiNote>();
            database.Add(note);
            await JsonFileHandler.SaveToJsonFileAsync(database, "database.json");

        }
        
    }
}
