namespace TicoPay.Common
{
    public interface IComprobanteRecepcion
    {
        string VoucherKey { get; set; }

        string ElectronicBill { get; set; }
    }
}