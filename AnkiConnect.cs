using Newtonsoft.Json;
using RestSharp;

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

            Console.Write($"‣ {note.Front}");

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
                            ""Front"": """+Utility.ReplaceSpaces(note.Front)+@""",
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
            
            dynamic res;

            try
            {
                res = JsonConvert.DeserializeObject<dynamic>(response.Content);
                note.NoteId = res.result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Utility.PrintSpaces(note.Front.Length,50)}\tError.\t✓");
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine($"{Utility.PrintSpaces(note.Front.Length,50)}\tAdded.\t✓");
            

            var database = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("database.json") ?? new List<AnkiNote>();
            database.Add(note);
            await JsonFileHandler.SaveToJsonFileAsync(database, "database.json");

        }

        public static async Task<List<long>> FindNotes(string deckName)
        {
            var options = new RestClientOptions()
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:8765", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
" + "\n" +
                       @"    ""action"": ""findNotes"",
" + "\n" +
                       @"    ""version"": 6,
" + "\n" +
                       @"    ""params"": {
" + "\n" +
                       $@"        ""query"": ""deck:{deckName}""
" + "\n" +
                       @"    }
" + "\n" +
                       @"}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            var responseList = new List<long>();
            try
            {
                var Obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                foreach (var item in Obj.result)
                {
                    responseList.Add((long)item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return responseList;
        }

        public static async Task CardsInfo(List<long> cards)
        {
            string cardsString = "[";
            foreach (var card in cards)
            {
                cardsString += card + ",";
            }

            cardsString = cardsString.Substring(0, cardsString.Length-1);
            cardsString += "]";

            var options = new RestClientOptions()
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:8765", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
" + "\n" +
                       @"    ""action"": ""cardsInfo"",
" + "\n" +
                       @"    ""version"": 6,
" + "\n" +
                       @"    ""params"": {
" + "\n" +
                       @"        ""cards"": "+cardsString+@"
" + "\n" +
                       @"    }
" + "\n" +
                       @"}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);

            var result = "";
            try
            {
                var deserializedObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var x = deserializedObject.result[0].fields.Front.value;
                foreach (var item in deserializedObject.result)
                {
                    try
                    {
                        result+=item.fields.Front.value.ToString()+", ";
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            result = result.Substring(0, result.Length-2);
            ClipboardManager.SetText(result);
        }
    }
}
