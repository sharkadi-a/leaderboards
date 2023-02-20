using System.Threading.Tasks;
using AndreyGames.Leaderboards.Service.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AndreyGames.Leaderboards.Service.Api
{
    public sealed class LeaderboardApiResponse : ActionResult
    {
        private readonly IActionResult _result;

        public LeaderboardApiResponse(object result)
        {
            _result = new OkObjectResult(result);
        }

        public LeaderboardApiResponse()
        {
            _result = new OkResult();
        }

        public LeaderboardApiResponse(LeaderboardsServiceException exception)
        {
            _result = new ObjectResult(new
            {
                exception.Message,
                exception.Data
            })
            {
                StatusCode = exception.HttpStatusCode
            };
        }
        
        public async override Task ExecuteResultAsync(ActionContext context)
        {
            await _result.ExecuteResultAsync(context);
        }
    }
}