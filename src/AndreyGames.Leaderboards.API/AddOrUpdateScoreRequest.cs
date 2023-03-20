namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Adds or updates score for the player
    /// </summary>
    public class AddOrUpdateScoreRequest : LeaderboardCryptoRequestBase
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
    }
}