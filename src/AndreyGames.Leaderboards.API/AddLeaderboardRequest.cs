namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Add leaderboard for a game 
    /// </summary>
    public class AddLeaderboardRequest : LeaderboardCryptoRequest
    {
        /// <summary>
        /// The name of a game
        /// </summary>
        public string Game { get; set; }
    }
}