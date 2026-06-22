
using GenerativeAI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnkiDictionary
{
    public class GeminiDictionaryConvertor
    {
        private readonly GenerativeModel _model;
        private readonly ChatSession _chat;
        private readonly GoogleAi _client;
        private readonly string _introduction;
        private readonly string _modelName;
        private readonly int _coolDown;
        private readonly List<int> _regulationList;
        private readonly int _maximumRequests;
        private int _regularAnswerCount;
        private bool _clearLine;
        private int _geminiRequestCounter;
        private string _mainField;
        private Dictionary<string, string> _dataObject;
        private List<string> _parameterNameList;

        public GeminiDictionaryConvertor(string apiKey, string introduction, string modelName)
        {
            _introduction = introduction;
            _modelName = modelName;
            _client = new GoogleAi(apiKey);
            _model = _client.CreateGenerativeModel(modelName);
            _chat = _model.StartChat();
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            _coolDown = Int32.Parse(config["Gemini:CoolDown"]!);
            _maximumRequests = Int32.Parse(config["Gemini:MaximumRequests"]!);
            _regularAnswerCount = Int32.Parse(config["Gemini:RegularAnswerCount"]!);
            _regulationList = new List<int>();
            _clearLine = false;
            _geminiRequestCounter = 0;
            _mainField = config["DynamicObject:MainField"]!;
            _dataObject = config.GetSection("DynamicObject:Object").Get<Dictionary<string, string>>()!;
            _parameterNameList = new List<string>();
            foreach (var item in _dataObject!)
                _parameterNameList.Add(item.Key);
        }

        private void CoolDown()
        {
            if (_clearLine)
            {
                Utility.ClearLastLines(3);
                _clearLine = false;
            }
            Console.WriteLine("\nCooling down");
            for (var i = 0; i < _coolDown; i++)
            {
                Utility.DrawProgressBar(i+1,_coolDown);
                Thread.Sleep(1000);
            }
            Console.WriteLine();
            Utility.ClearLastLines(3);
        }

        public async Task<string> MakeAnIntroduction(string? introduction)
        {
            introduction = string.IsNullOrEmpty(introduction) ? _introduction : introduction;

            Console.WriteLine("\n____________\n");
            Console.WriteLine(introduction);

            var response = await AskGemini(introduction);

            Console.WriteLine("\n____________\n");
            Console.WriteLine(response);

            return response;

        }
        
        private async Task<string> AskGemini(string question)
        {
            if (_geminiRequestCounter > _maximumRequests)
            {
                CoolDown();
                _geminiRequestCounter = 0;
            }

            var failureCounter = 0;
            var response = "";
            while (string.IsNullOrEmpty(response))
            {
                try
                {
                    _geminiRequestCounter++;
                    var res = await _chat.GenerateContentAsync(question);
                    response = res.Text();
                }
                catch (Exception e)
                {
                    if (e.Message.ToLower().Contains("many") 
                             || e.Message.ToLower().Contains("key") 
                             || e.Message.ToLower().Contains("500") 
                             || e.Message.ToLower().Contains("429"))
                    {

                        if (failureCounter > 4)
                        {
                            Console.WriteLine("\n____________\n");
                            Console.WriteLine("Failed to get a response!");
                            Console.WriteLine("\n____________\n");
                            _clearLine = false;
                            break;
                        }

                        CoolDown();

                        failureCounter++;
                    }
                    else if (e.Message.ToLower().Contains("403"))
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Error 403 - Forbidden!");
                        Console.WriteLine("\n____________\n");
                        _clearLine = false;
                    }
                    else if (e.Message.ToLower().Contains("404"))
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Error 404 - Not Found!");
                        Console.WriteLine("\n____________\n");
                        _clearLine = false;
                    }
                    else
                    {

                        await Utility.SaveAnError(
                            "Line 125 - \n\"A problem posed while asking Gemini.", e);
                        _clearLine = false;

                        Console.WriteLine(e.Message);

                        if (failureCounter > 2)
                        {
                            Console.WriteLine("\n____________\n");
                            Console.WriteLine("Failed to get a response!");
                            Console.WriteLine("\n____________\n");
                            _clearLine = false;
                            break;
                        }

                        failureCounter++;
                    }

                }
            }
            
            return response;
        }

        public static async Task<List<JObject>> StringToAnkiNotes(string input)
        {
            input += "     ";
            var ankiNotes = new List<JObject>();
            try
            {
                var start = input.IndexOf("{", StringComparison.Ordinal);
                var end = input.IndexOf("}", StringComparison.Ordinal);
                while (start >= 0 && end >= 0)
                {
                    var ankiString = input.Substring(start, end - start + 1);
                    try
                    {
                        var note = JsonConvert.DeserializeObject<JObject>(ankiString);
                        ankiNotes.Add(note);
                        start = input.IndexOf("{", end, StringComparison.Ordinal);
                        end = input.IndexOf("}", end + 1, StringComparison.Ordinal);
                    }
                    catch (Exception e)
                    {
                        await Utility.SaveAnError(
                            $"Line 167 - \nConfronted a problem while converting the string below :\n{ankiString}", e);

                        start = input.IndexOf("{", end, StringComparison.Ordinal);
                        end = input.IndexOf("}", end + 1, StringComparison.Ordinal);
                    }
                }
            }
            catch (Exception e)
            {
                await Utility.SaveAnError(
                    $"Line 178 - \nConfronted a problem at the beginning of converting this input :\n{input}", e);
            }
            return ankiNotes;
        }

        public async Task<List<JObject>> AskUntilCoverList(string askedString, bool needConfirm = true, int preProcessedCount = 0, int total = 0)
        {
            if (needConfirm)
            {
                needConfirm = Utility.AskTrueFalseQuestion("Do you want me to get confirmation to carry on?");
                Console.WriteLine("\n____________\n");
            }

            var notes = new List<JObject>();
            var askedList = askedString.Split(",").Select(Utility.FixText).ToList();

            if(String.IsNullOrWhiteSpace(askedList.Last()))
                askedList.RemoveAt(askedList.Count-1);

            total = total==0 ? askedList.Count : total;
            var remListBuffer = new List<int> {  total-preProcessedCount };
            while (askedList.Count > _regularAnswerCount)
            {

                var bufferList = askedList.Take(_regularAnswerCount).ToList();
                var answerNotes = await AskUntilCoverList(String.Join(", ", bufferList), needConfirm, notes.Count, total);
                foreach (var answerNote in answerNotes)
                {
                    notes.Add(answerNote);
                    var front = Utility.FixText(answerNote[_mainField].ToString());
                    var exactMatch = askedList.FirstOrDefault(x => x.ToLower() == front.ToLower());
                    if (exactMatch != null)
                    {
                        askedList.Remove(exactMatch);
                    }
                    else
                    {
                        var closestMatch = askedList
                            .Select(x => new { Word = x, Distance = Utility.CalculateLevenshteinDistance(front.ToLower(), x.ToLower()) })
                            .OrderBy(x => x.Distance)
                            .FirstOrDefault();

                        if (closestMatch != null && closestMatch.Distance <= (closestMatch.Word.Length / 3) + 1)
                        {
                            askedList.Remove(closestMatch.Word);
                        }
                    }
                }

                var carryOnBuffering = true;
                if(needConfirm)
                    carryOnBuffering = Utility.AskTrueFalseQuestion($"Carry on asking more {_regularAnswerCount} cards?");
                

                if (!carryOnBuffering)
                {
                    var preNotesList = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json") ?? new List<JObject>();
                    preNotesList.AddRange(notes);
                    await JsonFileHandler.SaveToJsonFileAsync(preNotesList, "saved.json");
                    return notes;
                }

                remListBuffer.Add(askedList.Count);

                if(remListBuffer.Count > 2)
                    if (remListBuffer[^1] == askedList.Count
                        && remListBuffer[^2] == askedList.Count
                        && remListBuffer[^3] == askedList.Count)
                    {
                        
                        var preNotesList = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json") ?? new List<JObject>();
                        preNotesList.AddRange(notes);
                        await JsonFileHandler.SaveToJsonFileAsync(preNotesList, "saved.json");

                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Ops! there's a problem with remaining or Gemini introduction.");
                        Console.WriteLine("Let's let the remaining slide");
                        return notes;
                    }
            }

            var notesStringToAsk = String.Join(", ", askedList.ToArray());
            var remList = new List<int> { askedList.Count };

            while (askedList.Count > 0 && !string.IsNullOrEmpty(notesStringToAsk))
            {
                if (_regulationList.Any())
                {
                    var average =  (int)_regulationList.Average();
                    _regularAnswerCount = average > 0 ? average+1 : _regularAnswerCount;
                }
                
                if (_clearLine)
                {
                    Utility.ClearLastLines(3);
                    _clearLine = false;
                }
                
                Console.WriteLine($"\nNext to ask : {notesStringToAsk}");
                Utility.DrawProgressBar(preProcessedCount+notes.Count, total);
                Console.WriteLine();
                _clearLine = true;

                var newNotesString = await AskGemini(notesStringToAsk);
                
                var newNotes = await StringToAnkiNotes(newNotesString);

                if (_clearLine)
                {
                    Utility.ClearLastLines(3);
                    _clearLine = false;
                }

                foreach (var newNote in newNotes)
                {
                    if (newNote == null || !newNote.ContainsKey(_mainField) || newNote[_mainField] == null)
                        continue;
                    if (notes.Any(x => x != null && x.ContainsKey(_mainField) &&
                                        x[_mainField] != null &&
                                        x[_mainField].ToString().ToLower() == newNote[_mainField].ToString().ToLower()))
                        continue;

                    var front = Utility.FixText(newNote[_mainField].ToString());
                    
                    var exactMatch = askedList.FirstOrDefault(x => x.ToLower() == front.ToLower());
                    if (exactMatch != null)
                    {
                        askedList.Remove(exactMatch);
                    }
                    else
                    {
                        var closestMatch = askedList
                            .Select(x => new { Word = x, Distance = Utility.CalculateLevenshteinDistance(front.ToLower(), x.ToLower()) })
                            .OrderBy(x => x.Distance)
                            .FirstOrDefault();

                        if (closestMatch != null && closestMatch.Distance <= (closestMatch.Word.Length / 3) + 1)
                        {
                            Console.WriteLine($"> AI spelling correction: mapped '{closestMatch.Word}' to '{front}'.");
                            askedList.Remove(closestMatch.Word);
                        }
                        else
                        {
                            Console.WriteLine($"> There is no alteration for {front}");
                        }
                    }

                    var hasEssentials = true;
                    var tagsToAppend = new List<string>();

                    foreach (var item in newNote)
                    {
                        if (!_dataObject.Any(x => x.Key.ToLower() == item.Key.ToLower()))
                            continue;
                        try
                        {
                            var requirementStr = _dataObject[item.Key]?.ToLower() ?? "";
                            var isEssential = requirementStr.Contains("essential");
                            var isTag = requirementStr.Contains("tag");
                            
                            var valStr = item.Value?.ToString() ?? "";

                            if (isEssential && string.IsNullOrWhiteSpace(valStr))
                            {
                                Console.WriteLine($"> '{item.Key}' has not found!");
                                hasEssentials = false;
                            }

                            if (isTag && !string.IsNullOrWhiteSpace(valStr))
                            {
                                tagsToAppend.Add(valStr.Replace(" ", "_"));
                            }
                        }
                        catch (Exception e)
                        {
                            hasEssentials = false;
                            if (e.Message.ToLower().Contains("key"))
                            {
                                Console.WriteLine($"> '{item.Key}' has not provided for '{newNote[_mainField]}'");
                            }
                            else
                            {
                                Console.WriteLine($"> Error while checking parameters '{item.Key}' in '{newNote[_mainField]}'");
                            }
                        }
                    }

                    if (tagsToAppend.Any())
                    {
                        if (newNote["Tags"] != null)
                        {
                            var existingTags = newNote["Tags"].ToString();
                            newNote["Tags"] = (existingTags + " " + string.Join(" ", tagsToAppend)).Trim();
                        }
                        else
                        {
                            newNote["Tags"] = string.Join(" ", tagsToAppend);
                        }
                    }

                    if (hasEssentials)
                    {
                        notes.Add(newNote);
                        Console.WriteLine($"> {front}{Utility.PrintSpaces(newNote[_mainField].ToString().Length)}Rem:{total - preProcessedCount - notes.Count}");
                    }
                    else
                    {
                        Console.WriteLine($"> {front} skipped!");
                    }
                }

                _regulationList.Add(newNotes.Count);

                notesStringToAsk = String.Join(", ", askedList.ToArray());

                remList.Add(askedList.Count);

                if(remList.Count > 2)
                    if (remList[^1] == askedList.Count
                        && remList[^2] == askedList.Count
                        && remList[^3] == askedList.Count)
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Ops! there's a problem with remaining or Gemini introduction.");
                        Console.WriteLine("Let's let the remaining slide");
                        break;
                    }

                if (askedList.Count > 0)
                {
                    Console.WriteLine($"\nNext to ask : {notesStringToAsk}");
                    Utility.DrawProgressBar(preProcessedCount+notes.Count, total);
                    Console.WriteLine();
                    _clearLine = true;
                }

                var preNotes = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json") ?? new List<JObject>();
                preNotes.AddRange(notes);
                await JsonFileHandler.SaveToJsonFileAsync(preNotes, "saved.json");
            }
            return notes;
        }
    }
}