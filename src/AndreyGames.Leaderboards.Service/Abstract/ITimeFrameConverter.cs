using System;
using AndreyGames.Leaderboards.API;

namespace AndreyGames.Leaderboards.Service.Abstract
{
    public interface ITimeFrameConverter
    {
        DateTime? GetStartDate(TimeFrame timeFrame);
        DateTime? GetEndDate(TimeFrame timeFrame);
    }
}