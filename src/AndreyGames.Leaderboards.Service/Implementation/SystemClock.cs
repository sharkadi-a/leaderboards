using System;
using AndreyGames.Leaderboards.Service.Abstract;

namespace AndreyGames.Leaderboards.Service.Implementation
{
    public class SystemClock : ISystemClock
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}