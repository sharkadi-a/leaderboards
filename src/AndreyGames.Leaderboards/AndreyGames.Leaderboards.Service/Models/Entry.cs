using System;
using Microsoft.EntityFrameworkCore;

namespace AndreyGames.Leaderboards.Service.Models
{
    [Index("LeaderboardId", nameof(PlayerName), IsUnique = true)]
    public class Entry
    {
        public long Id { get; set; }
        
        public virtual Leaderboard Leaderboard { get; set; }
        
        public string PlayerName { get; set; }
        
        public long Score { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}