using System;
using System.Linq;
using System.Threading.Tasks;
using AndreyGames.Leaderboards.Service.Abstract;
using AndreyGames.Leaderboards.Service.Exceptions;
using AndreyGames.Leaderboards.Service.Models;
using AndreyGames.Leaderboards.Service.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AndreyGames.Leaderboards.Service.Implementation
{
    internal class LeaderboardService : ILeaderboardService
    {
        private readonly LeaderboardContext _context;

        public LeaderboardService(LeaderboardContext context)
        {
            _context = context;
        }

        public Task<bool> Exist(string game)
        {
            return _context.Leaderboards.AnyAsync(x => x.Game == game);
        }

        public async Task CreateLeaderboard(string game)
        {
            if (await Exist(game))
            {
                throw new LeaderboardAlreadyExist(game);
            }
            
            var leaderboard = new Leaderboard
            {
                Game = game,
                IsActive = true,
            };

            await _context.Leaderboards.AddAsync(leaderboard);
        }
        
        public async Task<LeaderboardView> GetLeaderboard(string game, int? offset = null, int? limit = null)
        {
            var leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            var offsetValue = offset ?? 0;
            var limitValue = limit ?? 20;
            
            var entries = leaderboard.Entries
                .OrderByDescending(x => x.Score)
                .Skip(offsetValue)
                .Take(limitValue);

            var totalEntries = leaderboard.Entries.Count;
            
            var counter = totalEntries - offsetValue;
            
            return new LeaderboardView
            {
                Offset = offsetValue,
                Entries = entries.Select(x => new LeaderboardView.Entry
                {
                    Name = x.PlayerName,
                    Score = x.Score,
                    Rank = counter++,
                }).ToList(),
            };
        }

        public async Task AddOrUpdateScore(string game, string playerName, int score)
        {
            var leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            leaderboard.AddOrUpdateScore(playerName, score);
        }
    }
}