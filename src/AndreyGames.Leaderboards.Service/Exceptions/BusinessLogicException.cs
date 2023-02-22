namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public class BusinessLogicException : LeaderboardsServiceException
    {
        public override string Message { get; }
        
        public override int HttpStatusCode => 400;

        public override string ErrorCode => Data["ErrorCode"]?.ToString();

        public BusinessLogicException(string userMessage, string errorCode)
        {
            Message = userMessage;
            Data["ErrorCode"] = errorCode;
        }
    }
}