using System;
using System.Linq;
using System.Threading.Tasks;
using AndreyGames.Leaderboards.API;
using AndreyGames.Leaderboards.Service.Abstract;
using AndreyGames.Leaderboards.Service.Exceptions;
using AndreyGames.Leaderboards.Service.Models;
using Dapper;
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
            var leaderboard = await _context
                .Leaderboards
                .FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            var offsetValue = offset ?? 0;
            var limitValue = limit ?? 20;

            var entries = leaderboard.Entries
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Timestamp)
                .Skip(offsetValue)
                .Take(limitValue);
            
            var counter = offsetValue;
            
            return new LeaderboardView
            {
                Offset = offsetValue,
                Entries = entries.Select(x => new LeaderboardEntry
                {
                    Name = x.PlayerName,
                    Score = x.Score,
                    Rank = ++counter,
                }).ToList(),
            };
        }

        public async Task<LeaderboardEntry> GetScoreForPlayer(string game, string playerName)
        {
            var leaderboard = await _context
                .Leaderboards
                .FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            var entry = leaderboard.Entries.FirstOrDefault(x => x.PlayerName == playerName);

            if (entry is null)
            {
                return new LeaderboardEntry
                {
                    Name = playerName,
                };
            }

            var rank = await _context.Database
                .GetDbConnection()
                .QueryFirstOrDefaultAsync<int>(
                    @"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber""
                                        FROM ""Entries"" WHERE ""LeaderboardId"" = @id)
                                        SELECT ""RowNumber"" FROM entries WHERE ""PlayerName"" = @name",
                    new
                    {
                        id = leaderboard.Id,
                        name = playerName,
                    });

            return new LeaderboardEntry
            {
                Name = entry.PlayerName,
                Score = entry.Score,
                Rank = rank,
            };
        }

        public async Task PutPlayerScore(string game, string playerName, long score)
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