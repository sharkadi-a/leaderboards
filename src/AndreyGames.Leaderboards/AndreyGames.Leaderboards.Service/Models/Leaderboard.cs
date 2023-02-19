using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AndreyGames.Leaderboards.Service.Models
{
    [Index(nameof(Game), IsUnique = true)]
    public class Leaderboard
    {
        public int Id { get; set; }
        
        public string Game { get; set; }
        
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        
        public ICollection<Entry> Entries { get; set; }

        public virtual void AddOrUpdateScore(string playerName, int score)
        {
            var existing = Entries.FirstOrDefault(x => x.PlayerName == playerName);
            if (existing is null)
            {
                Entries.Add(new Entry
                {
                    PlayerName = playerName,
                    Score = score,
                    Timestamp = DateTime.Now,
                });
            }
            else
            {
                existing.Score = score;
                existing.Timestamp = DateTime.Now;
            }
        }
    }
}