using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress] // Provides basic email format validation
        [StringLength(256)]
        public string Email { get; set; } // Matches your DB User's Email property

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Plain text password from client
    }
}