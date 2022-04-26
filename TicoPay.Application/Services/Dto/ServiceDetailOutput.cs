using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using TicoPay.Common;
using TicoPay.Taxes;
using TicoPay.Users;

namespace TicoPay.Services.Dto
{
    [AutoMapFrom(typeof(Service))]
    public class ServiceDetailOutput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(Service.MaxNameLength)]
        public string Name { get; set; }

        [Display(Name = "Precio")]
        public decimal Price { get; set; }

        public Guid? TaxId { get; set; }
        [Display(Name = "Impuesto")]
        public Tax Tax { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public int LastModifierUserId { get; set; }

        public int CreatorUserId { get; set; }

        public IList<User> Users { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
