namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public sealed class LeaderboardAlreadyExist : BusinessLogicException
    {
        public LeaderboardAlreadyExist(string name) : base($"Leaderboard '{name}' already exist", "leaderboard_already_exist")
        {
        }
    }
}