using TicoPay.MultiTenancy;

namespace TicoPay.Invoices.Dto
{
    public interface IUnityInvoice
    {
        Tenant.FirmType? TipoFirma { get; set; }
    }
}