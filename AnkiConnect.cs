using System.Net.WebSockets;
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
        /// <param name="tagList">a list of tags for the card</param>
        /// <param name="deckName">the name of the destined deck</param>
        /// <param name="modelName">the name of the destined model</param>
        /// <param name="mainField">the main field (usually front)</param>
        /// <returns></returns>
        public static async Task AddNewNote(JObject note, List<string> tagList, string deckName, string modelName, string mainField)
        {
            (string fields, string tags) = JObjectToString(note, tagList);

            var parameterList = new List<string>();
            foreach (var parameter in note)
                parameterList.Add(parameter.Key.ToString());

            Console.Write($"> {note[mainField]}");

            var options = new RestClientOptions();
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

        /// <summary>
        /// Send request using Anki Connect to update note
        /// </summary>
        /// <param name="id">the id of the card to update</param>
        /// <param name="note">the Json object contains data for the card</param>
        /// <param name="tagList">a list of tags for the card</param>
        /// <param name="deckName">the name of the destined deck</param>
        /// <param name="modelName">the name of the destined model</param>
        /// <param name="mainField">the main field (usually front)</param>
        /// <returns></returns>
        public static async Task UpdateNote(string id, JObject note, List<string> tagList, string mainField)
        {
            (string fields, string tags) = JObjectToString(note, tagList);

            var parameterList = new List<string>();
            foreach (var parameter in note)
                parameterList.Add(parameter.Key.ToString());

            Console.Write($"> {note[mainField]}");

            var options = new RestClientOptions();
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:8765", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var body =
            @"{
                ""action"": ""updateNote"",
                ""version"": 6,
                ""params"": {
                    ""note"": {
                        ""id"": " + id + @",
                        ""fields"": " + fields + @",
                        ""tags"": [
                            " + tags + @"
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
                note["noteID"] = res.result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Utility.PrintSpaces(note[mainField].ToString().Length, 50)}\tError.\t✓");
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine($"{Utility.PrintSpaces(note[mainField].ToString().Length, 50)}\tUpdated.\t✓");


            var database = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("database.json") ?? new List<JObject>();
            database.Add(note);
            await JsonFileHandler.SaveToJsonFileAsync(database, "database.json");

        }

        /// <summary>
        /// Find list of note IDs with filters
        /// </summary>
        /// <param name="query">the query to filter in Anki</param>
        /// <param name="max">the max number of note IDs to return</param>
        /// <returns>The list of note IDs in the form of string spearated with comma</returns>
        public static async Task<string> FindNotes(string query, int max = int.MaxValue)
        {
            var options = new RestClientOptions();
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
                       $@"        ""query"": ""{query.Replace("\"","\\\"")}""
" + "\n" +
                       @"    }
" + "\n" +
                       @"}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            var responseList = "";
            try
            {
                var Obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                if(Obj["result"] != null)
                {
                    foreach (var item in Obj["result"])
                    {
                        if (max <= 0)
                            break;
                        responseList += item.ToString() + ", ";
                        max--;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            responseList = responseList.Substring(0, responseList.Length - 2);
            return responseList;
        }

        /// <summary>
        /// Get the noteId and the mainField of note(s)
        /// </summary>
        /// <param name="noteIDs">a list of note IDs in the form of string separated by comma</param>
        /// <param name="mainField">the main field of the note type</param>
        /// <returns>List of strings containing NoteId and mainField separated by comma</returns>
        public static async Task<Stack<string>> NotesInfo(string noteIDs, string mainField)
        {

            var options = new RestClientOptions();
            var client = new RestClient(options);
            var request = new RestRequest("http://localhost:8765", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
" + "\n" +
                       @"    ""action"": ""notesInfo"",
" + "\n" +
                       @"    ""version"": 6,
" + "\n" +
                       @"    ""params"": {
" + "\n" +
                       @"        ""notes"": [" + noteIDs + @"]
" + "\n" +
                       @"    }
" + "\n" +
                       @"}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);

            var result = new Stack<string>();
            try
            {
                var deserializedObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var x = deserializedObject.result[0].fields.Front.value;
                foreach (var item in deserializedObject.result)
                {
                    try
                    {
                        result.Push(item.fields[mainField].value.ToString() + ","+ item["noteId"].ToString());
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
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
                            var t = item.Value.ToString();
                            t = t.Replace("["," ");
                            t = t.Replace("]"," ");
                            t = t.Replace("\n"," ");
                            t = t.Replace("\r"," ");
                            t = t.Replace("\""," ");
                            if(t.Contains(","))
                            {
                                foreach (var tg in t.Split(','))
                                {
                                    tags += "\"" + Utility.ReplaceDetectedList(tg, false) + "\",\n";
                                }
                            }
                            else
                            {
                                tags += "\"" + Utility.ReplaceDetectedList(t, false) + "\",\n";
                            }
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
