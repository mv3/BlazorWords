namespace BlazorWords.Models
{
    public class UserGuessWord
    {
        public UserGuessWord()
        {

        }
        public UserGuessWord(int wordNumber)
        {
            Number = wordNumber;
        }
        public List<UserGuess> UserGuesses { get; set; } = new();
        public bool WordOver { get; set; }
        public int Number { get; set; }
    }
}
