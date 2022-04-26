using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    public interface IDtoViewBaseFields
    {
        int? ErrorCode { get; set; }

        string ErrorDescription { get; set; }

        string Action { get; set; }


        string Control { get; set; }

        string Query { get; set; }
    }
}
