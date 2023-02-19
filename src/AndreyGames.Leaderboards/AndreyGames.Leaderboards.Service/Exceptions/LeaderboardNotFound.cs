namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public sealed class LeaderboardNotFound : BusinessLogicException
    {
        public LeaderboardNotFound(string name) : base($"Leaderboard '{name}' not found", "leaderboard_not_found")
        {
        }
    }
}