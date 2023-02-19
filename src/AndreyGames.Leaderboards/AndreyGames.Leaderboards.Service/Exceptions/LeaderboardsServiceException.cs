using System;

namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public abstract class LeaderboardsServiceException : Exception
    {
        public abstract int HttpStatusCode { get; }
    }
}