namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public sealed class LeaderboardAlreadyExist : BusinessLogicException
    {
        public override int HttpStatusCode => 409;

        public LeaderboardAlreadyExist(string name) : base($"Leaderboard '{name}' already exist", "leaderboard_already_exist")
        {
            Data["Leaderboard"] = name;
        }
    }
}