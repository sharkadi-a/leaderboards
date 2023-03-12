using AndreyGames.Leaderboards.Service.Api;
using AndreyGames.Leaderboards.Service.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AndreyGames.Leaderboards.Service.Middleware
{
    public class FormatExceptionsAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not LeaderboardsServiceException exception)
            {
                context.Result = new LeaderboardApiResponse(context.Exception.InnerException ?? context.Exception);
            }
            else
            {
                context.Result = new LeaderboardApiResponse(exception);
                context.ExceptionHandled = true;
            }
        }
    }
}