using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity; // For Include() and other EF methods
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Wordle_hw.Data; // Assuming your DbContext is here
using Wordle_hw.Models; // Your database models and GameSession, GuessResult

namespace Wordle_hw.Services
{
    public class GameManager
    {
        // Replace in-memory storage with a dictionary of active game sessions for *in-progress* games
        // These will be loaded from/saved to the DB as needed.
        private static ConcurrentDictionary<Guid, GameSession> _activeGameSessions = new ConcurrentDictionary<Guid, GameSession>();
        private static List<string> _wordList;
        private static readonly object _lock = new object();

        private const int WordLength = 5;
        private const int MaxAttempts = 6; // Define max attempts here for consistency

        public GameManager()
        {
            LoadWordList();
        }

        private void LoadWordList()
        {
            if (_wordList == null)
            {
                lock (_lock)
                {
                    if (_wordList == null)
                    {
                        var appDataPath = HttpContext.Current.Server.MapPath("~/App_Data/words.txt");
                        // If you put words.txt in the root, change to "~/words.txt"
                        // var wordListPath = HttpContext.Current.Server.MapPath("~/words.txt");

                        if (File.Exists(appDataPath))
                        {
                            _wordList = File.ReadAllLines(appDataPath)
                                            .Where(w => w.Length == WordLength && w.All(char.IsLetter))
                                            .Select(w => w.ToUpper())
                                            .ToList();
                        }
                        else
                        {
                            _wordList = new List<string> { "APPLE", "BAKER", "CRANE", "DREAM", "EAGLE" };
                            System.Diagnostics.Debug.WriteLine("WARNING: words.txt not found. Using default word list.");
                        }
                    }
                }
            }
        }

        public StartGameResponse StartNewGame(int? userId) // userId is nullable for unauthenticated games
        {
            if (_wordList == null || !_wordList.Any())
            {
                throw new InvalidOperationException("Word list is not loaded or is empty. Cannot start a new game.");
            }

            var random = new Random();
            var targetWord = _wordList[random.Next(_wordList.Count)];

            // Create a new database Game entry
            var dbGame = new Game
            {
                GameId = Guid.NewGuid(), // Generate GUID for DB Game
                UserId = userId ?? 0, // Assign UserId (0 or default if anonymous)
                TargetWord = targetWord,
                StartDate = DateTime.UtcNow,
                Attempts = 0, // No attempts yet
                IsWin = false // Not won yet
            };

            using (var dbContext = new WordleDbContext())
            {
                // Optionally create or retrieve a User for anonymous games
                User currentUser = null;
                if (userId.HasValue)
                {
                    currentUser = dbContext.Users.Find(userId.Value);
                }
                else
                {
                    // For anonymous games, you might have a generic anonymous user,
                    // or just not link it to a user. For simplicity, we'll assign to 0.
                    // If 0 is not a valid User ID, you'd need to create an "Anonymous" user.
                    // For now, let's just make it assignable.
                }

                if (currentUser == null && userId.HasValue) // If userId provided but user not found
                {
                    // This is an error or requires user creation/registration logic
                    // For now, we'll proceed as anonymous if user not found for provided ID
                }

                dbContext.Games.Add(dbGame);
                dbContext.SaveChanges(); // Save the new game to the database

                // Create an in-memory session for immediate game play
                var gameSession = new GameSession(targetWord)
                {
                    GameId = dbGame.GameId, // Use the same GUID as the DB game
                    AttemptsLeft = MaxAttempts,
                    IsGameOver = false,
                    IsWon = false
                };
                _activeGameSessions.TryAdd(gameSession.GameId, gameSession);

                // Update user statistics if authenticated
                if (userId.HasValue && currentUser != null)
                {
                    var userStats = dbContext.Statistics.FirstOrDefault(s => s.UserId == userId.Value);
                    if (userStats == null)
                    {
                        userStats = new Statistic { UserId = userId.Value, GamesPlayed = 0, Wins = 0, CurrentStreak = 0, MaxStreak = 0, TotalPoints = 0 };
                        dbContext.Statistics.Add(userStats);
                    }
                    userStats.GamesPlayed++;
                    dbContext.SaveChanges();
                }

                return new StartGameResponse
                {
                    GameId = gameSession.GameId,
                    AttemptsLeft = gameSession.AttemptsLeft,
                    Message = "New Wordle game started. Good luck!"
                };
            }
        }


        public GuessResult ProcessGuess(Guid gameId, string guessedWord)
        {
            // Try to get the active game session from memory
            if (!_activeGameSessions.TryGetValue(gameId, out var gameSession))
            {
                // If not in memory, try to load from database
                using (var dbContext = new WordleDbContext())
                {
                    var dbGame = dbContext.Games
                                          .Include(g => g.Guesses) // Eager load guesses
                                          .FirstOrDefault(g => g.GameId == gameId);

                    if (dbGame == null)
                    {
                        return new GuessResult { Status = "Game Not Found", LetterStatuses = new List<LetterStatus>() };
                    }
                    if (dbGame.EndDate.HasValue) // If EndDate is set, game is over
                    {
                        return new GuessResult { Status = "Game Over", LetterStatuses = new List<LetterStatus>() };
                    }

                    // Reconstruct GameSession from DB Game
                    gameSession = new GameSession(dbGame.TargetWord)
                    {
                        GameId = dbGame.GameId,
                        AttemptsLeft = MaxAttempts - dbGame.Attempts, // Calculate remaining attempts
                        IsGameOver = false, // Assume not over until checked
                        IsWon = dbGame.IsWin // Set win status from DB
                    };
                    // Load existing guesses into the session
                    foreach (var dbGuess in dbGame.Guesses.OrderBy(g => g.GuessNumber))
                    {
                        gameSession.Guesses.Add(dbGuess.Word);
                    }
                    _activeGameSessions.TryAdd(gameId, gameSession); // Add to active sessions
                }
            }

            // At this point, gameSession should be available and up-to-date
            if (gameSession.IsGameOver)
            {
                return new GuessResult { Status = "Game Over", LetterStatuses = new List<LetterStatus>() };
            }

            // Basic validation for the guessed word (length and letters)
            if (guessedWord.Length != WordLength || !guessedWord.All(char.IsLetter))
            {
                return new GuessResult { Status = "Invalid Guess Format", LetterStatuses = new List<LetterStatus>() };
            }

            // Process the guess using the GameSession's logic
            var result = gameSession.MakeGuess(guessedWord);

            // Now, persist the guess and game state to the database
            using (var dbContext = new WordleDbContext())
            {
                var dbGame = dbContext.Games.Find(gameId);
                if (dbGame == null)
                {
                    return new GuessResult { Status = "Game Not Found after processing guess", LetterStatuses = new List<LetterStatus>() };
                }

                dbGame.Attempts++; // Increment attempt count
                dbGame.IsWin = result.IsWon;

                // Create a new Guess entry
                var dbGuess = new Guess
                {
                    GameId = gameId,
                    Word = guessedWord.ToUpper(),
                    GuessNumber = dbGame.Attempts, // This guess is the Nth attempt
                    GuessResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(result.LetterStatuses), // Serialize to JSON
                    GuessTime = DateTime.UtcNow
                };
                dbContext.Guesses.Add(dbGuess);

                if (result.IsGameOver)
                {
                    dbGame.EndDate = DateTime.UtcNow;

                    // Update user statistics if applicable
                    if (dbGame.UserId > 0) // Check if this game is linked to a user
                    {
                        var userStats = dbContext.Statistics.FirstOrDefault(s => s.UserId == dbGame.UserId);
                        if (userStats != null)
                        {
                            if (result.IsWon)
                            {
                                userStats.Wins++;
                                userStats.CurrentStreak++;
                                if (userStats.CurrentStreak > userStats.MaxStreak)
                                {
                                    userStats.MaxStreak = userStats.CurrentStreak;
                                }
                                // Example point calculation: more points for fewer attempts
                                userStats.TotalPoints += (MaxAttempts - dbGame.Attempts + 1) * 100;
                            }
                            else // Game lost
                            {
                                userStats.CurrentStreak = 0; // Reset streak on loss
                            }
                        }
                    }
                    _activeGameSessions.TryRemove(gameId, out _); // Remove from in-memory cache
                }

                dbContext.SaveChanges(); // Save all changes to the database
            }

            return result;
        }

        // Optional: Methods to retrieve user statistics or past games
        public Statistic GetUserStatistics(int userId)
        {
            using (var dbContext = new WordleDbContext())
            {
                return dbContext.Statistics.FirstOrDefault(s => s.UserId == userId);
            }
        }
    }
}