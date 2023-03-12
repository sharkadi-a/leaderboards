using System;
using System.Collections;
using System.Globalization;

namespace AndreyGames.Leaderboards.API
{
    /// <summary>
    /// Exception raised on the server side.
    /// </summary>
    public sealed class ApiException : Exception
    {
        /// <summary>
        /// Message
        /// </summary>
        public override string Message { get; }

        /// <summary>
        /// Additional data
        /// </summary>
        public override IDictionary Data { get; }

        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode => Data.Contains(nameof(ErrorCode)) ? Data[nameof(ErrorCode)].ToString() : default;

        /// <summary>
        /// Timestamp (when error happened)
        /// </summary>
        public DateTime? Timestamp => Data.Contains(nameof(Timestamp))
            ? DateTime.ParseExact(Data[nameof(Timestamp)].ToString(), "O", CultureInfo.InvariantCulture)
            : null;

        public ApiException(string message, IDictionary data)
        {
            Message = message;
            Data = data;
        }
    }
}