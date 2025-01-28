using AnkiDictionary;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;


// choose option
var isAsked = false;
var isIntroductionValid = false;

// Load appsettings.json
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var apiKey = config["Gemini:ApiKey"];
var defaultIntroduction = config["Gemini:DefaultIntroduction"];
var groupCount = config["Gemini:RegularAnswerCount"];
var coolDown = config["Gemini:CoolDown"];
var shortPause = config["Speed:ShortPause"];
var longPause = config["Speed:LongPause"];
var useAnkiConnect = config["General:UseAnkiConnect"] == "True";
var deckName = config["General:DeckName"];
var modelName = config["General:ModelName"];
var newTag = config["General:NewTag"];
var mainField = config["DynamicObject:MainField"];

var option = Utility.AskOptions(isAsked);
isAsked = true;
var validOptions = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "\u001b"};

if (apiKey == null 
    || defaultIntroduction == null
    || groupCount == null
    || coolDown == null
    || shortPause == null
    || longPause == null)
    return;

var geminiDictionaryConvertor =
    new GeminiDictionaryConvertor(apiKey, defaultIntroduction);

while (!validOptions.Any(x=>x.Equals(option)))
{
    option = Console.ReadKey(true).KeyChar.ToString();
}


while (!option.Equals("\u001b"))
{
    
    if (option.Equals("1") || option.Equals("4"))
    {
        while (!isIntroductionValid)
        {
            var introduction = Utility.AskAString("Give me an introduction to provide for Gemini or leave it empty to use default.");
            await geminiDictionaryConvertor.MakeAnIntroduction(introduction);
            isIntroductionValid = Utility.AskTrueFalseQuestion("Is the introduction valid?");
        }
    }

    switch (option)
    {
        case "1":
            var words = Utility.AskAString("Give me your word(s)/phrase(s) : ");

            var noteTags = Utility.AskAString("Please enter tag(s) : ");
            var tagList = new List<string>();
            if (noteTags.Contains(","))
                tagList = noteTags.Split(",").Select(Utility.FixFrontText).ToList();

            tagList.Add(newTag!);

            var requestedNotes = await geminiDictionaryConvertor.AskUntilCoverList(words);
            
            if (Utility.AskTrueFalseQuestion("Would you like to add given notes?"))
            {
                // Adding
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Adding new notes.");
                foreach (var note in requestedNotes)
                {
                    if (useAnkiConnect)
                    {
                        await AnkiConnect.AddNewNote(note, tagList, deckName!, modelName!, mainField!);
                    }
                    else
                    {
                        // Going for Anki window
                        ControllerSimulator.OpenAddNewWindow();
                        await ControllerSimulator.AddNewNote(note, tagList);
                    }
                }
            }

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;

            break;

        case "2":
            List<JObject> notes;
            
            var stringNotes = Utility.AskAString("Give me your note(s) to start. (JSON format)");
            
            try
            {
                notes = await GeminiDictionaryConvertor.StringToAnkiNotes(stringNotes);
            }
            catch (Exception)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Wrong format! Please notice the given note(s) must be in JSON format.");
                Console.WriteLine("\n____________\n");
                isAsked = false;
                break;
            }
            
            var tags = Utility.AskAString("Please enter tag(s) : ");
            var tagsList = new List<string>();
            if (tags.Contains(","))
                tagsList = tags.Split(",").Select(Utility.FixFrontText).ToList();

            tagsList.Add(newTag!);

            // Going for Anki window
            ControllerSimulator.OpenAddNewWindow();
            
            // Adding
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Adding new notes.");
            foreach (var note in notes)
            {
                if (useAnkiConnect)
                {
                    await AnkiConnect.AddNewNote(note, tagsList, deckName!, modelName!, mainField!);
                }
                else
                {
                    await ControllerSimulator.AddNewNote(note, tagsList);
                }
            }

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;

            break;

        case "3":
            var filter = Utility.AskAString("Give me the text if you want me to apply filter:");
            if (filter == "")
                filter = null;
            var neededFieldIndex = Utility.AskAnInteger("Give me the index of field which you wanna check:");
            var recordCount = Utility.AskAnInteger("How many record do you want me to check?");
            var skips = Utility.AskAnInteger("How many record do you want me to skip?");
            var mark =  Utility.AskAString("Give me a text mark if you want to leave a mark on note otherwise leave it empty:");
            if (mark == "")
                mark = null;
            
            var doubleDown = Utility.AskTrueFalseQuestion("Do you want to enable double down?");
            Console.WriteLine("\n____________\n");
            
            ControllerSimulator.OpenBrowseWindow();
            await ControllerSimulator.FindNeededItems(neededFieldIndex, recordCount, skips, filter, doubleDown, mark);

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;
            break;

        case "4":
            
            var cardsInNeeds =
                await JsonFileHandler.ReadFromJsonFileAsync<Dictionary<string, string>>("cardsInNeed.json");

            if (cardsInNeeds == null)
            {
                Console.WriteLine("\n____________\n");
                isAsked = false;
                break;
            }

            var saves =
                await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json")!;

            if (saves != null)
                foreach (var save in saves)
                {
                    foreach (var temp in cardsInNeeds)
                    {
                        if (temp.Key.ToLower() == save["Front"].ToString().ToLower())
                            cardsInNeeds.Remove(temp.Key);
                    }
                }

            var neededNotes = cardsInNeeds.Keys.Reverse().ToList();
            var howMany = -1;

            while (howMany < 1 || howMany > neededNotes.Count)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine($"There are {neededNotes.Count} cards.");
                howMany = Utility.AskAnInteger("How many cards do you want to update?");
            }

            neededNotes = neededNotes.Take(howMany).ToList();
            
            var stringUpdateNotes = "";

            foreach (var neededNote in neededNotes)
            {
                stringUpdateNotes += neededNote + ", ";
            }

            if (stringUpdateNotes.Length > 3)
            {
                stringUpdateNotes = stringUpdateNotes.Substring(0, stringUpdateNotes.Length - 2);
            }

            var updateNotes = await geminiDictionaryConvertor.AskUntilCoverList(stringUpdateNotes);
            
            if (Utility.AskTrueFalseQuestion("Would you like to update the notes?"))
            {
                ControllerSimulator.OpenBrowseWindow();
                await ControllerSimulator.UpdateNotes(updateNotes, new List<string>());
            }

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;
            
            break;

        case "5":

            Console.WriteLine("\n____________\n");
            Console.WriteLine("Updating...\n");
            var savedNotes = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json");

            while (savedNotes != null && savedNotes.Count > 0)
            {
                var dic = await JsonFileHandler.ReadFromJsonFileAsync<Dictionary<string, string>>("cardsInNeed.json");
                var extraCards = new List<JObject>();
                foreach (var card in savedNotes)
                {
                    if (dic != null && !dic.Any(x => x.Key.ToLower().Equals(card["Front"].ToString().ToLower())))
                    {
                        extraCards.Add(card);
                    }
                }

                foreach (var extraCard in extraCards)
                    savedNotes.Remove(extraCard);

                await JsonFileHandler.SaveToJsonFileAsync(savedNotes, "saved.json");

                ControllerSimulator.OpenBrowseWindow();
                await ControllerSimulator.UpdateNotes(savedNotes, new List<string>());

                savedNotes = await JsonFileHandler.ReadFromJsonFileAsync<List<JObject>>("saved.json");
            }
            
            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;
            break;

        case "6":

            var dictionary =
                await JsonFileHandler.ReadFromJsonFileAsync<Dictionary<string, string>>("cardsInNeed.json");
            if(dictionary == null)
            {
                Console.WriteLine("\n____________\n");
                isAsked = false;
                break;
            }

            var cardsInNeed = "";
            foreach (var card in dictionary)
            {
                cardsInNeed += card.Key + ", ";
            }
            if (cardsInNeed.Length > 3)
            {
                cardsInNeed = cardsInNeed.Substring(0, cardsInNeed.Length - 3);
            }

            if (cardsInNeed.Length > 0)
            {
                ClipboardManager.SetText(cardsInNeed);
            
                Console.WriteLine($"{dictionary.Count} words/phrases copied.");
                Console.WriteLine("\n____________\n");
            }
            else
            {
                Console.WriteLine("There is no words/phrases remaining.");
                Console.WriteLine("\n____________\n");
            }
            isAsked = false;
            break;

        case "7":
            List<JObject> updateNeededNotes;

            Console.WriteLine("\n____________\n");
            Console.WriteLine("Give me your note(s) to start. (JSON format)");
            var stringUpdateNeededNotes = Console.ReadLine();
            while (stringUpdateNeededNotes is null)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Give me your note(s) to start. (JSON format)");
                stringUpdateNeededNotes = Console.ReadLine();
            }
            try
            {
                updateNeededNotes = await GeminiDictionaryConvertor.StringToAnkiNotes(stringUpdateNeededNotes);
            }
            catch (Exception)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Wrong format! Please notice the given note(s) must be in JSON format.");
                Console.WriteLine("\n____________\n");
                isAsked = false;
                break;
            }
            ControllerSimulator.OpenBrowseWindow();
            await ControllerSimulator.UpdateNotes(updateNeededNotes, new List<string>());

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;
            break;
        
        case "8":
            isIntroductionValid = false;
            while (!isIntroductionValid)
            {
                var introduction = Utility.AskAString("Give me an introduction to provide for Gemini or leave it empty to use default.");
                await geminiDictionaryConvertor.MakeAnIntroduction(introduction);
                isIntroductionValid = Utility.AskTrueFalseQuestion("Is the introduction valid?");
            }
            break;

        case "9":
            var notesIdList = await AnkiConnect.FindNotes("current");
            Console.WriteLine($"{notesIdList.Count} note(s) found!");
            await AnkiConnect.CardsInfo(notesIdList);

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;
            break;

    }
    // choose option
    option = Utility.AskOptions(isAsked);
    if(!isAsked)
        isAsked = true;
}