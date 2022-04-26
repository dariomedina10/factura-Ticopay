using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Taxes;

namespace TicoPay.General
{
    public class Tipos : Entity<int>
    {
         public const int MaxNameLength = 150;
         public const int MaxcodigoLength = 2;

        [Required]
        public int GrupoTipo { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string name { get; set; }

        [Required]
        [StringLength(MaxcodigoLength)]
        public string codigo { get; set; }

       
        //public virtual ICollection<Invoice> Invoice { get; protected set; }

    }
}
