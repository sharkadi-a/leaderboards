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
    public class LeaderboardsController: ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardsController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
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
        public async Task<LeaderboardApiResponse> GetLeaderboard([FromBody] GetLeaderboardRequest request)
        {
            var view = await _leaderboardService.GetLeaderboard(request.Game, 
                request.WinnersOnly, 
                request.Offset, 
                request.Limit);

            return new LeaderboardApiResponse(view);
        }

        [HttpPost("score/get")]
        public async Task<LeaderboardApiResponse> GetPlayerScore([FromBody] GetPlayerScoreRequest request)
        {
            return new LeaderboardApiResponse(await _leaderboardService.GetScoreForPlayer(request.Game, request.PlayerName));
        }

        [HttpPost("score/put")]
        [CommitOnOk]
        public async Task<LeaderboardApiResponse> AddOrUpdateScore([FromBody] AddOrUpdateScoreRequest request)
        {
            await _leaderboardService.PutPlayerScore(request.Game, request.PlayerName, request.Score, request.IsWinner);
            return new LeaderboardApiResponse();
        }
    }
}