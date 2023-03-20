﻿namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Get a leaderboard for the game
    /// </summary>
    public class GetLeaderboardRequest : LeaderboardCryptoRequest
    {
        /// <summary>
        /// The game
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Paging offset
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// The limit of items on a page
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Return only winners
        /// </summary>
        public bool WinnersOnly { get; set; }
    }
}