using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using AndreyGames.Leaderboards.API;
using AndreyGames.Leaderboards.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndreyGames.Leaderboards.Service.Middleware
{
    public class RequestCryptoProcessorAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var onlyCryptoRequests = context.ActionArguments
                .Where(x => x.Value is LeaderboardsCryptoRequest)
                .ToArray();
            
            foreach (var argument in onlyCryptoRequests)
            {
                if (!context.HttpContext.Request.Headers.TryGetValue("User", out var user))
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

                var password = configuration.GetSection("Auth")
                    .GetChildren()
                    .Where(authSection => user == authSection["UserName"])
                    .Select(authSection => authSection["Password"])
                    .FirstOrDefault();

                if (password is null)
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                var iv = configuration["CryptoVectorString"];
                var cryptoRequest = (LeaderboardsCryptoRequest)argument.Value;
                var cryptoService = context.HttpContext.RequestServices.GetRequiredService<ICryptoService>();

                try
                {
                    var jsonBytes = cryptoService.DecryptFromBase64(cryptoRequest.Body, password, iv);
                    var json = JsonSerializer.Deserialize(jsonBytes, argument.Value.GetType());
                    context.ActionArguments[argument.Key] = json;
                }
                catch (CryptographicException)
                {
                    context.Result = new StatusCodeResult(401);
                }
            }
        }
    }
}