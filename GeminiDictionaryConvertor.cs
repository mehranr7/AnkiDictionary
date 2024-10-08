﻿using ChatAIze.GenerativeCS.Constants;
using ChatAIze.GenerativeCS.Clients;
using ChatAIze.GenerativeCS.Models;
using ChatAIze.GenerativeCS.Options.Gemini;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AnkiDictionary
{
    public class GeminiDictionaryConvertor
    {
        private readonly ChatCompletionOptions _completionOptions;
        private readonly GeminiClient _client;
        private readonly ChatConversation _conversation;
        private readonly string _introduction;
        private readonly int _coolDown;
        private readonly List<int> _regulationList;
        private readonly int _maximumRequests;
        private int _regularAnswerCount;
        private bool _clearLine;
        private int _geminiRequestCounter;

        public GeminiDictionaryConvertor(string apiKey, string introduction)
        {
            _introduction = introduction;
            _client = new GeminiClient(apiKey);
            _conversation = new ChatConversation();
            _completionOptions = new ChatCompletionOptions()
            {
                Model = ChatCompletionModels.Gemini.GeminiPro
            };
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
            var chatQuestion = new ChatMessage()
            {
                Content = question
            };
            _conversation.Messages.Add(chatQuestion);
            while (string.IsNullOrEmpty(response))
            {
                try
                {
                    _geminiRequestCounter++;
                    response = await _client.CompleteAsync(_conversation,_completionOptions);
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
                    else
                    {
                        
                        await Utility.SaveAnError(
                            "Line 125 - \n\"A problem posed while asking Gemini.", e);
                        _clearLine = false;
                        

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

        public static async Task<List<AnkiNote>> StringToAnkiNotes(string input)
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

        public async Task<List<AnkiNote>> AskUntilCoverList(string askedString, bool needConfirm = true, int preProcessedCount = 0, int total = 0)
        {
            if (needConfirm)
            {
                needConfirm = Utility.AskTrueFalseQuestion("Do you want me to get confirmation to carry on?");
                Console.WriteLine("\n____________\n");
            }

            var notes = new List<AnkiNote>();
            var askedList = askedString.Split(",").Select(Utility.FixFrontText).ToList();

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
                    askedList.RemoveAll(x => x.ToLower() == answerNote.Front.ToLower());
                }

                var carryOnBuffering = true;
                if(needConfirm)
                    carryOnBuffering = Utility.AskTrueFalseQuestion($"Carry on asking more {_regularAnswerCount} cards?");
                

                if (!carryOnBuffering)
                {
                    var preNotesList = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("saved.json") ?? new List<AnkiNote>();
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
                        
                        var preNotesList = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("saved.json") ?? new List<AnkiNote>();
                        preNotesList.AddRange(notes);
                        await JsonFileHandler.SaveToJsonFileAsync(preNotesList, "saved.json");

                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Ops! there's a problem with remaining or Gemini introduction.");
                        Console.WriteLine("Let's let the remaining slide");
                        return notes;
                    }
            }

            var notesStringToAsk = String.Join(", ", askedList.ToArray());
            var remainingNotes = total - preProcessedCount;
            var carryOn = true;
            var remList = new List<int> { remainingNotes };
            while (remainingNotes>0 && carryOn && !string.IsNullOrEmpty(notesStringToAsk))
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
                    if (notes.Any(x => x.Front.ToLower() == newNote.Front.ToLower()))
                        continue;
                    var front = Utility.FixFrontText(newNote.Front);
                    var hasExactMatch = askedList.Any(x => x.ToLower() == newNote.Front.ToLower());
                    if (askedList.Any(x=>x.ToLower().Contains(front.ToLower())) && !hasExactMatch)
                    {
                        Console.WriteLine($"‣ {front} has misspelling.");
                        var newFront = askedList.FirstOrDefault(x => x.ToLower().Contains(front.ToLower()) && x.ToLower().Contains(newNote.Type.ToLower()));

                        if (newFront != null)
                        {
                            askedList.RemoveAll(x=>x.ToLower().Contains(front.ToLower()) && x.ToLower().Contains(newNote.Type.ToLower()));
                            newNote.Front = Utility.FixFrontText(newFront);
                            Console.WriteLine($"‣ {front} switched to {newNote.Front}");
                            front = Utility.FixFrontText(newNote.Front);
                        }
                        else
                        {
                            Console.WriteLine($"‣ There is no alteration for {front}");
                        }
                    }
                    else
                    {
                        askedList.Remove(front);
                    }
                    notes.Add(newNote);
                    Console.WriteLine($"‣ {front}{Utility.PrintSpaces(newNote.Front.Length)}Rem:{total-preProcessedCount-notes.Count}");
                }

                _regulationList.Add(newNotes.Count);

                remainingNotes -= newNotes.Count;

                notesStringToAsk = String.Join(", ", askedList.ToArray());

                if (string.IsNullOrEmpty(notesStringToAsk))
                {
                    carryOn = false;
                }
                else
                {
                    carryOn = true;
                
                    if(needConfirm)
                        carryOn = Utility.AskTrueFalseQuestion("Carry on asking remaining cards?");
                }
                remList.Add(remainingNotes);

                if(remList.Count > 2)
                    if (remList[^1] == remainingNotes
                        && remList[^2] == remainingNotes
                        && remList[^3] == remainingNotes)
                    {
                        Console.WriteLine("\n____________\n");
                        Console.WriteLine("Ops! there's a problem with remaining or Gemini introduction.");
                        Console.WriteLine("Let's let the remaining slide");
                        break;
                    }
                
                Console.WriteLine($"\nNext to ask : {notesStringToAsk}");
                Utility.DrawProgressBar(preProcessedCount+notes.Count, total);
                Console.WriteLine();
                _clearLine = true;

                var preNotes = await JsonFileHandler.ReadFromJsonFileAsync<List<AnkiNote>>("saved.json") ?? new List<AnkiNote>();
                preNotes.AddRange(notes);
                await JsonFileHandler.SaveToJsonFileAsync(preNotes, "saved.json");
            }
            return notes;
        }
    }
}