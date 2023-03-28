using System;
using AndreyGames.Leaderboards.Service.Abstract;

namespace AndreyGames.Leaderboards.Tests
{
    public class TestSystemClock : ISystemClock
    {
        public DateTime CurrentTime { get; set; } = DateTime.UtcNow;

        public DateTime UtcNow() => CurrentTime;
    }
}