using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Add this

namespace Wordle_hw.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        [Required]
        [MaxLength(256)] // Adjust max length as needed
        public string Email { get; set; } // Renamed Username to Email as per requirement
        [Required]
        [MaxLength(256)] // Store hashed password
        public string PasswordHash { get; set; }

        // Navigation properties (EF will manage these relationships)
        public ICollection<Game> Games { get; set; }
        public Statistic Statistics { get; set; } // One-to-one relationship
    }
}