using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;

namespace TicoPay.General
{
    public class Moneda : Entity<int>
    {
         public const int MaxNameLength = 70;
         public const int MaxcodigoLength = 3;

        [Required]
        [StringLength(MaxNameLength)]
        public string NombrePais { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string NombreMoneda { get; set; }

        [Required]
        [StringLength(MaxcodigoLength)]
        public string codigoMoneda { get; set; }

        //public virtual ICollection<Invoice> Invoices { get; protected set; }
        //public virtual ICollection<PaymentInvoice> PaymentsInvoices { get; protected set; }

    }
}
