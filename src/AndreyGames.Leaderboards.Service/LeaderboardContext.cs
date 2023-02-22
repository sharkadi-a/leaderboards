using AndreyGames.Leaderboards.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace AndreyGames.Leaderboards.Service
{
    public class LeaderboardContext : DbContext
    {
        public LeaderboardContext(DbContextOptions<LeaderboardContext> options) : base(options)
        {
            
        }
        
        public DbSet<Leaderboard> Leaderboards { get; set; }
        
        public DbSet<Entry> Entries { get; set; }
    }
}