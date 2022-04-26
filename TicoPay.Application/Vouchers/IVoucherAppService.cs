using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TicoPay.MultiTenancy;
using TicoPay.Vouchers.Dto;

namespace TicoPay.Vouchers
{
    public interface IVoucherAppService
    {
        List<VoucherDto> SearchVouchers(SearchVoucher searchInput);

       
        VoucherDto downloadXML(HttpPostedFileBase file);
        Uri SaveAzureStorage(string documentname, string ruta, string _container);
        Uri SaveAzureStorageFromText(string documentname, string ruta, Stream content, string _container);

        void Createvoucher(VoucherDto input);
        void SyncsInvoicesWithTaxAdministration(Tenant tenant);

        void VouchersWithTaxAdministration(VoucherDto voucher);

        void ResendFailedVouchers(Tenant tenant);

        bool isDigitalPendingVoucher(int tenatId);
        bool isExistsVoucher(string keyref, string IdentificationSender, int tenatId);
        //Voucher BeginValidateWithHacienda(Tenant tenant, Certificate certified, Voucher voucher);
        //void SetVoucherWithInternet(ref Voucher voucher);
        //void GenerateAndUploadXML(Tenant tenant, Certificate certified, Voucher voucher, out Uri XML);
        //void SetFirmTypeRecurrente(VoucherDto input, Tenant tenant, Voucher voucher);
        //Certificate BeginCreateAndUploadXMl(Tenant tenant, Certificate certified, Voucher voucher);

        //void BeginSendMailAndTicoTalk(Tenant tenant, Voucher voucher, string voucherk);

        string CleanInvalidCharacterFromExternalProvider(string xml);
    }
}
