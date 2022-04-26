using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.ReportClosing.Dto
{
    public class ResultTotalDto
    {
        public string Month { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
     
    }
}
