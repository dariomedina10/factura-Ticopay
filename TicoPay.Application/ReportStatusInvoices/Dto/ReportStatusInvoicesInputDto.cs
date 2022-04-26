using System;
using System.Collections.Generic;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TicoPay.Invoices.XSD;
using Newtonsoft.Json;
using TicoPay.BranchOffices;

namespace TicoPay.ReportStatusInvoices.Dto
{
    public class ReportStatusInvoicesInputDto<T> : IDtoViewBaseFields
    {
        public Guid? Id { get; set; }

        public IList<T> InvoicesList { get; set; }

        //tipo de moneda actual
        public TypeDocument? Type { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime? InitialDate { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateTime? FinalDate { get; set; }

        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration? Status { get; set; }

        [Display(Name = "Acuse Recibido")]
        public StatusReception? StatusReception { get; set; }

        [Display(Name = "Estado")]
        public Status? StatusPay { get; set; }

        [Display(Name = "Código Cliente")]
        public Guid? ClientId { get; set; }

        public Guid? GroupsId { get; set; }

        [Display(Name = "Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "No. Documento")]
        public string NumberInvoice { get; set; }

        public string ConsecutiveNumberReference { get; set; }

        [Display(Name = "Moneda")]
        public NoteCodigoMoneda? CodigoMoneda { get; set; }


        [Display(Name = "Sucursal")]
        public Guid? BranchOfficeId { get; set; }

        [Display(Name = "Caja")]
        public Guid? DrawerId { get; set; }

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffice { get; set; }

        //SelectListItem
        public IList<SelectListItem> DrawerUser { get; set; }

        public ICollection<Group> Groups { get; set; }

        public IEnumerable<SelectListItem> listStatusRecepcion { get; set; }

        public IEnumerable<SelectListItem> listStatusTaxAdmin { get; set; }

        public int MinimumAmount { get; set; }
        public int MaxAmount { get; set; }

        //public TypeDocumentInvoice? TypeDocument { get; set; }

        public IList<SelectListItem> ListTypeDocument
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TypeDocumentInvoice.Invoice, TypeDocumentInvoice.Ticket);
            }
        }

        //public IEnumerable<SelectListItem> listStatus { get; set; }

        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> listStatus
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                foreach (Status status in Enum.GetValues(typeof(Status)))
                {
                    list.Add(new System.Web.Mvc.SelectListItem { Value = status.ToString(), Text = Application.Helpers.EnumHelper.GetDescription(status) });
                }
                return list;
            }

        }

        public TypeDocumentInvoice? TypeDocumentinvoice { get; set; }

        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> Types
        {
            get
            {
                //ojo con esta lista
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = TypeDocument.NoteCredito.ToString(), Text = "Nota de Crédito" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = TypeDocument.NoteDebito.ToString(), Text = "Nota de Débito" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = TypeDocument.Invoice.ToString(), Text = "Factura" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = TypeDocument.Ticket.ToString(), Text = "Tiquete Electrónico" });
                return list;
            }

        }

        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> CodigoMonedas
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = NoteCodigoMoneda.CRC.ToString(), Text = "CRC" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = NoteCodigoMoneda.USD.ToString(), Text = "USD" });
                return list;
            }

        }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
