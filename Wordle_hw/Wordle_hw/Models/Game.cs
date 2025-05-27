using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey

namespace Wordle_hw.Models
{
    public class Game
    {
        public Guid GameId { get; set; } // Primary Key (using Guid as discussed before)

        public int UserId { get; set; } // Foreign Key to User
        [ForeignKey("UserId")]
        public User User { get; set; } // Navigation property

        [Required]
        [StringLength(5)]
        public string TargetWord { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Nullable, as it's set when game ends
        public int Attempts { get; set; } // Number of attempts made
        public bool IsWin { get; set; } // True if won, false if lost

        // Navigation property for guesses
        public ICollection<Guess> Guesses { get; set; }
    }
}