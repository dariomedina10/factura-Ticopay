using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.BackgroundJobs
{
    public class UploadAndSendInvoiceJob : BackgroundJob<UploadAndSendInvoiceJobArgs>, ITransientDependency
    {
        public readonly IInvoiceAppService _invoiceAppClient;

        public UploadAndSendInvoiceJob(IInvoiceAppService invoiceAppClient)
        {
            _invoiceAppClient = invoiceAppClient;
        }

        [AutomaticRetry(Attempts = 0)]
        public override void Execute(UploadAndSendInvoiceJobArgs args)
        {
            _invoiceAppClient.CreateUploadSendElectronicIncoice(args.idinvoice, args.TenantId);
        }
    }

    [Serializable]
    public class UploadAndSendInvoiceJobArgs
    {
        public Guid idinvoice { get; set; }

        public int TenantId { get; set; }
    }
}
