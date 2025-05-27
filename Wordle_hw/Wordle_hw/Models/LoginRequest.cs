using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } // Matches your DB User's Email property

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Plain text password from client
    }
}