using AndreyGames.Leaderboards.Service.Abstract;
using AndreyGames.Leaderboards.Service.Implementation;
using AndreyGames.Leaderboards.Service.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace AndreyGames.Leaderboards.Service
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<LeaderboardContext>(ctx => ctx
                .UseLazyLoadingProxies()
                .UseNpgsql(_configuration.GetConnectionString("Default")));

            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "andrey.games Leaderboards API",
                Description = "Leaderboards API for andrey.games web services",
            }));

            services.AddScoped<AuthenticationEvents>();
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(
                    options =>
                    {
                        options.Realm = "andrey.games Leaderboards API";
                        options.EventsType = typeof(AuthenticationEvents);
                        options.AjaxRequestOptions.SuppressWwwAuthenticateHeader = true;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            MigrateDatabase(app);
        }

        public void MigrateDatabase(IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LeaderboardContext>();
                db.Database.Migrate();
            }
        }
    }
}