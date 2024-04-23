namespace AnkiDictionary
{
    public class AnkiNote
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
        public string Definition { get; set; }
        public string Image { get; set; }
        public string Sentence { get; set; }
        public string Persian { get; set; }
        public List<string> Collocations { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Antonyms { get; set; }
    }
}
