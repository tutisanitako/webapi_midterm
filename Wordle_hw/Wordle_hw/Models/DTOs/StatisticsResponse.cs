namespace Wordle_hw.Models.DTOs
{
    public class StatisticsResponse
    {
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int TotalPoints { get; set; }
        public double WinPercentage => GamesPlayed > 0 ? (double)Wins / GamesPlayed * 100 : 0;
    }
}