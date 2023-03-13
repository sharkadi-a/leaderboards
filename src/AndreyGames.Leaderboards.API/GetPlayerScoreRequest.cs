using System;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Return player score
    /// </summary>
    [Serializable]
    public class GetPlayerScoreRequest : LeaderboardCryptoRequestBase
    {
        /// <summary>
        /// The game
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Player name to return the score for
        /// </summary>
        public string PlayerName { get; set; }
    }
}