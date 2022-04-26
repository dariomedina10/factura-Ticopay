using Abp.Application.Services;

namespace WorkerInvoiceCreator
{
    public interface IWorkerProcess : IApplicationService
    {
        void Process();
        void Dispose();
    }
}
