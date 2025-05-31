using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models.DTOs
{
    public class GuessRequest
    {
        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Word must be exactly 5 characters long.")]
        public string Word { get; set; }
    }
}