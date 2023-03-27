using System.Linq;
using AndreyGames.Leaderboards.API;
using Bogus;
using Shouldly;
using Xunit;

namespace AndreyGames.Leaderboards.Tests
{
    [Collection("Leaderboards API Tests")]
    public class TimestampIntervalTests : IClassFixture<TestEnvironment>
    {
        private readonly TestEnvironment _testEnvironment;
        private readonly Faker _faker = new();

        private string CreateGameName() => $"{_faker.Company.CompanyName()} {_faker.Commerce.Ean13()}";

        public TimestampIntervalTests(TestEnvironment testEnvironment)
        {
            _testEnvironment = testEnvironment;
        }

        [Theory]
        [InlineData(TimeFrame.Infinite)]
        [InlineData(TimeFrame.Today)]
        [InlineData(TimeFrame.Week)]
        [InlineData(TimeFrame.Year)]
        public async void GetLeaderboard_ForEachTimeFrame_ShouldReturnResults(TimeFrame timeFrame)
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score, false);

            var leaderboard = await client.GetLeaderboard(game, timeFrame: timeFrame);

            leaderboard.Entries.Length.ShouldBe(1);
            leaderboard.Entries.ShouldContain(x => x.Name == player);
        }
    }
}