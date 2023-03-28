using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Shouldly;
using Xunit;

namespace AndreyGames.Leaderboards.Tests
{
    [Collection("Leaderboards API Tests")]
    public class LeaderboardTests : IClassFixture<TestEnvironment>
    {
        private readonly TestEnvironment _testEnvironment;
        private readonly Faker _faker = new();

        public LeaderboardTests(TestEnvironment testEnvironment)
        {
            _testEnvironment = testEnvironment;
        }

        private string CreateGameName() => $"{_faker.Company.CompanyName()} {_faker.Commerce.Ean13()}";

        private IEnumerable<T> GenerateUniqueRandom<T>(Func<T> producer, int count)
        {
            var set = new HashSet<T>();
            while (set.Count < count)
            {
                var item = producer();
                if (!set.Contains(item))
                {
                    set.Add(item);
                }
            }

            return set;
        }

        [Fact]
        public async void GetLeaderboard_WhenNoScores_ShouldBeEmpty()
        {
            var game = CreateGameName();
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            var leaderboard = await client.GetLeaderboard(game);

            leaderboard.Entries.ShouldBeEmpty();
        }

        [Fact]
        public async void AddScore_ThenGetLeaderboard_ShouldContainPlayerAndScore()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score, false);

            var leaderboard = await client.GetLeaderboard(game);

            leaderboard.Entries.ShouldContain(x => x.Name == player && x.Score == score);
        }

        [Fact]
        public async void AddScore_ThenUpdateHigherScoreAndGetLeaderboard_ShouldContainPlayerAndScore()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);
            var score2 = (int)_faker.Finance.Amount(score1, 10001, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            await client.AddOrUpdateScore(game, player, score1, false);
            var leaderboard1 = await client.GetLeaderboard(game);
            var result1 = leaderboard1.Entries.Single(x => x.Name == player);

            await client.AddOrUpdateScore(game, player, score2, false);
            var leaderboard2 = await client.GetLeaderboard(game);
            var result2 = leaderboard2.Entries.Single(x => x.Name == player);

            result1.Score.ShouldBe(score1);
            result2.Score.ShouldBe(score2);
        }

        [Fact]
        public async void AddScore_ThenUpdateLowerScoreAndGetLeaderboard_ShouldContainPlayerAndScore()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);
            var score2 = (int)_faker.Finance.Amount(99, score1, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            await client.AddOrUpdateScore(game, player, score1, false);
            var leaderboard1 = await client.GetLeaderboard(game);
            var result1 = leaderboard1.Entries.Single(x => x.Name == player);

            await client.AddOrUpdateScore(game, player, score2, false);
            var leaderboard2 = await client.GetLeaderboard(game);
            var result2 = leaderboard2.Entries.Single(x => x.Name == player);

            result1.Score.ShouldBe(score1);
            result2.Score.ShouldNotBe(score2);
        }

        [Fact]
        public async void AddScore_WhenManyResults_ShouldHaveCorrectOrder()
        {
            var playerCount = 10;
            var game = CreateGameName();
            var randomScores = GenerateUniqueRandom(() => _faker.Random.Long(0), playerCount);
            var scores = new Queue<long>(randomScores);

            var players = Enumerable.Range(0, playerCount).Select(x => new
            {
                Name = $"{new Faker().Person.FullName} {new Faker().Person.DateOfBirth}",
                Score = scores.Dequeue(),
                IsWinner = _faker.Random.Bool(),
            }).ToArray();

            var client = _testEnvironment.CreateLeaderboardsClient();
            await client.AddLeaderboard(game);

            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Name, player.Score, player.IsWinner);
            }

            var leaderboard = await client.GetLeaderboard(game, limit: playerCount);

            leaderboard.Entries.Length.ShouldBe(playerCount);

            var expectedDescending = players.OrderByDescending(x => x.Score).ToArray();
            var actual = leaderboard.Entries.ToArray();

            for (var i = 0; i < expectedDescending.Length; i++)
            {
                actual[i].Rank.ShouldBe(i + 1);
                expectedDescending[i].Name.ShouldBe(actual[i].Name);
                expectedDescending[i].Score.ShouldBe(actual[i].Score);
                expectedDescending[i].IsWinner.ShouldBe(actual[i].IsWinner);
            }
        }

        [Fact]
        public async void AddScore_WhenManyResultsAndAllNotWinner_ShouldBeEmptyWhenOnlyWinnersRequested()
        {
            var playerCount = 10;
            var game = CreateGameName();
            var randomScores = GenerateUniqueRandom(() => _faker.Random.Long(0), playerCount);
            var scores = new Queue<long>(randomScores);

            var players = Enumerable.Range(0, playerCount).Select(x => new
            {
                Name = $"{new Faker().Person.FullName} {new Faker().Person.DateOfBirth}",
                Score = scores.Dequeue(),
            }).ToArray();

            var client = _testEnvironment.CreateLeaderboardsClient();
            await client.AddLeaderboard(game);

            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Name, player.Score, false);
            }

            var leaderboard = await client.GetLeaderboard(game, winnersOnly: true, limit: playerCount);

            leaderboard.Entries.ShouldBeEmpty();
        }

        [Fact]
        public async void AddScore_WhenManyResultsAndSomeAreWinners_ShouldReturnsOnlyWinners()
        {
            const int total = 20;
            var winnersCount = _faker.Random.Int(1, total - 1);
            var nonWinnersCount = total - winnersCount;

            var game = CreateGameName();
            var randomScores = GenerateUniqueRandom(() => _faker.Random.Long(0), total);
            var scores = new Queue<long>(randomScores);

            var winners = Enumerable.Range(0, winnersCount).Select(x => new
            {
                Name = $"{new Faker().Person.FullName} {new Faker().Person.DateOfBirth}",
                Score = scores.Dequeue(),
                IsWinner = true,
            }).ToArray();

            var nonWinners = Enumerable.Range(0, nonWinnersCount).Select(x => new
            {
                Name = $"{new Faker().Person.FullName} {new Faker().Person.DateOfBirth}",
                Score = scores.Dequeue(),
                IsWinner = false,
            }).ToArray();

            var client = _testEnvironment.CreateLeaderboardsClient();
            await client.AddLeaderboard(game);

            foreach (var player in winners.Union(nonWinners))
            {
                await client.AddOrUpdateScore(game, player.Name, player.Score, player.IsWinner);
            }

            var leaderboard = await client.GetLeaderboard(game, limit: total);
            var entries = leaderboard.Entries.ToArray();

            entries.Length.ShouldBe(total);
            entries.Count(x => x.IsWinner).ShouldBe(winners.Length);
            entries.Count(x => !x.IsWinner).ShouldBe(nonWinners.Length);
        }
    }
}