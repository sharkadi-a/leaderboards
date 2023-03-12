namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Base request
    /// </summary>
    public abstract class LeaderboardCryptoRequestBase
    {
        /// <summary>
        /// Encrypted request body
        /// </summary>
        public string Body { get; set; }
    }
}