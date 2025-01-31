using System.Text.Json;

namespace AnkiDictionary
{
    public class JsonFileHandler
    {
        // Method to save an object to a JSON file
        public static async Task SaveToJsonFileAsync<T>(T obj, string filePath)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(obj);
                await File.WriteAllTextAsync(filePath, jsonString);
            }
            catch (Exception ex)
            {
            }
        }

        // Method to read an object from a JSON file
        public static async Task<T?> ReadFromJsonFileAsync<T>(string filePath) where T : new()
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(filePath);
                var obj = JsonSerializer.Deserialize<T>(jsonString);
                return obj;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("not find"))
                {
                    var file = File.Create(filePath);
                    file.Close();
                    return await ReadFromJsonFileAsync<T>(filePath);
                }
                else if(ex.Message.ToLower().Contains("linenumber: 0"))
                {
                    await SaveToJsonFileAsync(new T(), filePath);
                    return await ReadFromJsonFileAsync<T>(filePath);
                }
                await Utility.SaveAnError("Line 38 in Json", ex);
                Console.WriteLine($"An error occurred while reading from JSON file: {ex.Message}");
                return default(T);
            }
        }
    }
}