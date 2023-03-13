using System;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Base request
    /// </summary>
    [Serializable]
    public abstract class LeaderboardCryptoRequestBase
    {
        /// <summary>
        /// Encrypted request body
        /// </summary>
        public string Body { get; set; }
    }
}