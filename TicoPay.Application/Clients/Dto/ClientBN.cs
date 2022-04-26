using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Clients.Dto
{
     [AutoMapFrom(typeof(Client))]
    public class ClientBN : EntityDto<Guid>
    {
         public string Name { get; set; }
         public long Code { get; set; }
         //public Guid Id { get; set; }
         public int TenantId { get; set; }
         public bool? PagoAutomaticoBn { get; set; }
         public int? DiaPagoBn { get; set; }
         public int? MontoMaximoBn { get; set; }
         public FormaPago? FormaPagoBn { get; set; }
        public string LastName { get; internal set; }
    }
}
