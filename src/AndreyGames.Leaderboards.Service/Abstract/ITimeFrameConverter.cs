using System;
using AndreyGames.Leaderboards.API;

namespace AndreyGames.Leaderboards.Service.Abstract
{
    /// <summary>
    /// Converts time frame to DateTime (based on the current UTC date and time)
    /// </summary>
    public interface ITimeFrameConverter
    {
        /// <summary>
        /// Return start date for the time frame (should be treated as inclusive, i.e 'greater or equal to')
        /// </summary>
        DateTime? GetStartDate(TimeFrame timeFrame);
        
        /// <summary>
        /// Return start date for the time frame (should be treated as exclusive, i.e. 'lesser than')
        /// </summary>
        DateTime? GetEndDate(TimeFrame timeFrame);
    }
}