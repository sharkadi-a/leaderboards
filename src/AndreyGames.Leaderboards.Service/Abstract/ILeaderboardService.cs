using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndreyGames.Leaderboards.API;

namespace AndreyGames.Leaderboards.Service.Abstract
{
    public interface ILeaderboardService
    {
        Task<bool> Exist(string game);

        Task CreateLeaderboard(string game);

        Task<LeaderboardView> GetLeaderboard(string game, 
            DateTime? start = default, 
            DateTime? end = default,
            bool onlyWinners = false, 
            int? offset = null, 
            int? limit = null);

        Task<LeaderboardEntry> GetPlayerRank(string game,
            string playerName,
            bool caseInsensitive,
            DateTime? start = default,
            DateTime? end = default,
            bool onlyWinners = false);

        Task<ICollection<LeaderboardEntry>> GetScoreForPlayer(string game, string playerName, bool caseInsensitive);

        Task PutPlayerScore(string game, DateTime date, string playerName, long score, bool isWinner = false, bool isFraud = false);
    }
}