using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using TicoPay.Services;

namespace TicoPay.ReportTaxAdministration.Dto
{
    public class ViewInvoiceDto
    {
        public Guid Id { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Register Register { get; set; }
        public virtual Client Client { get; set; }
        public virtual ClientService ClientService { get; set; }

        public int TenantId { get; set; }
        public Guid? RegisterId { get; set; }
        public Guid? ClientServiceId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? UserId { get; set; }

        public int Number { get; set; }
        public string Alphanumeric { get; set; }
        public string Note { get; set; }
        public string ConsecutiveNumber { get; set; }
        public byte[] QRCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountPercentaje { get; set; }
        //public DateTime? PaymentDate { get; set; }
        public string Transaction { get; set; }
        public string TenantName { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; }
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }
        public string OtherConditionSale { get; set; }
        public int CreditTerm { get; set; }
        public VoucherSituation StatusVoucher { get; set; }
        public string MessageTaxAdministration { get; set; }
        public string MessageReceiver { get; set; }
        public string ElectronicBill { get; set; }
        public string ElectronicBillPDF { get; set; }
        public string VoucherKey { get; set; }
        public decimal ChangeType { get; set; }
        public decimal TotalServGravados { get; set; }
        public decimal TotalServExento { get; set; }
        public decimal TotalProductExento { get; set; }
        public decimal TotalProductGravado { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalExento { get; set; }
        public decimal SaleTotal { get; set; }
        public decimal NetaSale { get; set; }
        public bool SendInvoice { get; set; }
        public StatusTaxAdministration StatusTribunet { get; set; }
        public bool IsInvoiceReceptionConfirmed { get; set; }

        public ICollection<InvoiceLine> InvoiceLines { get; set; }
        public ICollection<PaymentInvoice> PaymentInvoices { get; set; }
        public ICollection<ListItems> ListItems { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ICollection<InvoiceHistoryStatus> InvoiceHistoryStatuses { get; set; }
        public ICollection<PaymentInvoice> InvoicePaymentTypes { get; set; }

        public string CedulaEmisor
        {
            get
            {
                if (Tenant == null)
                    return string.Empty;

                return Tenant.IdentificationNumber;
            }
        }
        public string CedulaReceptor
        {
            get
            {
                if (Client == null)
                    return string.Empty;

                return Client.Identification;
            }
        }
        public string NombreReceptor
        {
            get
            {
                if (Client == null)
                    return string.Empty;

                return Client.Name + " " + Client.LastName;
            }
        }

        public IEnumerable<Note> NotasCredito
        {
            get
            {
                if (Notes == null)
                    return null;

                return Notes.Where(n => n.NoteType == NoteType.Credito);
            }
        }
        public IEnumerable<Note> NotasDebito
        {
            get
            {
                if (Notes == null)
                    return null;

                return Notes.Where(n => n.NoteType == NoteType.Debito);
            }
        }

        public DateTime? PaymentDate
        {
            get
            {
                var firtsPaymentType = InvoicePaymentTypes.FirstOrDefault();
                if (firtsPaymentType != null)
                {
                    return firtsPaymentType.Payment.PaymentDate;
                }
                return null;
            }
        }

        public PaymetnMethodType? PaymetnMethodType
        {
            get
            {
                var firtsPaymentType = InvoicePaymentTypes.FirstOrDefault();
                if (firtsPaymentType != null)
                {
                    return firtsPaymentType.Payment.PaymetnMethodType;
                }
                return null;
            }
        }

        public decimal TotalInvoiceLines
        {
            get
            {
                if (InvoiceLines == null)
                    return 0;

                return InvoiceLines.Where(a => a.InvoiceId == Id).Sum(a => a.LineTotal);
            }
        }

        public decimal TotalNotasDebito
        {
            get
            {
                if (Notes == null)
                    return 0;

                return Notes.Where(a => a.InvoiceId == Id && a.NoteType == NoteType.Debito).Sum(a => a.Total);
            }
        }

        public decimal TotalNotasCredito
        {
            get
            {
                if (Notes == null)
                    return 0;

                return Notes.Where(a => a.InvoiceId == Id && a.NoteType == NoteType.Credito).Sum(a => a.Total);
            }
        }

        public decimal TotalTaxes
        {
            get
            {
                if (InvoiceLines == null)
                    return 0;

                return InvoiceLines.Where(a => a.InvoiceId == Id).Sum(a => a.TaxAmount);
            }
        }

        public ViewInvoiceDto(Invoice invoice)
        {
            Id = invoice.Id;
            Tenant = invoice.Tenant;
            Register = invoice.Register;
            Client = invoice.Client;
            TenantId = invoice.TenantId;
            RegisterId = invoice.RegisterId;
            ClientId = invoice.ClientId;
            UserId = invoice.UserId;
            Number = invoice.Number;
            Alphanumeric = invoice.Alphanumeric;
            Note = invoice.Note;
            ConsecutiveNumber = invoice.ConsecutiveNumber;
            QRCode = invoice.QRCode;
            SubTotal = invoice.SubTotal;
            DiscountPercentaje = invoice.DiscountPercentaje;
            DiscountAmount = invoice.DiscountAmount;
            TotalTax = invoice.TotalTax;
            Total = invoice.Total;
            Balance = invoice.Balance;
            DueDate = invoice.DueDate;
            Status = invoice.Status;
            CodigoMoneda = invoice.CodigoMoneda;
            UserName = invoice.UserName;
            IsDeleted = invoice.IsDeleted;
            DeleterUserId = invoice.DeleterUserId;
            DeletionTime = invoice.DeletionTime;
            ConditionSaleType = invoice.ConditionSaleType;
            OtherConditionSale = invoice.OtherConditionSale;
            CreditTerm = invoice.CreditTerm;
            StatusVoucher = invoice.StatusVoucher;
            MessageTaxAdministration = invoice.MessageTaxAdministration;
            MessageReceiver = invoice.MessageReceiver;
            ElectronicBill = invoice.ElectronicBill;
            ElectronicBillPDF = invoice.ElectronicBillPDF;
            VoucherKey = invoice.VoucherKey;
            ChangeType = invoice.ChangeType;
            TotalServGravados = invoice.TotalServGravados;
            TotalServExento = invoice.TotalServExento;
            TotalProductExento = invoice.TotalProductExento;
            TotalProductGravado = invoice.TotalProductGravado;
            TotalGravado = invoice.TotalGravado;
            TotalExento = invoice.TotalExento;
            SaleTotal = invoice.SaleTotal;
            NetaSale = invoice.NetaSale;
            SendInvoice = invoice.SendInvoice;
            StatusTribunet = invoice.StatusTribunet;
            IsInvoiceReceptionConfirmed = invoice.IsInvoiceReceptionConfirmed;
            InvoiceLines = invoice.InvoiceLines;
            Notes = invoice.Notes;
            InvoiceHistoryStatuses = invoice.InvoiceHistoryStatuses;
            InvoicePaymentTypes = invoice.InvoicePaymentTypes;
        }
    }
}
