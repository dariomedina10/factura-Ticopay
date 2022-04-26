using System;
using Abp.Application.Services;

namespace TicoPay.Common
{
    /// <summary>
    /// Interfaz to wrap the .NET DateTime
    /// </summary>
    public interface IDateTime : IApplicationService
    {
        /// <summary>
        /// Gets the current UTC Time
        /// </summary>
        DateTime Now { get; }
    }
}
