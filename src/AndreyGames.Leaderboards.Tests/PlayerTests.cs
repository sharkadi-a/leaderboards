using System.Linq;
using Bogus;
using Shouldly;
using Xunit;

namespace AndreyGames.Leaderboards.Tests
{
    [Collection("Leaderboards API Tests")]
    public class PlayerTests : IClassFixture<TestEnvironment>
    {
        private readonly TestEnvironment _testEnvironment;
        private readonly Faker _faker = new();

        public PlayerTests(TestEnvironment testEnvironment)
        {
            _testEnvironment = testEnvironment;
        }

        private string CreateGameName() => $"{_faker.Company.CompanyName()} {_faker.Commerce.Ean13()}";

        [Fact]
        public async void CreateLeaderboard_ShouldSucceed()
        {
            var game = CreateGameName();
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
        }

        [Fact]
        public async void CreateLeaderboard_WhenCreatingTheSameLeaderboard_ShouldSucceed()
        {
            var game = CreateGameName();
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await Should.NotThrowAsync(async () => await client.AddLeaderboard(game));
        }

        [Fact]
        public async void AddScore_ThenGetScore_ShouldBeEqual()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score, false, false);

            var actualScores = await client.GetPlayerScore(game, player);

            actualScores.Count.ShouldBe(1);
            actualScores.Single().Score.ShouldBe(score);
        }

        [Fact]
        public async void AddScore_ThenUpdateHigherScore_ShouldBeSingleAndUpdatedResult()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);
            var score2 = (int)_faker.Finance.Amount(score1 + 1, 10001, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            await client.AddOrUpdateScore(game, player, score1, false, false);
            var actualScores1 = await client.GetPlayerScore(game, player);

            await client.AddOrUpdateScore(game, player, score2, false, false);
            var actualScores2 = await client.GetPlayerScore(game, player);

            actualScores1.Count.ShouldBe(1);
            actualScores2.Count.ShouldBe(1);

            actualScores1.Single().Score.ShouldBe(score1);
            actualScores2.Single().Score.ShouldBe(score2);
        }

        [Fact]
        public async void AddScore_ThenUpdateLowerScore_ShouldNotChangeScore()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);
            var score2 = (int)_faker.Finance.Amount(99, score1, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            await client.AddOrUpdateScore(game, player, score1, false, false);
            var actualScores1 = await client.GetPlayerScore(game, player);

            await client.AddOrUpdateScore(game, player, score2, false, false);
            var actualScores2 = await client.GetPlayerScore(game, player);

            actualScores1.Count.ShouldBe(1);
            actualScores2.Count.ShouldBe(1);

            actualScores1.Single().Score.ShouldBe(score1);
            actualScores2.Single().Score.ShouldNotBe(score2);
        }

        [Fact]
        public async void AddScore_WhenWinner_ShouldHaveWinnerAttribute()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score, true, false);

            var actualScores = await client.GetPlayerScore(game, player);

            actualScores.Count.ShouldBe(1);
            actualScores.Single().IsWinner.ShouldBeTrue();
        }

        [Fact]
        public async void AddScore_WhenWinnerAndNot_ShouldHaveBothResults()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);
            var score2 = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score1, false, false);
            await client.AddOrUpdateScore(game, player, score2, true, false);

            var actualScores = await client.GetPlayerScore(game, player);

            actualScores.Count.ShouldBe(2);
            actualScores.ShouldContain(x => !x.IsWinner && x.Score == score1);
            actualScores.ShouldContain(x => x.IsWinner && x.Score == score2);
        }
    }
}