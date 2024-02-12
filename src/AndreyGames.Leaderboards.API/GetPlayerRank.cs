namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Get player's offset in the leaderboard
    /// </summary>
    public class GetPlayerRank : LeaderboardsCryptoRequest
    {
        /// <summary>
        /// The game
        /// </summary>
        public string Game { get; set; }
        
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Return only winners
        /// </summary>
        public bool WinnersOnly { get; set; }
        
        /// <summary>
        /// Time frame for data
        /// </summary>
        public TimeFrame? Time { get; set; }
    }
}