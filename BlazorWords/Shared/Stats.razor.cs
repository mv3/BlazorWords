using BlazorWords.Models;
using BlazorWords.Models.Enums;
using BlazorWords.Data;
using BlazorWords.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.LocalStorage;

namespace BlazorWords.Shared
{
    public class StatsBase : ComponentBase
    {
        [Parameter] public EventCallback Callback { get; set; }

        // Dependency Injection
        [Inject]
        public ILocalStorageService _localStorage { get; set; }
        [Inject]
        public IJSRuntime _JSRuntime { get; set; }

        protected class GuessDistribution
        {
            public int GuessNumber { get; set; }
            public int GuessCount { get; set; }
            public int Percentage { get; set; }
            public string Color { get; set; } = Colors.STAT_DEFAULT;
        }


        private UserData UserData { get; set; } = new();
        protected string GamesPlayed = "";
        protected string GamesWon = "";
        protected string WinPercent = "";
        protected string CurrentStreak = "";
        protected string MaxStreak = "";
        protected List<GuessDistribution> GuessDist = new();

        // JSInterop
        private async Task ShowToast(string message, int delay = Messages.DURATION_DEFAULT)
        {
            await _JSRuntime.InvokeVoidAsync("showToast", message, delay);
        }
        private async Task CopyToClipboard(string message)
        {
            await _JSRuntime.InvokeVoidAsync("copyToClipboard", message);
        }

        private async Task<bool> ShareStats(string message)
        {
            string result = await _JSRuntime.InvokeAsync<string>("shareStats", message);
            return true;
        }

        public async Task ReloadStats()
        {
            UserData? userData = await _localStorage.GetItemAsync<UserData>(Misc.STORAGE_KEY);
            if (userData == null)
            {
                UserData = new();
            }
            else
            {
                UserData = userData;
            }
            await loadStats();
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await ReloadStats();
        }

        private async Task loadStats()
        {
            GuessDist = new();
            var gamesPlayed = UserData.UserWords.Where(uw => uw.WordOver).ToList();
            GamesPlayed = gamesPlayed.Count().ToString();

            var wins = gamesPlayed.Where(uw => uw.UserGuesses.Any(ug => ug.IsCorrect)).ToList();
            GamesWon = wins.Count.ToString();

            if(gamesPlayed.Count > 0)
            {
                WinPercent = Math.Round(wins.Count * 100.0 / gamesPlayed.Count).ToString();
            }
            else
            {
                WinPercent = 0.ToString();
            }
            

            var currentStreak = 0;
            var maxStreak = 0;
            for (int i = 0; i < GameConfig.GUESS_COUNT; i++)
            {
                GuessDist.Add(new GuessDistribution { GuessNumber = i});
            }

            foreach (var game in gamesPlayed)
            {
                if (game.UserGuesses.Any(g => g.IsCorrect))
                {
                    currentStreak++;
                    var correctGuesses = game.UserGuesses.Where(ug => ug.IsCorrect).ToList();
                    foreach(var correct in correctGuesses)
                    {
                        GuessDist.First(gd => gd.GuessNumber == correct.GuessNumber).GuessCount++;
                    }
                    
                }
                else
                {
                    currentStreak = 0;
                }
                if (currentStreak > maxStreak)
                {
                    maxStreak = currentStreak;
                }
            }

            CurrentStreak = currentStreak.ToString();
            MaxStreak = maxStreak.ToString();

            var currentWord = UserData.UserWords.FirstOrDefault(uw => uw.Number == UserData.CurrentWord);

            foreach (var dist in GuessDist)
            {
                var maxCount = GuessDist.Max(gd=>gd.GuessCount);
                int pct = 0;
                if(maxCount > 0)
                {
                    pct = Convert.ToInt32(Math.Round(dist.GuessCount * 100.0 / maxCount));
                }
                dist.Percentage = pct < 5 ? 5 : pct;

                if (currentWord == null) continue;
                var correct = currentWord.UserGuesses.FirstOrDefault(ug => ug.IsCorrect);
                if (currentWord.WordOver && correct != null && correct.GuessNumber == dist.GuessNumber)
                {
                    dist.Color = Colors.STAT_CURRENT;
                }
            }
        }

        protected async Task ShareResult()
        {
            UserGuessWord? currentWord = UserData.UserWords.FirstOrDefault(uw => uw.Number == UserData.CurrentWord);

            if (currentWord == null) return;
            
            var correctGuess = currentWord.UserGuesses.Where(ug => ug.IsCorrect).FirstOrDefault();

            var guessNumber = correctGuess == null ? "X" : (correctGuess.GuessNumber + 1).ToString();

            var result = $"BLAZORword #{UserData.CurrentWord + 1} {guessNumber}/{currentWord.UserGuesses.Count}";
            foreach(var guess in currentWord.UserGuesses)
            {
                if (!guess.Guessed) continue;
                var guessTiles = "";
                foreach(var letter in guess.GuessLetters)
                {
                    switch (letter.LetterStatus)
                    {
                        case LetterStatus.Correct:
                            guessTiles += Misc.EMOJI_GREEN_SQUARE;
                            break;
                        case LetterStatus.Close:
                            guessTiles += Misc.EMOJI_YELLOW_SQUARE;
                            break;
                        default:
                            guessTiles += Misc.EMOJI_BLACK_SQUARE;
                            break;
                    }
                }
                result += Environment.NewLine + guessTiles;
            }

            await CopyToClipboard(result);
            await ShowToast(Messages.COPIED_TO_CLIPBOARD);
        }

        protected async Task ShareStats()
        {
            var result = "BLAZORwords - Stats" + Environment.NewLine;
            result += $"Won: {GamesWon}" + Environment.NewLine;
            result += $"Played: {GamesPlayed}" + Environment.NewLine;
            result += $"Win %: {WinPercent}" + Environment.NewLine;
            result += $"Current Streak: {CurrentStreak}" + Environment.NewLine;
            result += $"Max Streak: {MaxStreak}" + Environment.NewLine;

            await CopyToClipboard(result);
            await ShowToast(Messages.COPIED_TO_CLIPBOARD);
        }
    }
}
