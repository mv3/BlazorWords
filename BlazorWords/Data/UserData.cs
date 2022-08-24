using BlazorWords.Models;
using BlazorWords.Constants;

namespace BlazorWords.Data
{
    public class UserData
    {
        public UserData()
        {
        }
        public List<UserGuessWord> UserWords { get; set; } = new();
        public int CurrentWord { get; set; }
        public int CurrentGuess { get; set; }
        public int CurrentLetter { get; set; }
    }
}
