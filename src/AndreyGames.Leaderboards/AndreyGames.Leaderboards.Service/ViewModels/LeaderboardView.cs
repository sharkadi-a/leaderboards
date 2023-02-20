using System.Collections.Generic;

namespace AndreyGames.Leaderboards.Service.ViewModels
{
    public class LeaderboardView
    {
        public class Entry
        {
            public int Rank { get; set; }
            public string Name { get; set; }
            public long Score { get; set; }
        }

        public int Offset { get; set; }
        public ICollection<Entry> Entries { get; set; }
    }
}