using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models.Entities
{
    public class Statistic
    {
        public int Id { get; set; } // Keep this as the PK

        public int UserId { get; set; } // Keep this as the FK
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int TotalPoints { get; set; }

        public virtual User User { get; set; }
    }
}