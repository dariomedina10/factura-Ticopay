using System;

namespace TicoPay.Common
{
    /// <summary>
    /// Class to wrap the .NET DateTime
    /// </summary>
    public class DateTimeAdapter : IDateTime
    {
        /// <summary>
        /// Gets the current UTC Time
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.UtcNow; }
        }
    }
}
