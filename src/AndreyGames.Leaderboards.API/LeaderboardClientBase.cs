using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Base client for the Leaderboard Client. Inherit this class and implement required methods.
    /// </summary>
    public abstract class LeaderboardClientBase : ILeaderboardsClient
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

        private readonly CryptoService _cryptoService = new();

        protected abstract Task AddLeaderboard(string fullUrl, 
            LeaderboardCryptoRequestBase request,
            CancellationToken token = default);

        protected abstract Task<ICollection<LeaderboardEntry>> GetPlayerScore(string fullUrl, 
            LeaderboardCryptoRequestBase request,
            CancellationToken token = default);

        protected abstract Task<LeaderboardView> GetLeaderboard(string fullUrl, 
            LeaderboardCryptoRequestBase request,
            CancellationToken token = default);

        protected abstract Task AddOrUpdateScore(string fullUrl,
            LeaderboardCryptoRequestBase request,
            CancellationToken token = default);

        /// <summary>
        /// Serializes object to a byte array
        /// </summary>
        protected abstract byte[] SerializeJsonBytes(object request);

        private string CreateUrl(string path)
        {
            return $"{_baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        public LeaderboardClientBase(string baseUrl, string userName, string password, string seed)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _seed = seed ?? throw new ArgumentNullException(nameof(seed));
        }

        public Task AddLeaderboard(string game, CancellationToken token = default)
        {
            const string path = "/add";
            
            var json = SerializeJsonBytes(new AddLeaderboardRequest
            {
                Game = game
            });

            var request = new AddLeaderboardRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return AddLeaderboard(CreateUrl(path), request, token);
        }

        public Task<ICollection<LeaderboardEntry>> GetPlayerScore(string game, string playerName,
            CancellationToken token = default)
        {
            const string path = "/score/get";
            
            var json = SerializeJsonBytes(new GetPlayerScoreRequest
            {
                Game = game,
                PlayerName = playerName
            });

            var request = new GetPlayerScoreRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return GetPlayerScore(CreateUrl(path), request, token);
        }

        public Task<LeaderboardView> GetLeaderboard(string game, bool winnersOnly = false, int? offset = null,
            int? limit = null,
            CancellationToken token = default)
        {
            const string path = "/get";
            
            var json = SerializeJsonBytes(new GetLeaderboardRequest
            {
                Game = game,
                WinnersOnly = winnersOnly,
                Offset = offset,
                Limit = limit,
            });

            var request = new GetLeaderboardRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return GetLeaderboard(CreateUrl(path), request, token);
        }

        public Task AddOrUpdateScore(string game, string playerName, long score, bool isWinner,
            CancellationToken token = default)
        {
            const string path = "/score/put";
            
            var json = SerializeJsonBytes(new AddOrUpdateScoreRequest
            {
                Game = game,
                IsWinner = isWinner,
                PlayerName = playerName,
                Score = score,
            });

            var request = new AddOrUpdateScoreRequest
            {
                Body = _cryptoService.EncryptAsBase64(json,
                    _password,
                    _seed),
            };

            return AddOrUpdateScore(CreateUrl(path), request, token);
        }
    }
}