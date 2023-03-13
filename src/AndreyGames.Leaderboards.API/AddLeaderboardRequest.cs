using System;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Add leaderboard for a game 
    /// </summary>
    [Serializable]
    public class AddLeaderboardRequest : LeaderboardCryptoRequestBase
    {
        /// <summary>
        /// The name of a game
        /// </summary>
        public string Game { get; set; }
    }
}