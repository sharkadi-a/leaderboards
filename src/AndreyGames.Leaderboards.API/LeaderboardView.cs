using System;
using System.Collections.Generic;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// A leaderboard.
    /// </summary>
    [Serializable]
    public class LeaderboardView
    {
        /// <summary>
        /// Current paging offset
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Leaderboard entries
        /// </summary>
        public LeaderboardEntry[] Entries { get; set; }
    }
}