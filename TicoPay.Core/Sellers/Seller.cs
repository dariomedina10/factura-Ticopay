using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Sellers
{
    public class Seller : AuditedEntity<int>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string InternalCode { get; set; }

        [Required]
        public SellerType SellerType { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de comision debe estar entre 0 y 100")]
        public decimal SalesPercentage { get; set; }
    }

    public enum SellerType
    {
        // Vendedor
        Seller,
        // Distribuidor
        Distributor,
    }
}
