using System.Threading;
using System.Threading.Tasks;

namespace AndreyGames.Leaderboards.API
{
    public interface ILeaderboardsClient
    {
        Task AddLeaderboard(string game, CancellationToken token = default);
        Task<LeaderboardEntry> GetPlayerScore(string game, string playerName, CancellationToken token = default);
        Task<LeaderboardView> GetLeaderboard(string game, int? offset = null, int? limit = null, CancellationToken token = default);
        Task AddOrUpdateScore(string game, string playerName, int score, CancellationToken token = default);
    }
}