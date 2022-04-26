using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace TicoPay.Invoices
{
    public class ExchangeRate : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>
        /// Currency code from which the exchange rate was converted.
        /// </summary>
        [Required, MaxLength(3)]
        public string FromCurrencyCode { get; protected set; }

        /// <summary>
        /// Currency code to which the exchange rate was converted.
        /// </summary>
        [Required, MaxLength(3)]
        public string ToCurrencyCode { get; protected set; }

        /// <summary>
        /// Average exchange rate for the day.
        /// </summary>
        public decimal AverageRate { get; protected set; }

        /// <summary>
        /// Final exchange rate for the day.
        /// </summary>
        public decimal EndOfDayRate { get; protected set; }

        /// <summary>
        /// Date and time the exchange rate was obtained.
        /// </summary>
        public DateTime CurrencyRateDate { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// We don't make constructor public
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        protected ExchangeRate()
        {

        }
    }
}
