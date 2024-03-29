using System;
using System.Linq;
using AndreyGames.Leaderboards.API;
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

        private string RandomizeLetters(string input) =>
            new(input.Select(x => _faker.Random.Bool() ? char.ToUpper(x) : char.ToLower(x)).ToArray());
        
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
        public async void AddScore_ThenGetScoreCaseInsensitive_ShouldBeEqual()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score, false, false);

            var actualScores = await client.GetPlayerScore(game, RandomizeLetters(player), true);

            actualScores.Count.ShouldBe(1);
            actualScores.Single().Score.ShouldBe(score);
            actualScores.Single().Name = player;
        }
        
        [Fact]
        public async void AddScore_WhenPlayerNameHasWrongCase_ThenGetScore_ShouldNotBeEqual()
        {
            var game = CreateGameName();
            var player = _faker.Person.FullName;
            var score = (int)_faker.Finance.Amount(100, 10000, 0);
            var client = _testEnvironment.CreateLeaderboardsClient();

            await client.AddLeaderboard(game);
            await client.AddOrUpdateScore(game, player, score, false, false);

            var actualScores = await client.GetPlayerScore(game, RandomizeLetters(player), false);

            actualScores.Count.ShouldBe(0);
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

        [Fact]
        public async void AddScores_ThenGetPlayerRank_ShouldBeSameAsInLeaderboard()
        {
            var game = CreateGameName();

            var players = Enumerable
                .Range(0, _faker.Random.Int(10, 100))
                .Select(_ => new Faker())
                .Select(faker => $"{faker.Person.FullName} {faker.Person.DateOfBirth}")
                .ToDictionary(x => x,
                    _ => new
                    {
                        Score = _faker.Random.Int(0, 1000000),
                        IsWinner = _faker.Random.Bool()
                    });
            
            var client = _testEnvironment.CreateLeaderboardsClient();
            var clock = _testEnvironment.Clock;

            await client.AddLeaderboard(game);
            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Key, player.Value.Score, player.Value.IsWinner, false);
                clock.CurrentTime = clock.CurrentTime.AddMinutes(1);
            }

            var randomPlayer = players.ElementAt(_faker.Random.Int(0, players.Count));
            var playerRank = await client.GetPlayerRank(game, randomPlayer.Key, winnersOnly: randomPlayer.Value.IsWinner);
            var wholeLeaderboard = await client.GetLeaderboard(game, winnersOnly: randomPlayer.Value.IsWinner, limit: players.Count);
            
            playerRank.Rank.ShouldBeGreaterThan(0);
            playerRank.Rank.ShouldBe(wholeLeaderboard.Entries.Single(x => x.Name == randomPlayer.Key).Rank);
        }  
        
        [Fact]
        public async void AddScores_ThenGetPlayerRankCaseInsensitive_ShouldBeSameAsInLeaderboard()
        {
            var game = CreateGameName();

            var players = Enumerable
                .Range(0, _faker.Random.Int(10, 100))
                .Select(_ => new Faker())
                .Select(faker => $"{faker.Person.FullName} {faker.Person.DateOfBirth}")
                .ToDictionary(x => x,
                    _ => new
                    {
                        Score = _faker.Random.Int(0, 1000000),
                        IsWinner = _faker.Random.Bool()
                    });
            
            var client = _testEnvironment.CreateLeaderboardsClient();
            var clock = _testEnvironment.Clock;

            await client.AddLeaderboard(game);
            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Key, player.Value.Score, player.Value.IsWinner, false);
                clock.CurrentTime = clock.CurrentTime.AddMinutes(1);
            }

            var randomPlayer = players.ElementAt(_faker.Random.Int(0, players.Count));
            var playerRank = await client.GetPlayerRank(game, randomPlayer.Key.ToUpper(), caseInsensitive: true, winnersOnly: randomPlayer.Value.IsWinner);
            var wholeLeaderboard = await client.GetLeaderboard(game, winnersOnly: randomPlayer.Value.IsWinner, limit: players.Count);
            
            playerRank.Rank.ShouldBeGreaterThan(0);
            playerRank.Rank.ShouldBe(wholeLeaderboard.Entries.Single(x => x.Name == randomPlayer.Key).Rank);
        }        
        
        [Fact]
        public async void AddScores_WhenPlayerHasWrongCase_ThenGetPlayerRankInTimeFrame_ShouldNotBeInLeaderboard()
        {
            var game = CreateGameName();

            var players = Enumerable
                .Range(0, _faker.Random.Int(10, 100))
                .Select(_ => new Faker())
                .Select(faker => $"{faker.Person.FullName} {faker.Person.DateOfBirth}")
                .ToDictionary(x => x,
                    _ => new
                    {
                        Score = _faker.Random.Int(0, 1000000),
                        IsWinner = _faker.Random.Bool()
                    });
            
            var client = _testEnvironment.CreateLeaderboardsClient();
            var clock = _testEnvironment.Clock;

            await client.AddLeaderboard(game);
            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Key, player.Value.Score, player.Value.IsWinner, false);
                clock.CurrentTime = clock.CurrentTime.AddMinutes(1);
            }

            var randomPlayer = players.ElementAt(_faker.Random.Int(0, players.Count));
            var playerRank = await client.GetPlayerRank(game, RandomizeLetters(randomPlayer.Key), winnersOnly: randomPlayer.Value.IsWinner, timeFrame: TimeFrame.Week);
            
            playerRank.Rank.ShouldBe(0);
        }  
        
        [Fact]
        public async void AddScores_ThenGetPlayerRankInTimeFrame_ShouldBeSameAsInLeaderboard()
        {
            var game = CreateGameName();

            var players = Enumerable
                .Range(0, _faker.Random.Int(10, 100))
                .Select(_ => new Faker())
                .Select(faker => $"{faker.Person.FullName} {faker.Person.DateOfBirth}")
                .ToDictionary(x => x,
                    _ => new
                    {
                        Score = _faker.Random.Int(0, 1000000),
                        IsWinner = _faker.Random.Bool()
                    });
            
            var client = _testEnvironment.CreateLeaderboardsClient();
            var clock = _testEnvironment.Clock;

            await client.AddLeaderboard(game);
            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Key, player.Value.Score, player.Value.IsWinner, false);
                clock.CurrentTime = clock.CurrentTime.AddMinutes(1);
            }

            var randomPlayer = players.ElementAt(_faker.Random.Int(0, players.Count));
            var playerRank = await client.GetPlayerRank(game, randomPlayer.Key, winnersOnly: randomPlayer.Value.IsWinner, timeFrame: TimeFrame.Week);
            var wholeLeaderboard = await client.GetLeaderboard(game, winnersOnly: randomPlayer.Value.IsWinner, limit: players.Count, timeFrame: TimeFrame.Week);
            
            playerRank.Rank.ShouldBeGreaterThan(0);
            playerRank.Rank.ShouldBe(wholeLeaderboard.Entries.Single(x => x.Name == randomPlayer.Key).Rank);
        }    
        
        [Fact]
        public async void AddScores_ThenGetPlayerRankInTimeFrameCaseInsensitive_ShouldBeSameAsInLeaderboard()
        {
            var game = CreateGameName();

            var players = Enumerable
                .Range(0, _faker.Random.Int(10, 100))
                .Select(_ => new Faker())
                .Select(faker => $"{faker.Person.FullName} {faker.Person.DateOfBirth}")
                .ToDictionary(x => x,
                    _ => new
                    {
                        Score = _faker.Random.Int(0, 1000000),
                        IsWinner = _faker.Random.Bool()
                    });
            
            var client = _testEnvironment.CreateLeaderboardsClient();
            var clock = _testEnvironment.Clock;

            await client.AddLeaderboard(game);
            foreach (var player in players)
            {
                await client.AddOrUpdateScore(game, player.Key, player.Value.Score, player.Value.IsWinner, false);
                clock.CurrentTime = clock.CurrentTime.AddMinutes(1);
            }

            var randomPlayer = players.ElementAt(_faker.Random.Int(0, players.Count - 1));
            var playerRank = await client.GetPlayerRank(game, RandomizeLetters(randomPlayer.Key), caseInsensitive: true, winnersOnly: randomPlayer.Value.IsWinner, timeFrame: TimeFrame.Week);
            var wholeLeaderboard = await client.GetLeaderboard(game, winnersOnly: randomPlayer.Value.IsWinner, limit: players.Count, timeFrame: TimeFrame.Week);
            
            playerRank.Rank.ShouldBeGreaterThan(0);
            playerRank.Rank.ShouldBe(wholeLeaderboard.Entries.Single(x => x.Name == randomPlayer.Key).Rank);
        }
    }
}