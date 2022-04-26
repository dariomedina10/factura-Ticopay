using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;

namespace TicoPay.Clients.Dto
{
    public class CantonDTO : IDtoViewBaseFields
    {
        public int Id { get; set; }

        public string NameCanton { get; set; }

        public int ProvinciaId { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
