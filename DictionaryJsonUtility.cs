
using Newtonsoft.Json;

namespace AnkiDictionary
{
    public static class DictionaryJsonUtility
    {
        public static void ExportDictionaryToJson(Dictionary<string, string> dictionary, string filePath="dictionary.json")
        {
            string json = JsonConvert.SerializeObject(dictionary);
            File.WriteAllText(filePath, json);
        }

        public static Dictionary<string, string> ImportDictionaryFromJson(string filePath="dictionary.json")
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            else
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
        }
    }
}