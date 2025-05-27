using System;

namespace Wordle_hw.Models
{
    public class StartGameResponse
    {
        public Guid GameId { get; set; }
        public int AttemptsLeft { get; set; }
        public string Message { get; set; }
    }
}