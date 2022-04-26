using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Common;

namespace TicoPay.ReportClients.Dto
{
    public class ReportClientsInputDto : IDtoViewBaseFields
    {

        public IList<Client> ClientsList { get; set; }

        public Guid? GroupId { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
