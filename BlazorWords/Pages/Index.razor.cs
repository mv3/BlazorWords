using BlazorWords.Models;
using BlazorWords.Models.Enums;
using BlazorWords.Data;
using BlazorWords.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.LocalStorage;

namespace BlazorWords.Pages
{
    public class IndexBase : ComponentBase
    {
        // Dependency Injection
        [Inject]
        public ILocalStorageService _localStorage { get; set; }
        [Inject]
        public IJSRuntime _JSRuntime { get; set; }
        
        // Locals
        private readonly Dictionary Dictionary = new();
        protected readonly Keyboard keyboard = new();
        private UserData UserData { get; set; } = new();

        // Frontend properties
        protected GuessWord CurrentWord { get; set; } = new();
        protected UserGuessWord CurrentUserWord { get; set; } = new();
        protected bool ShowPrevButton { get; set; }
        protected bool ShowNextButton { get; set; }
        protected bool ShowDefinition { get; set; } = false;
        protected bool NoMoreWords { get; set; }

        // JSInterop
        private async Task ShowToast(string message, int delay = Messages.DURATION_DEFAULT)
        {
            await _JSRuntime.InvokeVoidAsync("showToast", message, delay);
        }
        private async Task FlipTile(int guessNumber, int letterNumber, string color, string border)
        {
            await _JSRuntime.InvokeVoidAsync("flipTile", guessNumber, letterNumber, color, border);
        }
        private async Task BounceTile(int guessNumber, int letterNumber)
        {
            await _JSRuntime.InvokeVoidAsync("bounceTile", guessNumber, letterNumber);
        }
        private async Task BumpTile(int guessNumber, int letterNumber)
        {
            await _JSRuntime.InvokeVoidAsync("bumpTile", guessNumber, letterNumber);
        }


        // Lifecycle
        protected override async Task OnInitializedAsync()
        {
            UserData? userData = await _localStorage.GetItemAsync<UserData>(Misc.STORAGE_KEY);
            if(userData == null)
            {
                AddUserWord(GetCurrentWord());
            }
            else
            {
                UserData = userData;
            }
            await SetWord();
            StateHasChanged();
        }

        protected override async Task OnParametersSetAsync()
        {
            await SetColorsByWord(CurrentUserWord);                            
        }

        protected async void ResetData()
        {
            UserData = new UserData();
            AddUserWord(GetCurrentWord());
            await SetWord();
            await SaveGame();
        }


        // Data access
        private async Task SetWord()
        {
            CurrentWord = GetCurrentWord();
            Console.WriteLine(CurrentWord.Number);
            CurrentUserWord = GetCurrentUserWord();

            if(CurrentUserWord.UserGuesses.Any(ug => ug.Guessed == false))
            {
                UserData.CurrentGuess = CurrentUserWord.UserGuesses.Where(ug => ug.Guessed == false).Min(ug => ug.GuessNumber);
                UserData.CurrentLetter = CurrentUserWord.UserGuesses.First(ug => ug.GuessNumber == UserData.CurrentGuess)
                .GuessLetters.Where(gl => gl.Letter == Misc.BLANK_SPACE).Min(gl => gl.Number);
            }            
            
            ShowDefinition = CurrentUserWord.WordOver;
            await SetButtonVisibilityAsync();
            //StateHasChanged();
            ResetButtonColors();
            await SetColorsByWord(CurrentUserWord);
        }
        private async Task SaveGame()
        {
            await _localStorage.SetItemAsync(Misc.STORAGE_KEY, UserData);
        }
        private void AddUserWord(GuessWord word)
        {
            var userWord = new UserGuessWord(word.Number);

            for(int i = 0; i < GameConfig.GUESS_COUNT; i++)
            {
                var userGuess = new UserGuess(i);
                for(int j = 0; j < word.Word.Length; j++)
                {
                    userGuess.GuessLetters.Add(new(j));
                }
                userWord.UserGuesses.Add(userGuess);
            }

            UserData.UserWords.Add(userWord);
        }

        private UserGuessWord GetCurrentUserWord()
        {
            return UserData.UserWords.First(uw => uw.Number == UserData.CurrentWord);
        }

        private GuessWord GetCurrentWord()
        {
            return Dictionary.GuessWords.First(gw => gw.Number == UserData.CurrentWord);
        }

        private UserGuess GetCurrentGuess()
        {
            return CurrentUserWord.UserGuesses.First(ug => ug.GuessNumber == UserData.CurrentGuess);
        }
        private GuessLetter GetCurrentLetter()
        {
            return GetCurrentGuess().GuessLetters.First(gl => gl.Number == UserData.CurrentLetter);
        }

        // Frontend Events
        protected async Task OnLetterKey(string key)
        {
            if (CurrentUserWord.WordOver) return;
            switch (key)
            {
                case "ENTER":
                    await SubmitGuess();
                    break;
                case "⌫":
                    Backspace();
                    break;
                default:
                    await EnterLetter(key);
                    break;
            }
        }

        protected async Task OnNextWord()
        {
            if (UserData.CurrentWord < Dictionary.GuessWords.Max(gw => gw.Number))
            {
                UserData.CurrentWord++;
                UserData.CurrentGuess = 0;
                if(!UserData.UserWords.Any(uw=>uw.Number == UserData.CurrentWord))
                {
                    AddUserWord(GetCurrentWord());
                }
                await SetWord();
                await SaveGame();
            }
        }

        protected async Task OnPrevWordAsync()
        {
            if(UserData.CurrentWord > Dictionary.GuessWords.Min(gw => gw.Number))
            {
                UserData.CurrentWord--;
                UserData.CurrentGuess = 0;
                await SetWord();
                await SaveGame();
            }
        }

        private async Task SetButtonVisibilityAsync()
        {
            ShowPrevButton = UserData.CurrentWord > 0;

            ShowNextButton = GetCurrentUserWord().WordOver && UserData.CurrentWord < Dictionary.GuessWords.Max(gw => gw.Number);

            // Last puzzle message
            NoMoreWords = GetCurrentUserWord().WordOver && (UserData.CurrentWord == Dictionary.GuessWords.Max(gw => gw.Number));
        }

        private async Task SubmitGuess()
        {
            if (UserData.CurrentLetter < CurrentWord.Word.Length)
            {
                if (!GetCurrentGuess().ClassList.Contains(StyleClasses.WORD_SHAKE))
                {
                    GetCurrentGuess().ClassList.Add(StyleClasses.WORD_SHAKE);
                }
                await ShowToast(Messages.NOT_ENOUGH_LETTERS);
                return;
            }
            await EvaluateGuess();
            await SaveGame();
        }

        private void Backspace()
        {
            if (UserData.CurrentLetter > 0)
            {
                UserData.CurrentLetter--;
                GetCurrentLetter().Letter = Misc.BLANK_SPACE;
                GetCurrentGuess().ClassList.RemoveAll(x => x == StyleClasses.WORD_SHAKE);
            }
        }

        private async Task EnterLetter(string letter)
        {
            if (UserData.CurrentLetter < CurrentWord.Word.Length && !CurrentUserWord.WordOver)
            {
                GetCurrentLetter().Letter = letter;
                await BumpTile(UserData.CurrentGuess, UserData.CurrentLetter);
                GetCurrentGuess().ClassList.RemoveAll(x => x == StyleClasses.WORD_SHAKE);
                UserData.CurrentLetter++;
            }
        }

        private async Task EvaluateGuess()
        {

            var guess = GetCurrentGuess();
            var word = new List<string>();

            guess.Guessed = true;

            foreach (var letter in CurrentWord.Word)
            {
                word.Add(letter.ToString());
            }
            List<int> result = new();
            var remainingLetters = word;

            // Mark right letter right position
            for (int i = 0; i < guess.GuessLetters.Count; i++)
            {
                if (guess.GuessLetters[i].Letter == word[i])
                {
                    guess.GuessLetters[i].LetterStatus = LetterStatus.Correct;
                    remainingLetters[i] = " ";
                }
                else
                {
                    guess.GuessLetters[i].LetterStatus = LetterStatus.Incorrect;
                }
            }

            // Mark right letter wrong position
            for (int i = 0; i < guess.GuessLetters.Count; i++)
            {
                if (remainingLetters.Contains(guess.GuessLetters[i].Letter) && guess.GuessLetters[i].LetterStatus != LetterStatus.Correct)
                {
                    guess.GuessLetters[i].LetterStatus = Models.Enums.LetterStatus.Close;
                    remainingLetters.Remove(guess.GuessLetters[i].Letter);
                }
            }
            await SetColors(guess);

            if (guess.GuessLetters.All(l => l.LetterStatus == LetterStatus.Correct))
            {
                guess.IsCorrect = true;
                await WordOverAsync(win: true);
            }
            else if(UserData.CurrentGuess == GetCurrentUserWord().UserGuesses.Max(ug => ug.GuessNumber))
            {
                await WordOverAsync(win: false);
            }
            else
            {
                UserData.CurrentGuess++;
            }
            UserData.CurrentLetter = 0;
        }
        private async Task WordOverAsync(bool win)
        {
            var word = GetCurrentUserWord();
            word.WordOver = true;

            if (win)
            {
                foreach (var letter in GetCurrentGuess().GuessLetters)
                {
                    await BounceTile(UserData.CurrentGuess, letter.Number);
                }
                await ShowToast(Messages.WIN_MESSAGE[UserData.CurrentGuess]);
            }
            else
            {
                await ShowToast(CurrentWord.Word.ToUpper());
            }
            ShowDefinition = true;
            await SetButtonVisibilityAsync();
        }

        private async Task SetColors(UserGuess guess)
        {
            foreach (var letter in guess.GuessLetters)
            {
                var key = keyboard.GetKey(letter.Letter);
                switch (letter.LetterStatus)
                {
                    case LetterStatus.Correct:
                        await FlipTile(guess.GuessNumber, letter.Number, Colors.LETTER_CORRECT, Colors.LETTER_CORRECT);
                        key.KeyColor = Colors.LETTER_CORRECT;
                        break;
                    case LetterStatus.Close:
                        await FlipTile(guess.GuessNumber, letter.Number, Colors.LETTER_CLOSE, Colors.LETTER_CLOSE);

                        if (key.KeyColor == Colors.KEY_DEFAULT)
                        {
                            key.KeyColor = Colors.LETTER_CLOSE;
                        }
                        break;
                    case LetterStatus.Incorrect:
                        await FlipTile(guess.GuessNumber, letter.Number, Colors.LETTER_INCORRECT, Colors.LETTER_INCORRECT);

                        if (key.KeyColor == Colors.KEY_DEFAULT)
                        {
                            key.KeyColor = Colors.LETTER_INCORRECT;
                        }
                        break;
                    default:
                        await FlipTile(guess.GuessNumber, letter.Number, Colors.LETTER_DEFAULT, Colors.LETTER_DEFAULT_BORDER);
                        break;
                }
            }
            
        }

        private async Task SetColorsByWord(UserGuessWord word)
        {
            foreach (var guess in word.UserGuesses)
            {                
                await SetColors(guess);
            }
        }

        private void ResetButtonColors()
        {
            foreach(var keyRow in keyboard.KeyRows)
            {
                foreach(var key in keyRow.Keys)
                {
                    key.KeyColor = Colors.KEY_DEFAULT;
                }
            }
        }
    }
}