using System;
using System.Collections.Generic;
using System.Linq;
using Wordle_hw.Models;
using Wordle_hw.Models.DTOs;
using Wordle_hw.Models.Entities;

namespace Wordle_hw.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _db;
        private readonly List<string> _wordList;

        public GameService()
        {
            _db = new AppDbContext();
            _wordList = new List<string>
            {
                "apple", "crane", "grape", "blaze", "stone", "plane", "train", "mouse",
                "house", "bread", "chair", "table", "phone", "watch", "light", "music",
                "beach", "ocean", "river", "glass", "paper", "sound", "smile", "heart"
            };
        }

        public Game StartNewGame(int userId)
        {
            var newGame = new Game
            {
                UserId = userId,
                TargetWord = GetRandomWord(),
                StartDate = DateTime.UtcNow,
                Attempts = 0,
                IsWin = false
            };

            _db.Games.Add(newGame);
            _db.SaveChanges();
            return newGame;
        }

        public GameResponse GetGame(int gameId, int userId)
        {
            // First, retrieve the game and its associated guesses from the database.
            // At this point, no string.Split() operations are performed yet,
            // as the data is still in the database context.
            var game = _db.Games
                .Where(g => g.Id == gameId && g.UserId == userId)
                .Select(g => new
                {
                    Game = g, // Get the Game entity itself
                    Guesses = g.Guesses.Select(guess => new // Get the Guesses and their string properties
                    {
                        GuessNumber = guess.GuessNumber,
                        Word = guess.Word,
                        GuessResultString = guess.GuessResult, // Store as string for now
                        GuessedColorString = guess.GuessedColor // Store as string for now
                    }).ToList() // Materialize guesses here to bring them into memory
                })
                .FirstOrDefault(); // Materialize the game and its guesses here

            // If no game is found, return null.
            if (game == null)
            {
                return null;
            }

            // Now that the data is in memory, we can perform the string.Split() operations.
            var gameResponse = new GameResponse
            {
                Id = game.Game.Id,
                StartDate = game.Game.StartDate,
                EndDate = game.Game.EndDate,
                Attempts = game.Game.Attempts,
                IsWin = game.Game.IsWin,
                Guesses = game.Guesses.Select(guess => new GuessResponse
                {
                    GuessNumber = guess.GuessNumber,
                    Word = guess.Word,
                    GuessResult = guess.GuessResultString.Split(','),   // Perform Split() on data in memory
                    GuessedColor = guess.GuessedColorString.Split(',') // Perform Split() on data in memory
                }).ToList()
            };

            return gameResponse;
        }

        public List<GameResponse> GetUserGames(int userId)
        {
            // No changes needed here as there are no .Split() calls within the LINQ query.
            return _db.Games
                .Where(g => g.UserId == userId)
                .Select(g => new GameResponse
                {
                    Id = g.Id,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    Attempts = g.Attempts,
                    IsWin = g.IsWin
                })
                .OrderByDescending(g => g.StartDate)
                .ToList();
        }

        public GuessEvaluationResponse ProcessGuess(int gameId, int userId, GuessRequest guessRequest)
        {
            var game = _db.Games.FirstOrDefault(g => g.Id == gameId && g.UserId == userId);
            if (game == null || game.IsWin || game.EndDate != null)
                return null;

            var guessNumber = game.Attempts + 1;
            if (guessNumber > 6)
                return null;

            var evaluationResponse = EvaluateGuess(guessRequest.Word.ToLower(), game.TargetWord.ToLower());

            var guess = new Guess
            {
                GameId = gameId,
                Word = guessRequest.Word.ToLower(),
                GuessNumber = guessNumber,
                GuessResult = string.Join(",", evaluationResponse.Result),
                GuessedColor = string.Join(",", evaluationResponse.Color)
            };

            game.Attempts = guessNumber;
            bool isWin = guessRequest.Word.ToLower() == game.TargetWord.ToLower();
            bool isGameOver = isWin || guessNumber == 6;

            if (isWin)
            {
                game.IsWin = true;
                game.EndDate = DateTime.UtcNow;
                UpdateStatistics(userId, true, guessNumber);
            }
            else if (guessNumber == 6)
            {
                game.EndDate = DateTime.UtcNow;
                UpdateStatistics(userId, false, guessNumber);
            }

            _db.Guesses.Add(guess);
            _db.SaveChanges();

            return evaluationResponse;
        }

        public string GetRandomWord()
        {
            var rand = new Random();
            return _wordList[rand.Next(_wordList.Count)];
        }

        private GuessEvaluationResponse EvaluateGuess(string guess, string target)
        {
            var result = new string[5];
            var color = new string[5];
            var targetChars = target.ToCharArray();
            var guessChars = guess.ToCharArray();

            // First pass: mark correct positions
            for (int i = 0; i < 5; i++)
            {
                if (guessChars[i] == targetChars[i])
                {
                    result[i] = "correct";
                    color[i] = "green";
                    targetChars[i] = '*'; // Mark as used
                    guessChars[i] = '*'; // Mark as used
                }
            }

            // Second pass: mark present but wrong position
            for (int i = 0; i < 5; i++)
            {
                if (guessChars[i] != '*')
                {
                    bool found = false;
                    for (int j = 0; j < 5; j++)
                    {
                        if (targetChars[j] == guessChars[i])
                        {
                            result[i] = "present";
                            color[i] = "yellow";
                            targetChars[j] = '*'; // Mark as used
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        result[i] = "absent";
                        color[i] = "gray";
                    }
                }
            }

            return new GuessEvaluationResponse
            {
                Result = result,
                Color = color
            };
        }

        private void UpdateStatistics(int userId, bool won, int attempts)
        {
            var stats = _db.Statistics.FirstOrDefault(s => s.UserId == userId);
            if (stats == null)
            {
                stats = new Statistic { UserId = userId };
                _db.Statistics.Add(stats);
            }

            stats.GamesPlayed++;

            if (won)
            {
                stats.Wins++;
                stats.CurrentStreak++;
                stats.MaxStreak = Math.Max(stats.CurrentStreak, stats.MaxStreak);

                // Calculate points based on attempts (fewer attempts = more points)
                int points = Math.Max(0, 7 - attempts);
                stats.TotalPoints += points;
            }
            else
            {
                stats.CurrentStreak = 0;
            }

            _db.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}