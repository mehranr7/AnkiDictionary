using AnkiDictionary;
using Microsoft.Extensions.Configuration;

// choose option
var option = ProgramHandler.AskOptions();
var validOptions = new List<string> {"1", "2", "3", "4", "5", "\u001b"};

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var apiKey = config["Gemini:ApiKey"];

if (apiKey == null)
    return;

var geminiDictionaryConvertor =
    new GeminiDictionaryConvertor(apiKey);

while (!validOptions.Any(x=>x.Equals(option)))
{
    option = Console.ReadKey(true).KeyChar.ToString();
}

if (option.Equals("1"))
{
    Console.WriteLine("\n____________\n");
    Console.WriteLine("Give me an introduction to provide for Gemini or leave it empty to use default.");
    var introduction = Console.ReadLine();
    await ProgramHandler.Introduction(introduction, geminiDictionaryConvertor);
}

while (!option.Equals("\u001b"))
{
    switch (option)
    {
        case "1":
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Give me your word(s)/phrase(s) to start.");
            var words = Console.ReadLine();
            while (words is null)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Give me your word(s)/phrase(s) to start.");
                words = Console.ReadLine();
            }
            await ProgramHandler.AskGeminiAnkiNotes(words, geminiDictionaryConvertor);
            
            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");

            break;

        case "2":
            List<AnkiNote> notes;

            Console.WriteLine("\n____________\n");
            Console.WriteLine("Give me your note(s) to start. (JSON format)");
            var stringNotes = Console.ReadLine();
            while (stringNotes is null)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Give me your note(s) to start. (JSON format)");
                stringNotes = Console.ReadLine();
            }
            try
            {
                notes = GeminiDictionaryConvertor.StringToAnkiNotes(stringNotes);
            }
            catch (Exception)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Wrong format! Please notice the given note(s) must be in JSON format.");
                break;
            }
            ProgramHandler.StartAddingNotes(notes);
            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");

            break;

        case "3":
            Console.WriteLine("\n____________\n");
            Console.WriteLine("Give me the text if you want me to apply FILTER:");
            var filter = Console.ReadLine();
            if (filter == "")
                filter = null;

            Console.WriteLine("\n____________\n");
            Console.WriteLine("How many record do you want me to CHECK?");
            var recordCount = Int32.Parse(Console.ReadLine()!);

            Console.WriteLine("\n____________\n");
            Console.WriteLine("How many record do you want me to SKIP?");
            var skips = Int32.Parse(Console.ReadLine()!);
            ProgramHandler.SeparateImageAndPronunciation(recordCount, skips, filter);

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            break;

        case "4":
            List<AnkiNote> updateNotes;

            Console.WriteLine("\n____________\n");
            Console.WriteLine("Give me your note(s) to start. (JSON format)");
            var stringUpdateNotes = Console.ReadLine();
            while (stringUpdateNotes is null)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Give me your note(s) to start. (JSON format)");
                stringUpdateNotes = Console.ReadLine();
            }
            try
            {
                updateNotes = GeminiDictionaryConvertor.StringToAnkiNotes(stringUpdateNotes);
            }
            catch (Exception)
            {
                Console.WriteLine("\n____________\n");
                Console.WriteLine("Wrong format! Please notice the given note(s) must be in JSON format.");
                Console.WriteLine("\n____________\n");
                break;
            }
            ProgramHandler.UpdateNotes(updateNotes);

            Console.WriteLine("Done.");
            Console.WriteLine("\n____________\n");
            break;
        case "5":
            var dictionary = DictionaryJsonUtility.ImportDictionaryFromJson();
            var cardsInNeed = "";
            foreach (var card in dictionary)
            {
                cardsInNeed += card.Key + ", ";
            }
            if (cardsInNeed.Length > 2)
            {
                cardsInNeed = cardsInNeed.Substring(0, cardsInNeed.Length - 2);
            }
            
            ClipboardManager.SetText(cardsInNeed);
            
            Console.WriteLine($"{dictionary.Count} words/phrases copied.");
            Console.WriteLine("\n____________\n");
            break;
    }
    
    // choose option
    option = ProgramHandler.AskOptions();
}