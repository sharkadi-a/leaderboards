namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public sealed class LeaderboardNotFound : BusinessLogicException
    {
        public override int HttpStatusCode => 404;

        public LeaderboardNotFound(string name) : base($"Leaderboard '{name}' not found", "leaderboard_not_found")
        {
            Data["Leaderboard"] = name;
        }
    }
}