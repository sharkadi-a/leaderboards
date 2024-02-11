using System;
using System.Threading.Tasks;
using AndreyGames.Leaderboards.API;
using AndreyGames.Leaderboards.Service.Abstract;
using AndreyGames.Leaderboards.Service.Api;
using AndreyGames.Leaderboards.Service.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace AndreyGames.Leaderboards.Service.Controllers
{
    [ApiController]
    [Route("/")]
    [FormatExceptions]
    [Produces("application/json")]
    [RequestCryptoProcessor]
    public class LeaderboardsController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly ITimeFrameConverter _timeFrameConverter;
        private readonly ISystemClock _systemClock;

        public LeaderboardsController(ILeaderboardService leaderboardService, 
            ITimeFrameConverter timeFrameConverter,
            ISystemClock systemClock)
        {
            _leaderboardService = leaderboardService;
            _timeFrameConverter = timeFrameConverter;
            _systemClock = systemClock;
        }

        [HttpPost("add")]
        [CommitOnOk]
        public async Task<LeaderboardApiResponse> AddLeaderboard([FromBody] AddLeaderboardRequest request)
        {
            if (!await _leaderboardService.Exist(request.Game))
            {
                await _leaderboardService.CreateLeaderboard(request.Game);
            }

            return new LeaderboardApiResponse();
        }

        [HttpPost("get")]
        public async Task<LeaderboardApiResponse> GetLeaderboard([FromBody] GetLeaderboardsRequest request)
        {
            var startDate = _timeFrameConverter.GetStartDate(request.Time ?? TimeFrame.Infinite);
            var endDate = _timeFrameConverter.GetEndDate(request.Time ?? TimeFrame.Infinite);

            var view = await _leaderboardService.GetLeaderboard(request.Game,
                startDate,
                endDate,
                request.WinnersOnly,
                request.Offset,
                request.Limit);

            return new LeaderboardApiResponse(view);
        }

        [HttpPost("score/get")]
        public async Task<LeaderboardApiResponse> GetPlayerScore([FromBody] GetPlayerScoreRequest request)
        {
            return new LeaderboardApiResponse(
                await _leaderboardService.GetScoreForPlayer(request.Game, request.PlayerName));
        }

        [HttpPost("score/put")]
        [CommitOnOk]
        public async Task<LeaderboardApiResponse> AddOrUpdateScore([FromBody] AddOrUpdateScoreRequest request)
        {
            await _leaderboardService.PutPlayerScore(request.Game,
                _systemClock.UtcNow(),
                request.PlayerName,
                request.Score,
                request.IsWinner,
                request.IsFraud);
            
            return new LeaderboardApiResponse();
        }

        [HttpPost("score/rank")]
        public async Task<LeaderboardApiResponse> GetPlayerRank([FromBody] GetPlayerRank request)
        {
            var startDate = _timeFrameConverter.GetStartDate(request.Time ?? TimeFrame.Infinite);
            var endDate = _timeFrameConverter.GetEndDate(request.Time ?? TimeFrame.Infinite);

            var data = await _leaderboardService.GetPlayerRank(request.Game,
                request.PlayerName,
                startDate,
                endDate,
                request.WinnersOnly);

            return new LeaderboardApiResponse(data);
        }
    }
}