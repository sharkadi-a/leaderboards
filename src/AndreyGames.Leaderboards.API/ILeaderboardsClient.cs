using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Leaderboard API Client
    /// </summary>
    public interface ILeaderboardsClient
    {
        /// <summary>
        /// Add leaderboard for a game 
        /// </summary>
        /// <exception cref="ApiException">Something went wrong on the server side.</exception>
        Task AddLeaderboard(string game, CancellationToken token = default);

        /// <summary>
        /// Get player's score for the game
        /// </summary>
        /// <exception cref="ApiException">Something went wrong on the server side.</exception>
        Task<ICollection<LeaderboardEntry>> GetPlayerScore(string game, 
            string playerName, 
            bool caseInsensitive = false,
            CancellationToken token = default);

        /// <summary>
        /// Returns player's rank position in the specified leaderboard.
        /// </summary>
        /// <exception cref="ApiException">Something went wrong on the server side.</exception>
        Task<LeaderboardEntry> GetPlayerRank(string game, 
            string playerName, 
            bool caseInsensitive = false,
            bool winnersOnly = false,
            TimeFrame? timeFrame = default, 
            CancellationToken token = default);

        /// <summary>
        /// Get leaderboard for the game
        /// </summary>
        /// <exception cref="ApiException">Something went wrong on the server side.</exception>
        Task<LeaderboardView> GetLeaderboard(string game, bool winnersOnly = false, TimeFrame? timeFrame = default, int? offset = null, int? limit = null, CancellationToken token = default);
        
        /// <summary>
        /// Add or update score for the game and player 
        /// </summary>
        /// <exception cref="ApiException">Something went wrong on the server side.</exception>
        Task AddOrUpdateScore(string game, string playerName, long score, bool isWinner, bool isFraud, CancellationToken token = default);
    }
}