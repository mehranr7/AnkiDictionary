using AnkiDictionary;
using Microsoft.Extensions.Configuration;

// choose option
var option = ProgramHandler.AskOptions();
var validOptions = new List<string> {"1", "2", "3"};

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
    option = ProgramHandler.AskOptions();
}

if (option.Equals("1"))
{
    Console.WriteLine("\n____________\n");
    Console.WriteLine("Give me an introduction to provide for Gemini or leave it empty to use default.");
    var introduction = Console.ReadLine();
    await ProgramHandler.Introduction(introduction, geminiDictionaryConvertor);
}

while (!option.Equals("3"))
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
            break;
    }
    
    // choose option
    option = ProgramHandler.AskOptions();
}


