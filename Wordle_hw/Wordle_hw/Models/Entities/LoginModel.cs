using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models.DTOs
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}