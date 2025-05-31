using System;
using System.Collections.Generic;

namespace Wordle_hw.Models.DTOs
{
    public class GameResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Attempts { get; set; }
        public bool IsWin { get; set; }
        public List<GuessResponse> Guesses { get; set; } = new List<GuessResponse>();
    }

    public class GuessResponse
    {
        public int GuessNumber { get; set; }
        public string Word { get; set; }
        public string[] GuessResult { get; set; }
        public string[] GuessedColor { get; set; }
    }

    public class GuessEvaluationResponse
    {
        public string[] Result { get; set; }
        public string[] Color { get; set; }
    }
}