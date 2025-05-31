using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(10)]
        public string TargetWord { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Attempts { get; set; }
        public bool IsWin { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Guess> Guesses { get; set; } = new List<Guess>();
    }
}