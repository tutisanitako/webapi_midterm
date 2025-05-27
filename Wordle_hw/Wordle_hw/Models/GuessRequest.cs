using System;
using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models
{
    public class GuessRequest
    {
        [Required]
        public Guid GameId { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Guess must be 5 letters long.")]
        public string Guess { get; set; }
    }
}