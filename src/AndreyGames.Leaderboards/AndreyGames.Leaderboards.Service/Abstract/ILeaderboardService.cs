using System.Threading.Tasks;
using AndreyGames.Leaderboards.Service.ViewModels;

namespace AndreyGames.Leaderboards.Service.Abstract
{
    public interface ILeaderboardService
    {
        Task<bool> Exist(string game);

        Task CreateLeaderboard(string game);
        
        Task<LeaderboardView> GetLeaderboard(string game, int? offset = null, int? limit = null);

        Task PutPlayerScore(string game, string playerName, long score);
    }
}