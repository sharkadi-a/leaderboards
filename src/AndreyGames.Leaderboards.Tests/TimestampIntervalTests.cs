using System;
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

        [Fact]
        public async void GetLeaderboard_WhenOnlyToday_ShouldNotIncludeOthersAndHaveCorrectRank()
        {
            var utcNow = DateTime.UtcNow;
            var game = CreateGameName();

            var date1 = utcNow.Date;
            var player1 = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);

            var date2 = utcNow.Date.AddDays(1);
            var player2 = _faker.Person.FullName;
            var score2 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var date3 = utcNow.Date.AddDays(-1);
            var player3 = _faker.Person.FullName;
            var score3 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            _testEnvironment.Clock.CurrentTime = date1;
            await client.AddOrUpdateScore(game, player1, score1, false);

            _testEnvironment.Clock.CurrentTime = date2;
            await client.AddOrUpdateScore(game, player2, score2, false);

            _testEnvironment.Clock.CurrentTime = date3;
            await client.AddOrUpdateScore(game, player3, score3, false);

            var leaderboard = await client.GetLeaderboard(game, timeFrame: TimeFrame.Today);
            
            leaderboard.Entries.Length.ShouldBe(1);
            leaderboard.Entries.ShouldContain(x => x.Name == player1);
            leaderboard.Entries.Single().Rank.ShouldBe(1);
        }        
        
        [Fact]
        public async void GetLeaderboard_WhenThisWeek_ShouldNotIncludeOthersAndHaveCorrectRank()
        {
            var utcNow = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc);
            var game = CreateGameName();

            var date1 = utcNow.Date;
            var player1 = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);

            var date2 = utcNow.Date.AddDays(-1);
            var player2 = _faker.Person.FullName;
            var score2 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var date3 = utcNow.Date.AddDays(7);
            var player3 = _faker.Person.FullName;
            var score3 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            _testEnvironment.Clock.CurrentTime = date1;
            await client.AddOrUpdateScore(game, player1, score1, false);

            _testEnvironment.Clock.CurrentTime = date2;
            await client.AddOrUpdateScore(game, player2, score2, false);

            _testEnvironment.Clock.CurrentTime = date3;
            await client.AddOrUpdateScore(game, player3, score3, false);

            var leaderboard = await client.GetLeaderboard(game, timeFrame: TimeFrame.Today);
            
            leaderboard.Entries.Length.ShouldBe(1);
            leaderboard.Entries.ShouldContain(x => x.Name == player1);
            leaderboard.Entries.Single().Rank.ShouldBe(1);
        }        
        
        [Fact]
        public async void GetLeaderboard_WhenThisMonth_ShouldNotIncludeOthersAndHaveCorrectRank()
        {
            var utcNow = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            var game = CreateGameName();

            var date1 = utcNow.Date;
            var player1 = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);

            var date2 = utcNow.Date.AddDays(-1);
            var player2 = _faker.Person.FullName;
            var score2 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var date3 = utcNow.Date.AddMonths(1);
            var player3 = _faker.Person.FullName;
            var score3 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            _testEnvironment.Clock.CurrentTime = date1;
            await client.AddOrUpdateScore(game, player1, score1, false);

            _testEnvironment.Clock.CurrentTime = date2;
            await client.AddOrUpdateScore(game, player2, score2, false);

            _testEnvironment.Clock.CurrentTime = date3;
            await client.AddOrUpdateScore(game, player3, score3, false);

            var leaderboard = await client.GetLeaderboard(game, timeFrame: TimeFrame.Today);
            
            leaderboard.Entries.Length.ShouldBe(1);
            leaderboard.Entries.ShouldContain(x => x.Name == player1);
            leaderboard.Entries.Single().Rank.ShouldBe(1);
        }        
        
        [Fact]
        public async void GetLeaderboard_WhenThisYear_ShouldNotIncludeOthersAndHaveCorrectRank()
        {
            var utcNow = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var game = CreateGameName();

            var date1 = utcNow.Date;
            var player1 = _faker.Person.FullName;
            var score1 = (int)_faker.Finance.Amount(100, 10000, 0);

            var date2 = utcNow.Date.AddDays(-1);
            var player2 = _faker.Person.FullName;
            var score2 = (int)_faker.Finance.Amount(100, 10000, 0);

            var date3 = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var player3 = _faker.Person.FullName;
            var score3 = (int)_faker.Finance.Amount(100, 10000, 0);
            
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);

            _testEnvironment.Clock.CurrentTime = date1;
            await client.AddOrUpdateScore(game, player1, score1, false);

            _testEnvironment.Clock.CurrentTime = date2;
            await client.AddOrUpdateScore(game, player2, score2, false);

            _testEnvironment.Clock.CurrentTime = date3;
            await client.AddOrUpdateScore(game, player3, score3, false);

            var leaderboard = await client.GetLeaderboard(game, timeFrame: TimeFrame.Today);
            
            leaderboard.Entries.Length.ShouldBe(1);
            leaderboard.Entries.ShouldContain(x => x.Name == player1);
            leaderboard.Entries.Single().Rank.ShouldBe(1);
        }
    }
}