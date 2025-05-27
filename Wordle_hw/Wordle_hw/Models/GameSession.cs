// Previously Wordle_hw.Models.Game.cs, now renamed to GameSession.cs
using System;
using System.Collections.Generic;

namespace Wordle_hw.Models
{
    public class GameSession // Renamed from 'Game'
    {
        public Guid GameId { get; set; } // Unique ID for each game
        public string TargetWord { get; private set; } // The word to guess
        public int AttemptsLeft { get; set; }
        public List<string> Guesses { get; private set; } // List of words guessed so far
        public bool IsGameOver { get; set; }
        public bool IsWon { get; set; }

        private const int MaxAttempts = 6;
        private const int WordLength = 5;

        public GameSession(string targetWord) // Renamed constructor
        {
            GameId = Guid.NewGuid();
            TargetWord = targetWord.ToUpper();
            AttemptsLeft = MaxAttempts;
            Guesses = new List<string>();
            IsGameOver = false;
            IsWon = false;
        }

        public GuessResult MakeGuess(string guessedWord)
        {
            // ... (your existing MakeGuess logic here, it remains largely the same) ...
            if (IsGameOver)
            {
                return new GuessResult { Status = "Game Over", LetterStatuses = new List<LetterStatus>() };
            }

            guessedWord = guessedWord.ToUpper();

            if (guessedWord.Length != WordLength)
            {
                return new GuessResult { Status = "Invalid Length", LetterStatuses = new List<LetterStatus>() };
            }

            AttemptsLeft--;
            Guesses.Add(guessedWord);

            var letterStatuses = new List<LetterStatus>();
            var targetWordChars = TargetWord.ToCharArray();
            var guessedWordChars = guessedWord.ToCharArray();

            // First pass: Find 'correct' letters (green)
            for (int i = 0; i < WordLength; i++)
            {
                if (guessedWordChars[i] == targetWordChars[i])
                {
                    letterStatuses.Add(new LetterStatus { Letter = guessedWordChars[i], Status = "correct" });
                    targetWordChars[i] = '_'; // Mark as used
                    guessedWordChars[i] = '-'; // Mark as used
                }
                else
                {
                    letterStatuses.Add(new LetterStatus { Letter = guessedWordChars[i], Status = "absent" }); // Default to absent
                }
            }

            // Second pass: Find 'present' letters (yellow)
            for (int i = 0; i < WordLength; i++)
            {
                if (guessedWordChars[i] != '-') // If not already marked as correct
                {
                    int indexInTarget = Array.IndexOf(targetWordChars, guessedWordChars[i]);
                    if (indexInTarget != -1)
                    {
                        letterStatuses[i].Status = "present";
                        targetWordChars[indexInTarget] = '_'; // Mark as used
                    }
                }
            }

            if (guessedWord == TargetWord)
            {
                IsWon = true;
                IsGameOver = true;
            }
            else if (AttemptsLeft == 0)
            {
                IsGameOver = true;
            }

            return new GuessResult
            {
                GameId = GameId,
                GuessedWord = guessedWord,
                AttemptsLeft = AttemptsLeft,
                IsGameOver = IsGameOver,
                IsWon = IsWon,
                LetterStatuses = letterStatuses,
                Status = "Success",
                TargetWord = TargetWord // Keep this for loss scenario
            };
        }
    }

    // Keep these models as they are for the API response
    public class LetterStatus
    {
        public char Letter { get; set; }
        public string Status { get; set; }
    }

    public class GuessResult
    {
        public Guid GameId { get; set; }
        public string GuessedWord { get; set; }
        public int AttemptsLeft { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsWon { get; set; }
        public List<LetterStatus> LetterStatuses { get; set; }
        public string Status { get; set; }
        public string TargetWord { get; set; }
    }
}