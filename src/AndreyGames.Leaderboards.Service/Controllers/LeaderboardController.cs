using System.Threading.Tasks;
using AndreyGames.Leaderboards.Service.Abstract;
using AndreyGames.Leaderboards.Service.Api;
using AndreyGames.Leaderboards.Service.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndreyGames.Leaderboards.Service.Controllers
{
    [Route("leaderboards")]
    [FormatExceptions]
    [Authorize]
    public class LeaderboardController: ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;

        }

        [HttpPost("{game}")]
        [CommitOnOk]
        public async Task<LeaderboardApiResponse> AddLeaderboard([FromRoute] string game)
        {
            if (!await _leaderboardService.Exist(game))
            {
                await _leaderboardService.CreateLeaderboard(game);
            }

            return new LeaderboardApiResponse();
        }

        [HttpGet("{game}")]
        public async Task<LeaderboardApiResponse> GetLeaderboard([FromRoute] string game, 
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var view = await _leaderboardService.GetLeaderboard(game, offset, limit);

            return new LeaderboardApiResponse(view);
        }

        [HttpGet("{game}/{playerName}/score")]
        public async Task<LeaderboardApiResponse> GetPlayerScore([FromRoute] string game, [FromRoute] string playerName)
        {
            return new LeaderboardApiResponse(await _leaderboardService.GetScoreForPlayer(game, playerName));
        }

        [HttpPut("{game}/{playerName}/score/{score:int}")]
        [CommitOnOk]
        public async Task<LeaderboardApiResponse> AddOrUpdateScore([FromRoute] string game, [FromRoute] string playerName, [FromRoute] int score)
        {
            await _leaderboardService.PutPlayerScore(game, playerName, score);
            return new LeaderboardApiResponse();
        }
    }
}