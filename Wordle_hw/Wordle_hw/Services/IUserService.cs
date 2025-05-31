using Wordle_hw.Models.DTOs;
using Wordle_hw.Models.Entities;

namespace Wordle_hw.Services
{
    public interface IUserService
    {
        bool Register(RegisterModel model);
        User Authenticate(LoginModel model);
        string GenerateJwtToken(User user);
        User GetUserById(int userId);
    }
}