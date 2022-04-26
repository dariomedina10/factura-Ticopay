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
    public class InvoicePendingPayPAR : EntityDto<Guid>
    {
        //public Guid Id { get; set; }
        public decimal Balance { get; set; }

        public DateTime DueDate { get; set; }

        public int Number { get; set; }

        public Guid ClientId { get; set; }
    }
}
