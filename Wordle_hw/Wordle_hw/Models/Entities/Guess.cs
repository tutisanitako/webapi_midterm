using System.ComponentModel.DataAnnotations;

namespace Wordle_hw.Models.Entities
{
    public class Guess
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Word { get; set; }

        public int GuessNumber { get; set; }

        [MaxLength(50)]
        public string GuessResult { get; set; }

        [MaxLength(50)]
        public string GuessedColor { get; set; }

        public virtual Game Game { get; set; }
    }
}