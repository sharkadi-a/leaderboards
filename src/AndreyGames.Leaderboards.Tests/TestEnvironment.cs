﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AndreyGames.Leaderboards.API;
using AndreyGames.Leaderboards.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AndreyGames.Leaderboards.Tests
{
    public class TestEnvironment : WebApplicationFactory<Startup>
    {
        private class Client : LeaderboardClientBase
        {
            private readonly HttpClient _httpClient;

            public static Client Create(HttpClient httpClient, IConfiguration configuration)
            {
                var iv = configuration["CryptoVectorString"];
                var auth = configuration.GetSection("Auth").GetChildren().First();
                return new Client(httpClient, auth["UserName"], auth["Password"], iv);
            }

            private Client(HttpClient httpClient, string userName, string password, string seed) : base("/", userName,
                password, seed)
            {
                _httpClient = httpClient;
                _httpClient.DefaultRequestHeaders.Add(UserHeaderName, userName);
            }

            async protected Task<TResult> Post<TRequest, TResult>(string fullUrl, TRequest request,
                CancellationToken token)
            {
                var httpResponseMessage = await _httpClient.PostAsJsonAsync(fullUrl, request, cancellationToken: token);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var response = await httpResponseMessage.Content.ReadFromJsonAsync<TResult>(cancellationToken: token);
                    return response;
                }

                var obj = JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                var dict = obj["data"].ToDictionary(x => x.First.Value<string>(), x => x.Last.Value<string>());
                throw new ApiException(obj["message"].Value<string>(), dict);
            }

            async protected Task Post<TRequest>(string fullUrl, TRequest request, CancellationToken token)
            {
                var httpResponseMessage = await _httpClient.PostAsJsonAsync(fullUrl, request, cancellationToken: token);

                if (httpResponseMessage.IsSuccessStatusCode) return;

                var obj = JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
                var dict = obj["data"].ToDictionary(x => x.First.Value<string>(), x => x.Last.Value<string>());
                throw new ApiException(obj["message"].Value<string>(), dict);
            }

            protected override Task AddLeaderboard(string fullUrl, LeaderboardCryptoRequest request, CancellationToken token = default)
            {
                return Post(fullUrl, request, token);
            }

            protected override Task<ICollection<LeaderboardEntry>> GetPlayerScore(string fullUrl, LeaderboardCryptoRequest request, CancellationToken token = default)
            {
                return Post<LeaderboardCryptoRequest, ICollection<LeaderboardEntry>>(fullUrl, request, token);
            }

            protected override Task<LeaderboardView> GetLeaderboard(string fullUrl, LeaderboardCryptoRequest request, CancellationToken token = default)
            {
                return Post<LeaderboardCryptoRequest, LeaderboardView>(fullUrl, request, token);
            }

            protected override Task AddOrUpdateScore(string fullUrl, LeaderboardCryptoRequest request, CancellationToken token = default)
            {
                return Post(fullUrl, request, token);
            }

            protected override byte[] SerializeJsonBytes(object request)
            {
                return Encoding.Default.GetBytes(JsonSerializer.Serialize(request));
            }
        }

        private class LeaderboardsClientWrapper : ILeaderboardsClient
        {
            private readonly ILeaderboardsClient _leaderboardsClientImplementation;

            private readonly HashSet<string> _leaderboards = new();

            public IReadOnlyCollection<string> CreatedLeaderboards => _leaderboards;

            public LeaderboardsClientWrapper(ILeaderboardsClient leaderboardsClientImplementation)
            {
                _leaderboardsClientImplementation = leaderboardsClientImplementation;
            }

            public async Task AddLeaderboard(string game, CancellationToken token = default)
            {
                await _leaderboardsClientImplementation.AddLeaderboard(game, token);
                _leaderboards.Add(game);
            }

            public Task<ICollection<LeaderboardEntry>> GetPlayerScore(string game, string playerName,
                CancellationToken token = default)
            {
                return _leaderboardsClientImplementation.GetPlayerScore(game, playerName, token);
            }

            public Task<LeaderboardView> GetLeaderboard(string game, bool winnersOnly = false, int? offset = null,
                int? limit = null,
                CancellationToken token = default)
            {
                return _leaderboardsClientImplementation.GetLeaderboard(game, winnersOnly, offset, limit, token);
            }

            public Task AddOrUpdateScore(string game, string playerName, long score, bool isWinner,
                CancellationToken token = default)
            {
                return _leaderboardsClientImplementation.AddOrUpdateScore(game, playerName, score, isWinner, token);
            }
        }

        private LinkedList<LeaderboardsClientWrapper> _clients = new();

        public ILeaderboardsClient CreateLeaderboardsClient()
        {
            var configuration = Services.GetRequiredService<IConfiguration>();
            var client = Client.Create(CreateClient(), configuration);
            var wrapper = new LeaderboardsClientWrapper(client);
            _clients.AddLast(wrapper);
            return wrapper;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            const string connectionString =
                "***REMOVED***";
            const string vector = "Test";

            var config = new TestConfigFileBuilder()
                .AddApiUser("user", "user")
                .UseCryptoVectorString(vector)
                .UseConnectionString(connectionString);

            builder.UseTestServer()
                .ConfigureLogging(logging => logging.ClearProviders())
                .ConfigureAppConfiguration(appBuilder => appBuilder.AddJsonStream(config.Build()));
        }

        protected override void Dispose(bool disposing)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LeaderboardContext>();
            using var tran = db.Database.BeginTransaction();

            foreach (var game in _clients.SelectMany(x => x.CreatedLeaderboards))
            {
                db.Database.ExecuteSqlRaw("DELETE FROM \"Entries\" WHERE \"LeaderboardId\" IN " +
                                          "(SELECT \"Id\" FROM \"Leaderboards\" WHERE \"Game\" = {0})", game);
                db.Database.ExecuteSqlRaw("DELETE FROM \"Leaderboards\" WHERE \"Game\" = {0}", game);
            }
            
            tran.Commit();

            base.Dispose(disposing);
        }
    }
}