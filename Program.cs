using System.Net.WebSockets;
using AnkiDictionary;
using GenerativeAI.Types.RagEngine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

System.Console.InputEncoding = System.Text.Encoding.UTF8;
System.Console.OutputEncoding = System.Text.Encoding.UTF8;

// choose option
var isAsked = false;
var isIntroductionValid = false;

// Load appsettings.json
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

if (!File.Exists("AI Explanation.txt"))
{
    Console.WriteLine("Error: 'AI Explanation.txt' is missing in the project directory.");
    return;
}
var defaultIntroduction = File.ReadAllText("AI Explanation.txt");

var apiKey = config["Gemini:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Error: API Key is required for Google Gemini AI. Please set 'Gemini:ApiKey' in appsettings.json.");
    return;
}

var model = config["Gemini:Model"];
var groupCount = config["Gemini:RegularAnswerCount"];
var coolDown = config["Gemini:CoolDown"];
var deckName = config["General:DeckName"];
var modelName = config["General:ModelName"];
var newTag = config["General:NewTag"];
var editTag = config["General:EditTag"];
var mainField = config["DynamicObject:MainField"];

if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(groupCount) || 
    string.IsNullOrWhiteSpace(coolDown) || string.IsNullOrWhiteSpace(deckName) || 
    string.IsNullOrWhiteSpace(modelName) || string.IsNullOrWhiteSpace(mainField))
{
    Console.WriteLine("Error: One or more required configurations are missing in appsettings.json.");
    return;
}

var option = Utility.AskOptions(isAsked);
isAsked = true;
var validOptions = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "\u001b"};

var geminiDictionaryConvertor =
    new GeminiDictionaryConvertor(apiKey, defaultIntroduction, model!);

while (!validOptions.Any(x=>x.Equals(option)))
{
    option = Console.ReadKey(true).KeyChar.ToString();
}


while (!option.Equals("\u001b"))
{
    
    if (option.Equals("1") || option.Equals("3"))
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
                tagList = noteTags.Split(",").Select(Utility.FixText).ToList();

            tagList.Add(newTag!);

            var requestedNotes = await geminiDictionaryConvertor.AskUntilCoverList(words);
            
            if (Utility.AskTrueFalseQuestion("Would you like to add given notes?"))
            {
                // Adding
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Adding new notes.");
                foreach (var note in requestedNotes)
                {
                    await AnkiConnect.AddNewNote(note, tagList, deckName!, modelName!, mainField!);
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
                tagsList = tags.Split(",").Select(Utility.FixText).ToList();

            tagsList.Add(newTag!);

            // Adding
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Adding new notes.");
            foreach (var note in notes)
            {
                await AnkiConnect.AddNewNote(note, tagsList, deckName!, modelName!, mainField!);
            }

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;

            break;

        case "3":
            var query = Utility.AskAString("Give me the query to apply filter (leave empty to update ALL notes of the deck):");
            if (query == "")
                query = "deck:"+deckName;

            var recordCount = Utility.AskAnInteger("How many record do you want me to check?");

            var tempTag = Utility.AskAString("Please enter tag(s) : ");
            var newTagList = new List<string>();
            if (tempTag.Contains(","))
                newTagList = tempTag.Split(",").Select(Utility.FixText).ToList();

            newTagList.Add(editTag!);


            var filteredNotes = await AnkiConnect.FindNotes(query, recordCount);

            var dictNotes = await AnkiConnect.NotesInfo(filteredNotes, mainField!);


            // Updating
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Updating new notes.");

            while (dictNotes.Any())
            {
                var notesToAsk = "";
                var notePairs = new List<string>();
                for (int i = 0; i < int.Parse(groupCount); i++)
                {
                    if (dictNotes.Any())
                    {
                        var currentNote = dictNotes.Pop();
                        notePairs.Add(currentNote);
                        notesToAsk += currentNote.Split(',')[0] + ",";
                    }
                }
                
                var updatedNote = await geminiDictionaryConvertor.AskUntilCoverList(notesToAsk,false);
                foreach (var item in notePairs)
                {
                    var pair = item.Split(',');
                    var updatedItem = updatedNote.FirstOrDefault(x => x[mainField!]!.ToString().ToLower().Contains(pair[0].ToLower()));
                    if(updatedItem!=null)
                        await AnkiConnect.UpdateNote(pair[1], updatedItem, newTagList, mainField!);
                }
            }

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            isAsked = false;
            break;

        case "5":
            isIntroductionValid = false;
            while (!isIntroductionValid)
            {
                var introduction = Utility.AskAString("Give me an introduction to provide for Gemini or leave it empty to use default.");
                await geminiDictionaryConvertor.MakeAnIntroduction(introduction);
                isIntroductionValid = Utility.AskTrueFalseQuestion("Is the introduction valid?");
            }
            break;

        case "6":
            var notesIdList = await AnkiConnect.FindNotes("deck:"+deckName);

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