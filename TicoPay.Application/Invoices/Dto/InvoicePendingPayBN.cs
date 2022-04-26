using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.Dto
{
     [AutoMapFrom(typeof(Invoice))]
    public class InvoicePendingPayBN : EntityDto<Guid>
    {
        //public Guid Id { get; set; }

        public decimal Balance { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PaymentDate { get; set; }
        public Guid? PaymentId { get; set; }

        public string Number { get; set; }

        public Status Status { get; set; }

        public Guid? ClientId { get; set; }
        public long? Code { get; set; }
        public int CreditTerm { get; internal set; }
    }
}
