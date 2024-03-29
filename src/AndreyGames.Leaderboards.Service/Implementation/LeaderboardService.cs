﻿using System;
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

            var wherePlaceholder = BasicQueryBuilder<LeaderboardEntry>.WherePlaceholder;
            var pagingPlaceholder = BasicQueryBuilder<LeaderboardEntry>.PagingPlaceholder;
            
            var queryTemplate =
                $@"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber"" 
                    FROM ""Entries"" WHERE ""LeaderboardId"" = @LeaderboardId {wherePlaceholder})
                    SELECT ""RowNumber"" as ""Rank"", ""Score"", ""IsWinner"", ""PlayerName"" as ""Name"" FROM entries {pagingPlaceholder}";

            var queryBuilder = BasicQueryBuilder<LeaderboardEntry>
                .New(_context)
                .WithQueryTemplate(queryTemplate)
                .WithEnvelope("AND ({0})")
                .WithArbitraryParameter("LeaderboardId", leaderboard.Id)
                .WithPaging(limitValue, offsetValue);

            if (onlyWinners)
            {
                queryBuilder = queryBuilder.And("IsWinner", true);
            }
            
            if (start.HasValue && end.HasValue)
            {
                queryBuilder = queryBuilder.AndBetween("Timestamp", start.Value, end.Value);
            }

            var results = await queryBuilder.Execute();
            var entries = results.ToArray();
            
            return new LeaderboardView
            {
                Offset = entries.LastOrDefault()?.Rank ?? 0, 
                Entries = entries,
            };
        }

        public async Task<LeaderboardEntry> GetPlayerRank(string game, string playerName, bool caseInsensitive, DateTime? start = default, DateTime? end = default,
            bool onlyWinners = false)
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

            var wherePlaceholder = BasicQueryBuilder<ScoreItem>.WherePlaceholder;
            
            var queryTemplateCaseSensitive =
                $@"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber"" 
                    FROM ""Entries"" WHERE ""LeaderboardId"" = @LeaderboardId {wherePlaceholder})
                    SELECT ""RowNumber"" as ""Rank"", ""Score"", ""IsWinner"" FROM entries WHERE ""PlayerName"" = @PlayerName";

            var queryTemplateCaseInsensitive =
                $@"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber"" 
                    FROM ""Entries"" WHERE ""LeaderboardId"" = @LeaderboardId {wherePlaceholder})
                    SELECT ""RowNumber"" as ""Rank"", ""Score"", ""IsWinner"" FROM entries WHERE LOWER(""PlayerName"") = LOWER(@PlayerName)";
            
            var queryBuilder = BasicQueryBuilder<ScoreItem>
                .New(_context)
                .WithQueryTemplate(caseInsensitive ? queryTemplateCaseInsensitive : queryTemplateCaseSensitive)
                .WithEnvelope("AND ({0})")
                .WithArbitraryParameter("LeaderboardId", leaderboard.Id)
                .WithArbitraryParameter("PlayerName", playerName);

            if (onlyWinners)
            {
                queryBuilder = queryBuilder.And("IsWinner", true);
            }
            
            if (start.HasValue && end.HasValue)
            {
                queryBuilder = queryBuilder.AndBetween("Timestamp", start.Value, end.Value);
            }

            var results = await queryBuilder.Execute();

            return results.Select(x => new LeaderboardEntry
            {
                IsWinner = x.IsWinner,
                Rank = x.Rank,
                Score = x.Score,
                Name = playerName,
            }).DefaultIfEmpty(new LeaderboardEntry
            {
                IsWinner = false,
                Name = playerName,
                Rank = 0,
                Score = 0,
            }).FirstOrDefault();
        }

        private class ScoreItem
        {
            public long Score { get; set; }
            public int Rank { get; set; }
            public bool IsWinner { get; set; }
        }
        
        public async Task<ICollection<LeaderboardEntry>> GetScoreForPlayer(string game, string playerName, bool caseInsensitive)
        {
            var leaderboard = await _context
                .Leaderboards
                .FirstOrDefaultAsync(x => x.Game == game && x.IsActive);

            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            var entry = caseInsensitive
                ? leaderboard.Entries.FirstOrDefault(x => x.PlayerName.ToLower() == playerName.ToLower())
                : leaderboard.Entries.FirstOrDefault(x => x.PlayerName == playerName);

            if (entry is null)
            {
                return ArraySegment<LeaderboardEntry>.Empty;
            }

            const string caseSensitiveQuery =
                @"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber""
                                        FROM ""Entries"" WHERE ""LeaderboardId"" = @id)
                                        SELECT ""RowNumber"" as ""Rank"", ""Score"", ""IsWinner"" FROM entries WHERE ""PlayerName"" = @name";
            
            const string caseInsensitiveQuery = 
                @"WITH entries as (SELECT ""Entries"".*, row_number() OVER (ORDER BY ""Score"" DESC, ""Timestamp"" DESC) as ""RowNumber""
                                        FROM ""Entries"" WHERE ""LeaderboardId"" = @id)
                                        SELECT ""RowNumber"" as ""Rank"", ""Score"", ""IsWinner"" FROM entries WHERE LOWER(""PlayerName"") = LOWER(@name)";

            var scores = await _context.Database
                .GetDbConnection()
                .QueryAsync<ScoreItem>(
                    caseInsensitive ? caseInsensitiveQuery : caseSensitiveQuery,
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
        
        public async Task PutPlayerScore(string game, DateTime date, string playerName, long score,
            bool isWinner = false, bool isFraud = false)
        {
            var leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(x => x.Game == game && x.IsActive);
                
            if (leaderboard is null)
            {
                throw new LeaderboardNotFound(game);
            }

            if (isFraud)
            {
                // In case of fraud, do nothing
                return;
            }

            leaderboard.AddOrUpdateScore(playerName, date, score, isWinner);
        }
    }
}