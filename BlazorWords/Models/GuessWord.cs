namespace BlazorWords.Models
{
    public class GuessWord
    {
        public GuessWord(int number, string word, string definition, string hint, string link)
        {
            Number = number;
            Word = word;
            Definition = definition;
            Hint = hint;
            Link = link;
        }

        public GuessWord()
        {

        }

        public int Number { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public string Link { get; set; }
        public string Hint { get; set; } = string.Empty;
    }
}
