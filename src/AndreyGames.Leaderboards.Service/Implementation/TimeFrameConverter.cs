using System;
using System.Globalization;
using AndreyGames.Leaderboards.API;
using AndreyGames.Leaderboards.Service.Abstract;

namespace AndreyGames.Leaderboards.Service.Implementation
{
    public class TimeFrameConverter : ITimeFrameConverter
    {
        private readonly ISystemClock _systemClock;

        public TimeFrameConverter(ISystemClock systemClock)
        {
            _systemClock = systemClock;

        }

        public DateTime? GetStartDate(TimeFrame timeFrame)
        {
            var now = _systemClock.UtcNow();

            switch (timeFrame)
            {
                case TimeFrame.Year: return new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).Date;
                case TimeFrame.Month:
                    return new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                case TimeFrame.Week:
                    var culture = CultureInfo.CurrentCulture;
                    var diff = now.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
                    if (diff < 0) diff += 7;
                    return now.AddDays(-diff).Date;
                case TimeFrame.Today: return now.Date;
            }

            return null;
        }

        public DateTime? GetEndDate(TimeFrame timeFrame)
        {
            var now = _systemClock.UtcNow();

            switch (timeFrame)
            {
                case TimeFrame.Year: return new DateTime(now.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc).Date;
                case TimeFrame.Month:
                    return new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).Date;
                case TimeFrame.Week:
                    var culture = CultureInfo.CurrentCulture;
                    var diff = now.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
                    if (diff < 0) diff += 7;
                    return now.AddDays(diff).Date;
                case TimeFrame.Today: return now.AddDays(1).Date;
            }

            return null;
        }
    }
}