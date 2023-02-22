using System.Collections.Generic;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// A leaderboard.
    /// </summary>
    public class LeaderboardView
    {
        /// <summary>
        /// Current paging offset
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Leaderboard entries
        /// </summary>
        public ICollection<LeaderboardEntry> Entries { get; set; }
    }
}