using System.Collections.Generic;
using Wordle_hw.Models.DTOs;
using Wordle_hw.Models.Entities;

namespace Wordle_hw.Services
{
    public interface IGameService
    {
        Game StartNewGame(int userId);
        GameResponse GetGame(int gameId, int userId);
        List<GameResponse> GetUserGames(int userId);
        GuessEvaluationResponse ProcessGuess(int gameId, int userId, GuessRequest guessRequest);
        string GetRandomWord();
    }
}