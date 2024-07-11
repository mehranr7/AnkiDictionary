namespace AnkiDictionary
{
    public class AnkiNote
    {
        public string NoteId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
        public string Level { get; set; }
        public string Band { get; set; }
        public short Frequency { get; set; }
        public string AmericanPhonetic { get; set; }
        public string BritishPhonetic { get; set; }
        public string Definition { get; set; }
        public string? Image { get; set; }
        public string Sentence { get; set; }
        public string Persian { get; set; }
        public string? Adjective { get; set; }
        public string? Adverb { get; set; }
        public string? Verb { get; set; }
        public string? Noun { get; set; }
        public List<string> Collocations { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Antonyms { get; set; }
        public List<string> Categories { get; set; }
    }
}
