using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace AndreyGames.Leaderboards.Service
{
    public class AuthenticationEvents : BasicAuthenticationEvents
    {
        private readonly IConfiguration _configuration;

        public AuthenticationEvents(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public override Task ValidatePrincipalAsync(ValidatePrincipalContext context)
        {
            if (_configuration.GetSection("Auth")
                .GetChildren()
                .Any(authSection => context.UserName == authSection["UserName"] 
                                    && context.Password == authSection["Password"]))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                context.Principal = principal;
            }

            return Task.CompletedTask;
        }
    }
}