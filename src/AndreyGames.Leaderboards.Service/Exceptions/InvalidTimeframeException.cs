namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public class InvalidTimeframeException : BusinessLogicException
    {
        public InvalidTimeframeException() : base("Start date should not be after end date", "invalid_time_frame")
        {
        }
    }
}