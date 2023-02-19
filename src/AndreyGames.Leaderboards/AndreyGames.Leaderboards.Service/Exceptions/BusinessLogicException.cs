namespace AndreyGames.Leaderboards.Service.Exceptions
{
    public class BusinessLogicException : LeaderboardsServiceException
    {
        public override string Message { get; }
        
        public string ErrorCode { get; }

        public override int HttpStatusCode => 400;

        public BusinessLogicException(string userMessage, string errorCode)
        {
            Message = userMessage;
            ErrorCode = errorCode;
            Data["ErrorCode"] = errorCode;
        }
    }
}