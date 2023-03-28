using System;
using System.Collections.Generic;
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

        public async Task<LeaderboardView> GetLeaderboard(string game, DateTime? start = default, DateTime? end = default, bool onlyWinners = false,
            int? offset = null, int? limit = null)
        {
            if (start.HasValue && end.HasValue)
            {
                if (start.Value > end.Value)
                {
                    throw new InvalidTimeframeException();
                }
            }
            
            var leaderboard = await _context
                .Leaderboards
                .FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            var offsetValue = Math.Max(0, offset ?? 0);
            var limitValue = Math.Max(1, limit ?? 20);

            IEnumerable<Entry> entries = leaderboard.Entries;
            
            if (start.HasValue)
            {
                entries = entries.Where(x => x.Timestamp >= start.Value);
            }

            if (end.HasValue)
            {
                entries = entries.Where(x => x.Timestamp < end.Value);
            }
            
            if (onlyWinners)
            {
                entries = entries.Where(x => x.IsWinner);
            }

            entries = entries.OrderByDescending(x => x.Score)
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
                    IsWinner = x.IsWinner,
                }).ToArray(),
            };
        }

        private class ScoreItem
        {
            public long Score { get; set; }
            public int Rank { get; set; }
            public bool IsWinner { get; set; }
        }
        
        public async Task<ICollection<LeaderboardEntry>> GetScoreForPlayer(string game, string playerName)
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
                return ArraySegment<LeaderboardEntry>.Empty;
            }

            var scores = await _context.Database
                .GetDbConnection()
                .QueryAsync<ScoreItem>(
                    @"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber""
                                        FROM ""Entries"" WHERE ""LeaderboardId"" = @id)
                                        SELECT ""RowNumber"" as ""Rank"", ""Score"", ""IsWinner"" FROM entries WHERE ""PlayerName"" = @name", 
                    new
                    {
                        id = leaderboard.Id,
                        name = playerName,
                    });

            var list = new LinkedList<LeaderboardEntry>();
            foreach (var score in scores)
            {
                list.AddLast(new LeaderboardEntry
                {
                    Name = entry.PlayerName,
                    Rank = score.Rank,
                    Score = score.Score,
                    IsWinner = score.IsWinner
                });
            }

            return list;
        }
        
        public async Task PutPlayerScore(string game, DateTime date, string playerName, long score, bool isWinner = false)
        {
            var leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            leaderboard.AddOrUpdateScore(playerName, date, score, isWinner);
        }
    }
}