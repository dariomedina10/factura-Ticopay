using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;
using TicoPay.Clients;
using TicoPay.Core;
using TicoPay.Drawers;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using TicoPay.Taxes;

namespace TicoPay.Printers
{
    public class DocumentPrint
    {
        public string VoucherKey { get; set; }

        public Status Status { get; set; }

        public Tenant Tenant { get; set; }

        public TypeDocumentInvoice TypeDocument { get; set; }

        public Client Client { get; set; }

        public string ConsecutiveNumber { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }

        public decimal DiscountAmount { get; protected set; }

        public decimal TotalTax { get; set; }

        public decimal Total { get; set; }

        public bool IsContingency { get; set; } = false;

        public string ConsecutiveNumberContingency { get; set; }

        public string ReasonContingency { get; set; }

        public DateTime? DateContingency { get; set; }

        public string GeneralObservation { get; set; }

        public decimal TotalServGravados { get; set; }
        public decimal TotalServExento { get; set; }
        public decimal TotalProductExento { get; set; }
        public decimal TotalProductGravado { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalExento { get; set; }
        public decimal SaleTotal { get; set; }
        public decimal NetaSale { get; set; }

        public Document DocumentRef { get; set; }

        public List<DetailsDocument> Details { get; set; }

        public List<PaymentDocument> Payments { get; set; }

        public BranchOffice BranchOffice { get; set; }

        public DocumentPrint(Invoice invoicenew)
        {
            VoucherKey = invoicenew.VoucherKey;
            CodigoMoneda = invoicenew.CodigoMoneda;
            Client = invoicenew.Client;
            ConditionSaleType = invoicenew.ConditionSaleType;
            ConsecutiveNumber = invoicenew.ConsecutiveNumber;
            DueDate = invoicenew.DueDate;
            ExpirationDate = invoicenew.ExpirationDate;
            Tenant = invoicenew.Tenant;
            Total = invoicenew.Total;
            TotalExento = invoicenew.TotalExento;
            TotalGravado = invoicenew.TotalGravado;
            TotalProductExento = invoicenew.TotalProductExento;
            TotalProductGravado = invoicenew.TotalProductGravado;
            TotalServExento = invoicenew.TotalServExento;
            TotalServGravados = invoicenew.TotalServGravados;
            TotalTax = invoicenew.TotalTax;
            DiscountAmount = invoicenew.DiscountAmount;
            TypeDocument = invoicenew.TypeDocument;
            Status = invoicenew.Status;
            GeneralObservation = invoicenew.GeneralObservation;
            IsContingency = invoicenew.IsContingency;
            ConsecutiveNumberContingency = invoicenew.ConsecutiveNumberContingency;
            DateContingency = invoicenew.DateContingency;
            ReasonContingency = invoicenew.ReasonContingency;
            BranchOffice = invoicenew.Drawer.BranchOffice;

            Details = (from c in invoicenew.InvoiceLines
                       select new DetailsDocument
                       {
                           PricePerUnit = c.PricePerUnit,
                           TaxAmount = c.TaxAmount,
                           Total = c.Total,
                           DiscountPercentage = c.DiscountPercentage,
                           Note = c.Note,
                           Title = c.Title,
                           Quantity = c.Quantity,
                           LineType = c.LineType,
                           ServiceId = c.ServiceId,
                           ProductId = c.ProductId,
                           LineNumber = c.LineNumber,
                           CodeTypes = c.CodeTypes,
                           DescriptionDiscount = c.DescriptionDiscount,
                           SubTotal = c.SubTotal,
                           LineTotal = c.LineTotal,
                           Tax = c.Tax,
                           UnitMeasurement = c.UnitMeasurement,
                           UnitMeasurementOthers = c.UnitMeasurementOthers

                       }).ToList();         
            Payments = (from c in invoicenew.InvoicePaymentTypes
                        select new PaymentDocument
                        {
                            Amount = c.Amount,
                            PaymetnMethodType = c.PaymetnMethodType,
                            IsPaymentReversed = c.IsPaymentReversed,
                            Payment = c.Payment,
                            Bank = c.Bank,
                            UserCard = c.UserCard
                        }
                        ).ToList();
            
        }

        public DocumentPrint(Note note)
        {
            if (note.ConsecutiveNumberReference != null)
            {
                var tipo = "Nota de " + (note.NoteType == NoteType.Credito ? NoteType.Debito.GetDescription() : NoteType.Credito.GetDescription());
                DocumentRef = new Document { ConsecutiveNumber= note.ConsecutiveNumberReference , TypeDocument= tipo };            
               
            }
            else
            {                
                DocumentRef = new Document { ConsecutiveNumber = note.Invoice.ConsecutiveNumber, TypeDocument = note.Invoice.TypeDocument.GetDescription() };               
            }
            
            VoucherKey = note.VoucherKey;
            CodigoMoneda = (FacturaElectronicaResumenFacturaCodigoMoneda) note.CodigoMoneda;
            Client = note.Invoice.Client;
            ConditionSaleType = note.ConditionSaleType;
            ConsecutiveNumber = note.ConsecutiveNumber;
            DueDate = note.CreationTime;
            ExpirationDate = note.DueDate;
            Tenant = note.Invoice.Tenant;
            Total = note.Total;
            TotalExento = note.TotalExento;
            TotalGravado = note.TotalGravado;
            TotalProductExento = note.TotalProductExento;
            TotalProductGravado = note.TotalProductGravado;
            TotalServExento = note.TotalServExento;
            TotalServGravados = note.TotalServGravados;
            TotalTax = note.TaxAmount;
            DiscountAmount = note.DiscountAmount;
            TypeDocument = note.NoteType== NoteType.Credito? TypeDocumentInvoice.NotaCredito: TypeDocumentInvoice.NotaDebito;
            Status = note.Status;
            Details = (from c in note.NotesLines
                       select new DetailsDocument
                       {
                           PricePerUnit = c.PricePerUnit,
                           TaxAmount = c.TaxAmount,
                           Total = c.Total,
                           DiscountPercentage = c.DiscountPercentage,
                           Note = c.Notes,
                           Title = c.Title,
                           Quantity = c.Quantity,
                           LineType = c.LineType,
                           ServiceId = c.ServiceId,
                           ProductId = c.ProductId,
                           LineNumber = c.LineNumber,
                           CodeTypes = c.CodeTypes,
                           DescriptionDiscount = c.DescriptionDiscount,
                           SubTotal = c.SubTotal,
                           LineTotal = c.LineTotal,
                           Tax = c.Tax,
                           UnitMeasurement = c.UnitMeasurement,
                           UnitMeasurementOthers = c.UnitMeasurementOthers

                       }).ToList();
            Payments = note.NotePaymentTypes==null?null:(from c in note.NotePaymentTypes
                        select new PaymentDocument
                        {
                            Amount = c.Amount,
                            PaymetnMethodType = c.PaymetnMethodType,
                            IsPaymentReversed = c.IsPaymentReversed,
                            Payment = c.Payment,
                            //Bank = c.Bank,
                            //UserCard = c.UserCard
                        }
                       ).ToList();
            BranchOffice = note.Drawer.BranchOffice;
        }
    }

    public class DetailsDocument
    {
        public decimal PricePerUnit { get;  set; }
        public decimal TaxAmount { get;  set; }
        public decimal Total { get;  set; }
        public decimal DiscountPercentage { get;  set; }
        [MaxLength(200)]
        public string Note { get;  set; }
        [MaxLength(160)]
        public string Title { get;  set; }
        public decimal Quantity { get;  set; }      
        public LineType LineType { get;  set; }
        public Guid? ServiceId { get;  set; }
        public Guid? ProductId { get;  set; }
        public int LineNumber { get;  set; }
        public CodigoTypeTipo CodeTypes { get;  set; }
        [MaxLength(20)]
        public string DescriptionDiscount { get;  set; }
        public decimal SubTotal { get;  set; }
        public decimal LineTotal { get;  set; }   
        public Tax Tax { get; set; }     
        public UnidadMedidaType UnitMeasurement { get; set; }
        public string UnitMeasurementOthers { get; set; }
    }

    public class PaymentDocument
    {
      
        public decimal Amount { get;  set; }

        public PaymetnMethodType PaymetnMethodType { get; set; }  
            
        public bool? IsPaymentReversed { get; set; }              

        public  Payment Payment { get;  set; }

        public  Bank Bank { get; set; }
        
        public string UserCard { get; set; }
    }
}
