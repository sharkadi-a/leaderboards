using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AndreyGames.Leaderboards.Service.Models
{
    [Index(nameof(Game), nameof(IsActive), IsUnique = true)]
    public class Leaderboard
    {
        public int Id { get; set; }
        
        public string Game { get; set; }
        
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

        public virtual Entry AddOrUpdateScore(string playerName, DateTime date, long score, bool isWinner = false)
        {
            var existing = Entries.FirstOrDefault(x => x.PlayerName == playerName && x.IsWinner == isWinner);
            if (existing is null)
            {
                var entry = new Entry
                {
                    PlayerName = playerName,
                    Score = score,
                    Timestamp = date,
                    IsWinner = isWinner,
                    Leaderboard = this,
                };
                
                Entries.Add(entry);

                return entry;
            }
            
            if (existing.Score < score && existing.Timestamp <= date)
            {
                existing.Score = score;
                existing.Timestamp = DateTime.Now;
            }

            return existing;
        }
    }
}