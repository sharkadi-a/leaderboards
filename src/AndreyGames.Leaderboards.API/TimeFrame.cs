namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// A time window to collect data within
    /// </summary>
    public enum TimeFrame
    {
        /// <summary>
        /// Infinite time window (data for all time)
        /// </summary>
        Infinite = 0,
        
        /// <summary>
        /// This year
        /// </summary>
        Year = 1,
        
        /// <summary>
        /// This week
        /// </summary>
        Week = 2,
        
        /// <summary>
        /// Today
        /// </summary>
        Today = 3,
    }
}