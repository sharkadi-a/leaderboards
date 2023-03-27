using System;

namespace AndreyGames.Leaderboards.Service.Abstract
{
    public interface ISystemClock
    {
        DateTime UtcNow();
    }
}