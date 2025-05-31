using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Wordle_hw.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public virtual ICollection<Game> Games { get; set; } = new List<Game>();
        public virtual Statistic Statistics { get; set; }
    }
}