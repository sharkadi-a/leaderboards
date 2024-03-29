﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Base client for the Leaderboard Client. Inherit this class and implement required methods.
    /// </summary>
    public abstract class LeaderboardsClientBase : ILeaderboardsClient
    {
        /// <summary>
        /// Auth header
        /// </summary>
        protected string UserHeaderName => "User";

        /// <summary>
        /// User name for the service
        /// </summary>
        protected string UserName { get; }

        private readonly string _baseUrl;
        private readonly string _password;
        private readonly string _seed;
        private Action<string, object[]> _log;

        private readonly CryptoService _cryptoService = new();

        protected abstract Task AddLeaderboard(string fullUrl,
            LeaderboardsCryptoRequest request,
            CancellationToken token = default);

        protected abstract Task<ICollection<LeaderboardEntry>> GetPlayerScore(string fullUrl,
            LeaderboardsCryptoRequest request,
            CancellationToken token = default);

        protected abstract Task<LeaderboardEntry> GetPlayerRank(string fullUrl,
            LeaderboardsCryptoRequest request,
            CancellationToken token = default);

        protected abstract Task<LeaderboardView> GetLeaderboard(string fullUrl,
            LeaderboardsCryptoRequest request,
            CancellationToken token = default);

        protected abstract Task AddOrUpdateScore(string fullUrl,
            LeaderboardsCryptoRequest request,
            CancellationToken token = default);

        protected abstract void LogFormat(string message, params object[] args);

        /// <summary>
        /// Serializes object to a byte array
        /// </summary>
        protected abstract byte[] SerializeJsonBytes<T>(T data);

        private string CreateUrl(string path)
        {
            return $"{_baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        public LeaderboardsClientBase(string baseUrl, string userName, string password, string seed)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _seed = seed ?? throw new ArgumentNullException(nameof(seed));
        }

        public Task AddLeaderboard(string game, CancellationToken token = default)
        {
            const string path = "/add";
            var url = CreateUrl(path);

            LogFormat("Executing AddLeaderboard command on URL '{0}', game=[{1}]", url, game);

            var json = SerializeJsonBytes(new AddLeaderboardRequest
            {
                Game = game
            });

            var request = new LeaderboardsCryptoRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return AddLeaderboard(url, request, token);
        }

        public Task<ICollection<LeaderboardEntry>> GetPlayerScore(string game, 
            string playerName, 
            bool caseInsensitive = false, 
            CancellationToken token = default)
        {
            const string path = "/score/get";
            var url = CreateUrl(path);

            LogFormat("Executing GetPlayerScore command on URL '{0}', game=[{1}], playerName=[2]", url, game, playerName);

            var json = SerializeJsonBytes(new GetPlayerScoreRequest
            {
                Game = game,
                PlayerName = playerName,
                CaseInsensitive = caseInsensitive,
            });

            var request = new LeaderboardsCryptoRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };
            
            return GetPlayerScore(url, request, token);
        }

        public Task<LeaderboardEntry> GetPlayerRank(string game, 
            string playerName, 
            bool caseInsensitive = false,
            bool winnersOnly = false, 
            TimeFrame? timeFrame = default,
            CancellationToken token = default)
        {
            const string path = "/score/rank";
            var url = CreateUrl(path);

            LogFormat(
                "Executing GetPlayerOffset command on URL '{0}', game=[{1}], winnersOnly=[{2}], timeFrame=[{3}]",
                url, game, winnersOnly, timeFrame);

            var json = SerializeJsonBytes(new GetPlayerRankRequest
            {
                Game = game,
                PlayerName = playerName,
                CaseInsensitive = caseInsensitive,
                WinnersOnly = winnersOnly,
                Time = timeFrame,
            });

            var request = new LeaderboardsCryptoRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return GetPlayerRank(url, request, token);        
        }

        public Task<LeaderboardView> GetLeaderboard(string game, 
            bool winnersOnly = false, 
            TimeFrame? timeFrame = default,
            int? offset = null,
            int? limit = null,
            CancellationToken token = default)
        {
            const string path = "/get";
            var url = CreateUrl(path);

            LogFormat(
                "Executing GetLeaderboard command on URL '{0}', game=[{1}], winnersOnly=[{2}], timeFrame=[{3}] offset=[{4}], limit=[{5}]",
                url, game, winnersOnly, timeFrame, offset, limit);

            var json = SerializeJsonBytes(new GetLeaderboardsRequest
            {
                Game = game,
                WinnersOnly = winnersOnly,
                Offset = offset,
                Limit = limit,
                Time = timeFrame,
            });

            var request = new LeaderboardsCryptoRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return GetLeaderboard(url, request, token);
        }

        public Task AddOrUpdateScore(string game, string playerName, long score, bool isWinner, bool isFraud,
            CancellationToken token = default)
        {
            const string path = "/score/put";
            var url = CreateUrl(path);

            LogFormat(
                "Executing AddOrUpdateScore command on URL '{0}', game=[{1}], playerName=[{2}], score=[{3}], isWinner=[{4}]",
                url, game, playerName, score, isWinner);

            var json = SerializeJsonBytes(new AddOrUpdateScoreRequest
            {
                Game = game,
                IsWinner = isWinner,
                PlayerName = playerName,
                Score = score,
                IsFraud = isFraud,
            });

            var request = new LeaderboardsCryptoRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return AddOrUpdateScore(url, request, token);
        }
    }
}