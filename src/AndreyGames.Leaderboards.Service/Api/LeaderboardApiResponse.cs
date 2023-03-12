using System;
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

        public LeaderboardApiResponse(Exception exception)
        {
            if (!exception.Data.Contains("Timestamp"))
            {
                exception.Data["Timestamp"] = DateTime.Now.ToString("O");
            }
            
            _result = new ObjectResult(new
            {
                exception.Message,
                exception.Data
            })
            {
                StatusCode = exception is LeaderboardsServiceException e ? e.HttpStatusCode : 500,
            };
        }
        
        public async override Task ExecuteResultAsync(ActionContext context)
        {
            await _result.ExecuteResultAsync(context);
        }
    }
}