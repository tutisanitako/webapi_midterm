using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey

namespace Wordle_hw.Models
{
    public class Guess
    {
        public int Id { get; set; } // Primary Key

        public Guid GameId { get; set; } // Foreign Key to Game
        [ForeignKey("GameId")]
        public Game Game { get; set; } // Navigation property

        [Required]
        [StringLength(5)]
        public string Word { get; set; } // The word guessed

        public int GuessNumber { get; set; } // Which attempt this was (1st, 2nd, etc.)

        // Store the result of each letter's status (e.g., "C:correct,R:present,A:present,N:absent,E:correct")
        // This is a simplified way. A more robust way might involve a separate table for LetterStatus
        [Required]
        public string GuessResultJson { get; set; } // Store JSON string of LetterStatus list

        public DateTime GuessTime { get; set; } // When the guess was made
    }
}