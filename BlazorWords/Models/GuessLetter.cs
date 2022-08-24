using BlazorWords.Models.Enums;
namespace BlazorWords.Models
{
    public class GuessLetter
    {
        public GuessLetter(int number)
        {
            Number = number;
        }
        public GuessLetter()
        {

        }
        public string Letter { get; set; } = "&nbsp;";
        public int Number { get; set; }
        public LetterStatus LetterStatus { get; set; }
        public List<string> ClassList { get; set; } = new();
        public string Style { get; set; } = string.Empty;
    }
}
