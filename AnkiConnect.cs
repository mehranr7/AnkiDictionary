using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AnkiDictionary
{
    public static class AnkiConnect
    {
        /// <summary>
        /// Send request using Anki Connect to add new note
        /// </summary>
        /// <param name="note">the Json object contains data for the card</param>
        /// <param name="tags">a list of tags for the card</param>
        /// <param name="deckName">the name of the destined deck</param>
        /// <param name="modelName">the name of the destined model</param>
        /// <param name="newTag">the specific tag for new cards</param>
        /// <returns></returns>
        public static async Task AddNewNote(JObject note, List<string> tagList, string deckName, string modelName, string mainField)
        {
            (string fields, string tags) = JObjectToString(note, tagList);

            var parameterList = new List<string>();
            foreach (var parameter in note)
                parameterList.Add(parameter.Key.ToString());

            Console.Write($"> {note[mainField]}");

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
                        ""deckName"": """+deckName+@""",
                        ""modelName"": """+modelName+@""",
                        ""fields"": "+fields+@",
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
                            "+tags+@"
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
                note["noteID"]= res.result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Utility.PrintSpaces(note[mainField].ToString().Length,50)}\tError.\t✓");
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine($"{Utility.PrintSpaces(note[mainField].ToString().Length,50)}\tAdded.\t✓");
            

            var database = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("database.json") ?? new List<JObject>();
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
    
        /// <summary>
        /// Convert JObject to the string for Anki Connect
        /// </summary>
        /// <param name="note">The Json Object contains parameters for new Anki note</param>
        /// <returns>The string contains field parameters getting from input surrended with brackets</returns>
        private static (string, string) JObjectToString(JObject note, List<string> tagList)
        {
            var tags = "";
            var resutl = "{\n";

            foreach (var tag in tagList)
                tags += "\"" + tag + "\",\n";

            if (note.Count > 0)
            {
                foreach (var item in note)
                {
                    if (item.Key.ToLower() == "tag" || item.Key.ToLower() == "tags")
                    {
                        if (item.Value.ToString().Contains('"'))
                        {
                            tags += "\"" + Utility.ReplaceDetectedList(item.Value.ToString(), false) + "\",\n";
                        }
                        else
                        {
                            var tList = item.Value.ToString().Split(",");
                            foreach (var t in tList)
                            {
                                tags += "\"" + Utility.ReplaceDetectedList(t, false) + "\",\n";
                            }
                        }
                        continue;
                    }
                    resutl += "\"" + item.Key + "\" : \"" + Utility.ReplaceDetectedList(item.Value.ToString(),true) + "\",\n";
                }

                tags = tags.Substring(0, tags.Length - 2);
                resutl =  resutl.Substring(0, resutl.Length-2);
            }
            resutl += "}";

            return (resutl, tags);
        }
    }
}
