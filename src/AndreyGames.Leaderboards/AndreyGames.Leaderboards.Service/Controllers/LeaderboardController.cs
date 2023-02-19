using System.Threading.Tasks;
using AndreyGames.Leaderboards.Service.Abstract;
using AndreyGames.Leaderboards.Service.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AndreyGames.Leaderboards.Service.Controllers
{
    [Route("leaderboards")]
    public class LeaderboardController: ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;

        }

        [HttpPost("{game}")]
        public async Task<ActionResult> AddLeaderboard([FromRoute] string game)
        {
            if (!await _leaderboardService.Exist(game))
            {
                await _leaderboardService.CreateLeaderboard(game);
            }

            return Ok();
        }

        [HttpGet("{game}")]
        public async Task<ActionResult<LeaderboardView>> GetLeaderboard([FromRoute] string game, 
            [FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            var view = await _leaderboardService.GetLeaderboard(game, offset, limit);

            return Ok(view);
        }

        [HttpPut("{game}/{playerName}/{score:int}")]
        public async Task<ActionResult> AddOrUpdateScore([FromRoute] string game, [FromRoute] string playerName, [FromRoute] int score)
        {
            await _leaderboardService.AddOrUpdateScore(game, playerName, score);
            return Ok();
        }
    }
}