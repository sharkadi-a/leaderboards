using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace AndreyGames.Leaderboards.Service.Middleware
{
    public class CommitOnOkAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var appContext = context.HttpContext.RequestServices.GetService<LeaderboardContext>();
            if (appContext != null && (context.Exception == null || context.ExceptionHandled))
            {
                appContext.SaveChanges();
            }
        }
    }
}