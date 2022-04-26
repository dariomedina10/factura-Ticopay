using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;

namespace TicoPay
{
    public class invoiceService : ITransientDependency
    {
        private readonly IInvoiceAppService _invoiceservice;
        public invoiceService(IInvoiceAppService invoiceservice)
        {
            _invoiceservice = invoiceservice;
        }


        public List<InvoicePendingPayBN> GetInvoicesPendingPay(ClientBN client)
        {
            return _invoiceservice.GetInvoicesPendingPay(client);
        }

        public InvoicePendingPayBN GetInvoicesByNumber(ClientBN client, string invoicenumber)
        {
            return _invoiceservice.GetInvoicesByNumber(client, invoicenumber);
        }

        public void ReverseInvoice(InvoicePendingPayBN factura, string reference)
        {
            _invoiceservice.ReverseInvoice(factura, reference);
        }

        public IList<InvoicePendingPayBN> GetInvoicesPendingPayPAR(int index, List<int> listTenant, int PageSize, out bool indicador)
        {
            return _invoiceservice.GetInvoicesPendingPayPAR(index, listTenant, PageSize, out indicador);
        }

        public void ApplyNCRGeneral(Guid Clientid)
        {
            try
            {
                var invoices = _invoiceservice.ObtenerFacturaNCR(Clientid);

                foreach (var _invoice in invoices)
                {
                    _invoiceservice.CreateNoteCorreccion(_invoice.Id, Convert.ToDouble(_invoice.NetaSale),0,1, _invoice.TotalTax,_invoice.Total, _invoice.TenantId) ;
                }
            }
            catch (Exception e)
            {
              
            }
        }

        public List<InvoicePendingPayBN> GetInvoicesPendingOld(ClientBN client, DateTime invoicedate)
        {
            return _invoiceservice.GetInvoicesPendingOld(client, invoicedate);
        }
        public int PayInvoiceBn(InvoicePendingPayBN factura, int codigoAgencia, string trama)
        {
            return _invoiceservice.PayInvoiceBn(factura, codigoAgencia, trama);
        }
    }
}
