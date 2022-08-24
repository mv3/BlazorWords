namespace BlazorWords.Models
{
    public class UserGuess
    {
        public UserGuess()
        {

        }
        public UserGuess(int guessNumber)
        {
            GuessNumber = guessNumber;
        }
        public int GuessNumber { get; set; }
        public List<GuessLetter> GuessLetters { get; set; } = new();
        public List<string> ClassList { get; set; } = new();
        public bool IsCorrect { get; set; }
        public bool Guessed { get; set; }
    }
}
