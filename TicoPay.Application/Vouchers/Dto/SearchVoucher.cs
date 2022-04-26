using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.BranchOffices;
using TicoPay.Drawers;
using TicoPay.Invoices;

namespace TicoPay.Vouchers.Dto
{
    public class SearchVoucher
    {
        
        public const int MaxNameLength = 80;
        public const int MaxIdentificationLength = 12;
        public const int MaxIdentificationExtranjeroLength = 20;
        public const int MaxConsecutiveNumberLength = 20;
        public const int MaxKeyLength = 20;

        public Guid? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; } = 100;

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; } = 10;

        public int? Page { get; set; }

        [StringLength(MaxIdentificationLength)]
        [Display(Name = "Nº Idendificación Proveedor")]
        public string Identification { get; set; }

        [MaxLength(MaxConsecutiveNumberLength)]
        [Display(Name = "Nº Factura")]
        public string ConsecutiveNumberInvoice { get; set; }

        [MaxLength(MaxConsecutiveNumberLength)]
        [Display(Name = "Nº Comprobante")]
        public string ConsecutiveNumber { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime? StartDueDate { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateTime? EndDueDate { get; set; }

        [StringLength(MaxNameLength)]
        [Display(Name = "Proveedor")]
        public string Name { get; set; }

        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration? StatusTribunet { get; set; }

        [Display(Name = "Estatus Envío")]
        public StatusVoucher? StatusSendVoucher { get; set; }

        [Display(Name = "Estado Recepción")]
        public MessageVoucher? Status { get; set; }

        public bool isKey { get; set; }

        [Display(Name = "Sucursal")]
        public Guid? BranchOfficeId { get; set; }

        [Display(Name = "Caja")]
        public Guid? DrawerId { get; set; }

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffice { get; set; }

        public int? MinimumAmount { get; set; }
        public int? MaxAmount { get; set; }

        //SelectListItem
        public IList<SelectListItem> DrawerUser { get; set; }

        public string MessageTaxAdministration { get; set; }

        public MessageSupplier? MessageSupplier { get; set; }
        public string MessageTaxAdministrationSupplier { get; set; }

        public IList<SelectListItem> ListStatusSendVoucher
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(typeof(StatusVoucher));
            }
        }

        public IList<SelectListItem> ListStatusTribunet
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(typeof(StatusTaxAdministration));
            }
        }

        public IList<SelectListItem> ListStatus
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(typeof(MessageVoucher));
            }
        }

        public List<VoucherDto> Entities { get; set; }

        public Drawer Drawer { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
    }

    public enum StatusVoucher
    {
       
        //[Description("Todos")]
        //Todos = -1,       
        [Description("No Enviados")]
        NoEnviado=0,       
        [Description("Enviados")]
        Enviados=1,
    }

}
