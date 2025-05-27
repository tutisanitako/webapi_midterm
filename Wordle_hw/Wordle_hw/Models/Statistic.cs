using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey

namespace Wordle_hw.Models
{
    public class Statistic
    {
        [Key] // Indicate this is the primary key
        [ForeignKey("User")] // Also a foreign key to User (one-to-one)
        public int UserId { get; set; } // Primary Key and Foreign Key

        public User User { get; set; } // Navigation property

        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int TotalPoints { get; set; } // You might define how points are calculated
    }
}