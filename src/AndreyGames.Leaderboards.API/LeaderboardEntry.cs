namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Entry for each player
    /// </summary>
    public sealed class LeaderboardEntry
    {
        /// <summary>
        /// Player's rank
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Player's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Player's max score
        /// </summary>
        public long Score { get; set; }
        
        /// <summary>
        /// True means that the player is a winner
        /// </summary>
        public bool IsWinner { get; set; }
    }
}