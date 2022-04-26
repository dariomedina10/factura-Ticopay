using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Sellers.Dto
{
    [AutoMapFrom(typeof(Seller))]
    public class SellerDto : EntityDto<int>
    {
        public int id { get; set; }

        public string Name { get; set; }

        public string InternalCode { get; set; }

        public SellerType SellerType { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de comision debe estar entre 0 y 100")]
        public decimal SalesPercentage { get; set; }
    }

    
}
