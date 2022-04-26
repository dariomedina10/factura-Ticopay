using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;
using TicoPay.Services;
using TicoPay.Taxes;

namespace TicoPay.Invoices.Dto
{
    public class AddServiceInvoice : IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        public int GridId { get; set; }
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Rate { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal TaxAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Quanty { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Total { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DiscountPercentaje { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal DiscountAmount { get; set; }

        public IList<Service> Services { get; set; }
        public string ServiceId { get; set; }
        public IList<string> ClientServiceList { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
