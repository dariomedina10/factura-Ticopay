using System.Collections.Generic;
using Abp.Application.Services.Dto;
using TicoPay.Common;

namespace TicoPay.Clients.Dto
{
    public class GetClientsOutput : IDtoViewBaseFields
    {
        public List<ClientDto> Clients { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }

}
