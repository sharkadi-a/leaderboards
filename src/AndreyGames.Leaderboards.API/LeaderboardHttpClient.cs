using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndreyGames.Leaderboards.API
{
    public abstract class LeaderboardClientBase : ILeaderboardsClient
    {
        private readonly Uri _baseUrl;
        private readonly string _userName;
        private readonly string _password;
        
        protected abstract Task<string> PostJson(string json, Uri url, string basicAuthValue, CancellationToken token);
        
        protected abstract Task<string> GetJson(string json, Uri url, string basicAuthValue, CancellationToken token);

        public LeaderboardClientBase(Uri baseUrl, string userName, string password)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _userName = userName ?? throw new ArgumentNullException(nameof(userName));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }
        
        public async Task AddLeaderboard(string game, CancellationToken token = default)
        {
            var url = $"{_baseUrl}/{game}";
            var json = await PostJson(string.Empty, new Uri(url), CreateBasicAuthHeader(), token);
            
        }

        public Task<LeaderboardEntry> GetPlayerScore(string game, string playerName, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<LeaderboardView> GetLeaderboard(string game, int? offset = null, int? limit = null, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        public Task AddOrUpdateScore(string game, string playerName, int score, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        private string CreateBasicAuthHeader()
        {
            var value = $"{_userName}:{_password}";
            var bytes = Encoding.Default.GetBytes(value);
            var asciiBytes = Encoding.Convert(Encoding.Default, Encoding.ASCII, bytes);
            return Convert.ToBase64String(asciiBytes);
        }
    }
}