using System;

namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public abstract class LeaderboardsServiceException : Exception
    {
        public abstract int HttpStatusCode { get; }
        
        public abstract string ErrorCode { get; }

        protected LeaderboardsServiceException()
        {
            Data["Timestamp"] = DateTime.UtcNow.ToString("O");
        }
    }
}