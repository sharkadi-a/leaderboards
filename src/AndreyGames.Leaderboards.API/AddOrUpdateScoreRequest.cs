namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Adds or updates score for the player
    /// </summary>
    public class AddOrUpdateScoreRequest : LeaderboardsCryptoRequest
    {
        /// <summary>
        /// The game
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Player's name
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// New player score
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// Is player a winner?
        /// </summary>
        public bool IsWinner { get; set; }
        
        /// <summary>
        /// Is the result actually a fraud? If true that means request will be silently dropped. A security measure.
        /// </summary>
        public bool IsFraud { get; set; }
    }
}