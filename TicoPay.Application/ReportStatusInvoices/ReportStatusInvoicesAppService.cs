using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.ReportStatusInvoices.Dto;
using System.Data.Entity;
using Abp.Application.Services;
using System.Linq.Expressions;
using LinqKit;
using TicoPay.Application.Helpers;
using TicoPay.Invoices.XSD;
using TicoPay.Taxes;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using TicoPay.Users;
using TicoPay.Common;
using BCR;

namespace TicoPay.ReportStatusInvoices
{
    public class ReportStatusInvoicesAppService : ApplicationService, IReportStatusInvoicesAppService
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;

        private readonly IRepository<Note, Guid> _noteRepository;

        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<Tax, Guid> _taxRepository;
        private readonly IRepository<ClientGroup, Guid> _clientRepository;
        private readonly IUserAppService _userAppService;
        RateOfDay _rateOfDay = new RateOfDay();

        public ReportStatusInvoicesAppService(IRepository<Invoice, Guid> invoiceRepository, IInvoiceManager invoiceManager, IRepository<Note, Guid> noteRepository, 
            IRepository<Tax, Guid> taxRepository, IRepository<ClientGroup, Guid> clientRepository, IUserAppService userAppService)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _noteRepository = noteRepository;
            _taxRepository = taxRepository;
            _clientRepository = clientRepository;
            _userAppService = userAppService;
        }


        public IList<ReportStatusInvoicesDto> SearchReportStatusInvoices(ReportStatusInvoicesInputDto<ReportStatusInvoicesDto> searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();

            if (drawers != null)
            {
                var predicate = PredicateBuilder.New<Invoice>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));


                if (searchInput.Status != null)
                    predicate = predicate.And(a => a.StatusTribunet == searchInput.Status.Value);

                if (searchInput.Id!=null)
                    predicate = predicate.And(a => a.Id == searchInput.Id);

                if (searchInput.StatusReception != null)
                {
                    bool isReception = searchInput.StatusReception == 0 ? false : true;
                    predicate = predicate.And(a => a.IsInvoiceReceptionConfirmed == isReception);
                }
                if (searchInput.NumberInvoice != null)
                {
                    var numero = searchInput.NumberInvoice.Length < 10 ? searchInput.NumberInvoice : searchInput.NumberInvoice.Substring(10);
                    var number = numero.ToString().PadLeft(10, '0');
                    predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);
                }
                if (searchInput.TypeDocumentinvoice != null)
                    predicate = predicate.And(a => a.TypeDocument == searchInput.TypeDocumentinvoice);

                if (searchInput.ClientId != null)
                    predicate = predicate.And(a => a.ClientId == searchInput.ClientId);

                if (searchInput.GroupsId != null)
                    predicate = predicate.And(a => a.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));


                if (searchInput.InitialDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.DueDate) >= searchInput.InitialDate);

                if (searchInput.FinalDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.DueDate) <= searchInput.FinalDate);

                if (searchInput.CodigoMoneda != null)
                {
                    string value = searchInput.CodigoMoneda.ToString();

                    if (Enum.TryParse(value, out FacturaElectronicaResumenFacturaCodigoMoneda facturaMoneda))
                    {
                        predicate = predicate.And(a => a.CodigoMoneda == facturaMoneda);
                    }
                }

            if (searchInput.TypeDocumentinvoice != null)
                predicate = predicate.And(a => a.TypeDocument == searchInput.TypeDocumentinvoice.Value);

                if (searchInput.BranchOfficeId != null && searchInput.DrawerId == null)
                {
                    predicate = predicate.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId.Value);
                }

                if (searchInput.DrawerId != null)
                {
                    predicate = predicate.And(a => a.DrawerId == searchInput.DrawerId);
                }

                //predicate = predicate.And(a => a.Client != null);



                var invoiceList = _invoiceRepository.GetAll().Include(a => a.Notes).Include(a => a.Client).Where(predicate);

                var query = (from invoice in invoiceList
                         orderby invoice.ConsecutiveNumber descending
                             select new ReportStatusInvoicesDto
                             {
                                 Id= invoice.Id,
                                 ClientId = invoice.ClientId,
                                 ClientCode = invoice.Client != null ? invoice.Client.Code : 0,
                                 ClientName = invoice.ClientName != null ? invoice.ClientName : string.Empty,
                                 ConsecutiveNumber = invoice.ConsecutiveNumber,
                                 Notes = (from p in invoice.Notes
                                          select new ReportStatusNoteDto
                                          {
                                              NoteType = p.NoteType,
                                              Amount = p.Amount,
                                              CodigoMoneda = p.CodigoMoneda,
                                              ConsecutiveNumber = p.ConsecutiveNumber,
                                              TaxAmount = p.TaxAmount,
                                              Total = p.Total,
                                              SendInvoice = p.SendInvoice,
                                              StatusTribunet = p.StatusTribunet,
                                              CreationTime = p.CreationTime,
                                          IsNoteReceptionConfirmed = p.IsNoteReceptionConfirmed,
                                          HasMessageTaxAdministration = p.MessageTaxAdministration != null
                                          }).ToList(),
                                 CodigoMoneda = invoice.CodigoMoneda,
                                 DueDate = invoice.DueDate,
                                 Status = invoice.Status,
                                 Total = invoice.Total,
                                 SendInvoice = invoice.SendInvoice,
                                 StatusTribunet = invoice.StatusTribunet,
                                 IsInvoiceReceptionConfirmed = invoice.IsInvoiceReceptionConfirmed,
                                 MessageTaxAdministration = invoice.MessageTaxAdministration,
                             TypeDocument=invoice.TypeDocument,
                             HasElectronicBill = invoice.ElectronicBill != null
                             }).ToList();
                return query;
            }
            else
            {
                return null;
            }
            
        }


        public IList<ReportInvoicesNotes> SearchReportInvoicesNotes(ReportStatusInvoicesInputDto<ReportInvoicesNotes> searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();

            if (drawers != null)
            {
                var predicate = PredicateBuilder.New<Invoice>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));
                var predicateNote = PredicateBuilder.New<Note>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));
                IQueryable<Invoice> invoiceList = null;
                IQueryable<Note> invoiceNote = null;

                if (searchInput.InitialDate != null)
                {
                    predicate = predicate.And(a => a.DueDate >= searchInput.InitialDate);
                    predicateNote = predicateNote.And(a => a.CreationTime >= searchInput.InitialDate);
                }

                if (searchInput.FinalDate != null)
                {
                    predicate = predicate.And(a => a.DueDate <= searchInput.FinalDate);
                    predicateNote = predicateNote.And(a => a.CreationTime <= searchInput.FinalDate);
                }

                if (searchInput.Status != null)
                    predicate = predicate.And(a => a.StatusTribunet == searchInput.Status.Value);

                if (searchInput.StatusPay != null)
                    predicate = predicate.And(a => a.Status == searchInput.StatusPay.Value);

                if (searchInput.StatusReception != null)
                {
                    bool isReception = searchInput.StatusReception == 0 ? false : true;
                    predicate = predicate.And(a => a.IsInvoiceReceptionConfirmed == isReception);
                }

                if (searchInput.NumberInvoice != null)
                {
                    var numero = searchInput.NumberInvoice.Length < 10 ? searchInput.NumberInvoice : searchInput.NumberInvoice.Substring(10);
                    var number = numero.ToString().PadLeft(10, '0');
                    predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);
                    predicateNote = predicateNote.And(a => a.ConsecutiveNumber.Substring(10) == number);
                }
                if (searchInput.ClientId != null)
                {
                    predicate = predicate.And(a => a.ClientId == searchInput.ClientId);
                    predicateNote = predicateNote.And(a => a.Invoice.ClientId == searchInput.ClientId);
                }

                if (searchInput.GroupsId != null)
                {
                    predicate = predicate.And(a => a.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));
                    predicateNote = predicateNote.And(a => a.Invoice.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));
                }

                if (searchInput.ConsecutiveNumberReference != null)
                {
                    predicateNote = predicateNote.And(a => a.Invoice.ConsecutiveNumber == searchInput.ConsecutiveNumberReference);
                }
                if (searchInput.Type != null & searchInput.Type != TypeDocument.Invoice)
                {
                    if (searchInput.Type == TypeDocument.NoteCredito)
                        predicateNote = predicateNote.And(a => a.NoteType == NoteType.Credito);

                    if (searchInput.Type == TypeDocument.NoteDebito)
                        predicateNote = predicateNote.And(a => a.NoteType == NoteType.Debito);
                }

                if (searchInput.CodigoMoneda != null)
                {

                    string value = searchInput.CodigoMoneda.ToString();

                    if (Enum.TryParse(value, out FacturaElectronicaResumenFacturaCodigoMoneda facturaMoneda))
                    {
                        predicate = predicate.And(a => a.CodigoMoneda == facturaMoneda);
                    }

                    if (Enum.TryParse(value, out NoteCodigoMoneda noteCodigoMoneda))
                    {
                        predicateNote = predicateNote.And(a => a.CodigoMoneda == noteCodigoMoneda);
                    }
                }

                if (searchInput.BranchOfficeId != null)
                {
                    predicate = predicate.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
                    predicateNote = predicateNote.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
                }
                if (searchInput.DrawerId != null)
                {
                    predicate = predicate.And(a => a.DrawerId == searchInput.DrawerId);
                    predicateNote = predicateNote.And(a => a.DrawerId == searchInput.DrawerId);
                }
                if (searchInput.Type != null)
                {
                    if (searchInput.Type == TypeDocument.Invoice)
                        predicate = predicate.And(a => a.TypeDocument == TypeDocumentInvoice.Invoice);


                    if (searchInput.Type == TypeDocument.Ticket)
                        predicate = predicate.And(a => a.TypeDocument == TypeDocumentInvoice.Ticket);

                    if (searchInput.Type == TypeDocument.NoteCredito)
                        predicateNote = predicateNote.And(a => a.NoteType == NoteType.Credito);

                    if (searchInput.Type == TypeDocument.NoteDebito)
                        predicateNote = predicateNote.And(a => a.NoteType == NoteType.Debito);
                }

                if (searchInput.MinimumAmount != 0 || searchInput.MaxAmount != 0)
                {
                    invoiceList = _invoiceRepository.GetAll().Where(predicate);
                    invoiceNote = _noteRepository.GetAll().Where(predicateNote);
                    var client = invoiceList.Select(a => a.ClientId).Distinct();

                    //var client = invoiceList.Where(a => a.ClientId != null).Select(a => a.ClientId).Distinct();

                    var predicate2 = PredicateBuilder.New<Invoice>(false);
                    var predicateNote2 = PredicateBuilder.New<Note>(false);

                    int i = 0;
                    if (searchInput.CodigoMoneda != null)
                    {
                        foreach (var item in client)
                        {
                            var SumTotalInvoice = invoiceList.Where(a => a.ClientId == item).Sum(a => a.Total);
                            var SumTotalNoteCredit = invoiceNote.Where(a => a.Invoice.ClientId == item).Where(a => a.NoteType == NoteType.Credito).Sum(a => (decimal?)a.Total) ?? 0;
                            var SumTotalNoteDebit = invoiceNote.Where(a => a.Invoice.ClientId == item).Where(a => a.NoteType == NoteType.Debito).Sum(a => (decimal?)a.Total) ?? 0;
                            var Total = (SumTotalInvoice + SumTotalNoteDebit) - SumTotalNoteCredit;

                            if (Total >= searchInput.MinimumAmount && Total <= searchInput.MaxAmount)
                            {
                                i++;
                                var id = item;
                                predicate2 = predicate2.Or(a => a.ClientId == item);
                                predicateNote2 = predicateNote2.Or(a => a.Invoice.ClientId == item);
                            }

                        }

                    }
                    else
                    {

                        // obterner la tasa 
                        var fechaInicio = DateTimeZone.Now();
                        var fechaFin = DateTimeZone.Now();
                        ResultRateDto rate = _rateOfDay.GetDayRate(fechaInicio, fechaFin).FirstOrDefault();

                        foreach (var item in client)
                        {
                            var SumTotalInvoiceCRC = invoiceList.Where(a => a.ClientId == item && a.CodigoMoneda == FacturaElectronicaResumenFacturaCodigoMoneda.CRC).Sum(a => (decimal?)a.Total) ?? 0;
                            var SumTotalNoteCreditCRC = invoiceNote.Where(a => a.Invoice.ClientId == item && a.CodigoMoneda == NoteCodigoMoneda.CRC && a.NoteType == NoteType.Credito).Sum(a => (decimal?)a.Total) ?? 0;
                            var SumTotalNoteDebitCRC = invoiceNote.Where(a => a.Invoice.ClientId == item && a.CodigoMoneda == NoteCodigoMoneda.CRC && a.NoteType == NoteType.Debito).Sum(a => (decimal?)a.Total) ?? 0;

                            var SumTotalInvoiceUSD = invoiceList.Where(a => a.ClientId == item && a.CodigoMoneda == FacturaElectronicaResumenFacturaCodigoMoneda.USD).Sum(a => (decimal?)(a.Total * rate.rate)) ?? 0;
                            var SumTotalNoteCreditUSD = invoiceNote.Where(a => a.Invoice.ClientId == item && a.CodigoMoneda == NoteCodigoMoneda.USD && a.NoteType == NoteType.Credito).Sum(a => (decimal?)(a.Total * rate.rate)) ?? 0;
                            var SumTotalNoteDebitUSD = invoiceNote.Where(a => a.Invoice.ClientId == item && a.CodigoMoneda == NoteCodigoMoneda.USD && a.NoteType == NoteType.Debito).Sum(a => (decimal?)(a.Total * rate.rate)) ?? 0;

                            

                            var SumTotalInvoice = SumTotalInvoiceCRC + SumTotalInvoiceUSD;
                            var SumTotalNoteCredit = SumTotalNoteCreditCRC + SumTotalNoteCreditUSD;
                            var SumTotalNoteDebit = SumTotalNoteDebitCRC + SumTotalNoteDebitUSD;
                            

                            var Total = (SumTotalInvoice + SumTotalNoteDebit) - SumTotalNoteCredit;

                            if (Total >= searchInput.MinimumAmount && Total <= searchInput.MaxAmount)
                            {
                                i++;
                                var id = item;
                                predicate2 = predicate2.Or(a => a.ClientId == item);
                                predicateNote2 = predicateNote2.Or(a => a.Invoice.ClientId == item);
                            }

                        }

                    }
                    

                    predicate = predicate.And(predicate2);
                    predicateNote = predicateNote.And(predicateNote2);

                }

                invoiceList = _invoiceRepository.GetAll().Where(predicate);
                invoiceNote = _noteRepository.GetAll().Where(predicateNote);

                var query = new List<ReportInvoicesNotes>();

                if ((searchInput.Type == null) || (searchInput.Type == TypeDocument.Invoice) || searchInput.Type == TypeDocument.Ticket) {
                    if (searchInput.ConsecutiveNumberReference == null)
                    {
                        var queryInvoice = (from invoice in invoiceList.Include(a => a.Client).Include(a => a.InvoicePaymentTypes)
                                            orderby invoice.ConsecutiveNumber descending
                                            select new ReportInvoicesNotes
                                            {
                                                Type = invoice.TypeDocument == TypeDocumentInvoice.Invoice ? TypeDocument.Invoice : TypeDocument.Ticket,
                                                ClientId = invoice.ClientId,
                                                ClientName = invoice.ClientName,
                                                ClientIdentification = invoice.ClientIdentificationType != IdentificacionTypeTipo.NoAsiganda ? invoice.ClientIdentification : string.Empty,
                                                ConsecutiveNumber = invoice.ConsecutiveNumber,
                                                PaymentInvoices = (from p in invoice.InvoicePaymentTypes
                                                                   select new ReportPaymentInvoice
                                                                   {
                                                                       Amount = p.Amount,
                                                                       PaymentDate = p.Payment.PaymentDate,
                                                                       LastModificationTime = p.LastModificationTime,
                                                                       PaymentInvoiceType = p.Payment.PaymentType,
                                                                       PaymetnMethodType = p.Payment.PaymetnMethodType,
                                                                       Transaction = p.Payment.Transaction
                                                                   }).ToList(),
                                                CodigoMoneda = invoice.CodigoMoneda.ToString(),
                                                Date = invoice.DueDate,
                                                Status = invoice.Status,
                                                Total = invoice.Total,
                                                Impuesto = invoice.TotalTax,
                                                Descuento = invoice.DiscountAmount,
                                                StatusTribunet = invoice.StatusTribunet,
                                                IsInvoiceReceptionConfirmed = invoice.IsInvoiceReceptionConfirmed,
                                                IdentificationType = invoice.ClientIdentificationType,
                                                IdentificacionExtranjero = invoice.ClientIdentificationType == IdentificacionTypeTipo.NoAsiganda ? invoice.ClientIdentification : string.Empty

                                            }).ToList();
                        query.AddRange(queryInvoice);
                    }
                }
                if ((searchInput.Type == null) || (searchInput.Type == TypeDocument.NoteCredito) || (searchInput.Type == TypeDocument.NoteDebito))
                {
                    var queryNote = (from note in invoiceNote.Include(a => a.Invoice).Include(a => a.Invoice.Client).Include(a => a.CodigoMoneda)
                             orderby note.ConsecutiveNumber descending
                                     select new ReportInvoicesNotes
                                     {
                                         Type = (TypeDocument)note.NoteType,
                                         ClientId = note.Invoice.ClientId,
                                         ClientName = note.Invoice.ClientName,
                                         ClientIdentification = note.Invoice.ClientIdentificationType != IdentificacionTypeTipo.NoAsiganda ? note.Invoice.ClientIdentification : string.Empty,
                                         ConsecutiveNumber = note.ConsecutiveNumber,
                                         ConsecutiveNumberReference = note.Invoice.ConsecutiveNumber,
                                         PaymentInvoices = (from p in note.NotePaymentTypes
                                                            select new ReportPaymentInvoice
                                                            {
                                                                Amount = p.Amount,
                                                                PaymentDate = p.Payment.PaymentDate,
                                                                LastModificationTime = p.LastModificationTime,
                                                                PaymentInvoiceType = p.Payment.PaymentType,
                                                                PaymetnMethodType = p.Payment.PaymetnMethodType,
                                                                Transaction = p.Payment.Transaction
                                                            }).ToList(),
                                         CodigoMoneda = note.CodigoMoneda.ToString(),
                                         Date = note.CreationTime,
                                         Total = note.Total,
                                         Impuesto = note.TaxAmount,
                                         Descuento = note.DiscountAmount,
                                         StatusTribunet = note.StatusTribunet,
                                         IdentificationType = note.Invoice.ClientIdentificationType,
                                         IdentificacionExtranjero = note.Invoice.ClientIdentificationType == IdentificacionTypeTipo.NoAsiganda ? note.Invoice.ClientIdentification : string.Empty

                                     }).ToList();

                    query.AddRange(queryNote);

                   
                }

                return query;
            }
            else
            {
                return null;
            }
        }

        public IList<IntegracionZohoSVConta> SearchIntegrationSVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            var query = new List<IntegracionZohoSVConta>();

            if ((searchInput.Type==null)|| (searchInput.Type== TypeDocument.Invoice) || (searchInput.Type == TypeDocument.Ticket))
                query = QueryFacturaSVConta(searchInput);

            if ((searchInput.Type == null) || (searchInput.Type == TypeDocument.NoteDebito) || (searchInput.Type == TypeDocument.NoteCredito))
            {
                var queryNote = QueryNoteSVConta(searchInput);
                query.AddRange(queryNote);
            }
                
            return query;
        }

        public IList<IntegracionZohoSVConta> SearchIntegrationZoho(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            
            var query = new List<IntegracionZohoSVConta>();
                if ((searchInput.Type == TypeDocument.Invoice)|| searchInput.Type== TypeDocument.Ticket)
                {
                query = QueryFacturaZoho(searchInput);
                }
            //ojo con esto
                else if (searchInput.Type == TypeDocument.NoteCredito || searchInput.Type == TypeDocument.NoteDebito)
                {
                query = QueryNoteZoho(searchInput);
                }

            return query;
        }

        public DataTable DataTableZohoCVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput, IntegrationZohoSVConta integrationZohoSVConta)
        {
            DataTable dt = new DataTable();
            switch (integrationZohoSVConta)
            {
                case IntegrationZohoSVConta.SVConta:
                    dt = DataTableCVConta(searchInput);
                    break;
                case IntegrationZohoSVConta.Zoho:
                    switch (searchInput.Type)
                    {
                        case TypeDocument.Invoice:
                            dt = DataTableZohoInvoice(searchInput);
                            break;
                        case TypeDocument.NoteCredito:
                            dt = DataTableZohoCreditNote(searchInput, TypeDocument.NoteCredito);
                            break; ;
                        case TypeDocument.NoteDebito:
                            dt = DataTableZohoCreditNote(searchInput, TypeDocument.NoteDebito);
                            break;
                    }
                    break;
            }
            return dt;
        }

        private DataTable DataTableZohoInvoice(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nro Documento", typeof(string));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Fecha Documento", typeof(string));
            dt.Columns.Add("Fecha de Vencimiento", typeof(string));
            dt.Columns.Add("Moneda", typeof(string));
            dt.Columns.Add("itemPrice", typeof(decimal));
            dt.Columns.Add("cantidad", typeof(decimal));
            dt.Columns.Add("servicio", typeof(string));
            dt.Columns.Add("Descuento_porce", typeof(decimal));
            dt.Columns.Add("Descuento_amount", typeof(decimal));
            dt.Columns.Add("Item Tax1", typeof(string));
            dt.Columns.Add("item tax %", typeof(decimal));
            dt.Columns.Add("Item Tax1 Amount", typeof(decimal));
            dt.Columns.Add("Item Total", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));

            var result = SearchIntegrationZoho(searchInput);
            CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");
            string expiracion = "";
            foreach (var r in result.ToList())
            {
                if (r.ExpirationDate != null)
                {
                    expiracion = r.ExpirationDate.Value.ToString("dd/MM/yyyy");
                }

                foreach(var l in r.InvoiceSVContaLines)
                dt.Rows.Add(r.ConsecutiveNumber.ToString(formato),
                            r.ClientName,
                            r.DateDocument.ToString("dd/MM/yyyy"),
                            expiracion,
                            r.CodigoMoneda,
                                l.ItemPrice.ToString("###,###,###.00", formato),
                                l.Cantidad.ToString("###,###,###.00", formato),
                                l.Servicio,
                                l.DiscountPercentage.ToString("###,###,###.00", formato),
                                l.DiscountAmount.ToString("###,###,###.00", formato),
                                l.ItemTax1,
                                l.ItemTax.ToString("###,###,###.00", formato),
                                l.ItemTaxAmount.ToString("###,###,###.00", formato),
                                l.ItemTotal.ToString("###,###,###.00", formato),
                                l.Total.ToString("###,###,###.00", formato));
            }

            return dt;
        }

        private DataTable DataTableZohoCreditNote(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput, TypeDocument type)
        {
            DataTable dt = new DataTable();
            if (type.Equals(TypeDocument.NoteCredito))
            {
                dt.Columns.Add("Credit Note Date", typeof(string));
                dt.Columns.Add("Credit Note Number", typeof(string));
            }
            if (type.Equals(TypeDocument.NoteDebito))
            {
                dt.Columns.Add("Debit Note Date", typeof(string));
                dt.Columns.Add("Debit Note Number", typeof(string));
            }

            dt.Columns.Add("Customer Name", typeof(string));
            dt.Columns.Add("Currency Code", typeof(string));
            dt.Columns.Add("Item Name", typeof(string));
            dt.Columns.Add("Item Desc", typeof(string));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("Item Price", typeof(decimal));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("Discount Amount", typeof(decimal));
            dt.Columns.Add("Item Tax", typeof(string));
            dt.Columns.Add("Item Tax %", typeof(decimal));
            dt.Columns.Add("Item Tax Amount", typeof(decimal));
            dt.Columns.Add("Item Total", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));

            var result = SearchIntegrationZoho(searchInput);
            CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");
            string expiracion = "";
            foreach (var r in result.ToList())
            {
                if (r.ExpirationDate != null)
                {
                    expiracion = r.ExpirationDate.Value.ToString("dd/MM/yyyy");
                }
                foreach(var l in r.InvoiceSVContaLines)
                dt.Rows.Add(r.DateDocument.ToString("dd/MM/yyyy"),
                            r.ConsecutiveNumber.ToString(formato),
                            r.ClientName,
                            r.CodigoMoneda,
                                l.Servicio,
                                l.DescriptionDiscount,
                                l.Cantidad.ToString("###,###,###.00", formato),
                                l.ItemPrice.ToString("###,###,###.00", formato),
                                l.DiscountPercentage.ToString("###,###,###.00", formato),
                                l.DiscountAmount.ToString("###,###,###.00", formato),
                                l.ItemTax1,
                                l.ItemTax.ToString("###,###,###.00", formato),
                                l.ItemTaxAmount.ToString("###,###,###.00", formato),
                                l.ItemTotal.ToString("###,###,###.00", formato),
                                l.Total.ToString("###,###,###.00", formato));
            }

            return dt;
        }

        private DataTable DataTableCVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Tipo Documento", typeof(string));
            dt.Columns.Add("Nro Documento", typeof(string));
            dt.Columns.Add("Nombre Cliente", typeof(string));
            dt.Columns.Add("Identificacion/Pasaporte", typeof(string));
            dt.Columns.Add("Fecha Pasaporte", typeof(string));
            dt.Columns.Add("% Impuesto", typeof(decimal));
            dt.Columns.Add("Condición de Venta", typeof(string));
            dt.Columns.Add("Moneda", typeof(string));
            dt.Columns.Add("Monto Total Exento", typeof(decimal));
            dt.Columns.Add("Monto Total Gravado", typeof(decimal));

            var result = SearchIntegrationSVConta(searchInput);
            CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");

            foreach (var r in result)
            {
                dt.Rows.Add(
                        EnumHelper.GetDescription(r.Type),
                            r.ConsecutiveNumber.ToString(formato),
                            r.ClientName,
                            r.ClientIdentification,
                            r.DateDocument.ToString("dd/MM/yyyy"),
                            r.ItemTax.ToString("###,###,###.00", formato),
                            EnumHelper.GetDescription(r.ConditionSaleType),
                            r.CodigoMoneda,
                            r.TotalExento.ToString("###,###,###.00", formato),
                        r.TotalGravado.ToString("###,###,###.00", formato)
                        );
            }

            return dt;
        }

        private List<IntegracionZohoSVConta> QueryFacturaSVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();

            if (drawers != null)
            {
                var predicate = PredicateBuilder.New<Invoice>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));


                if (searchInput.Status != null)
                    predicate = predicate.And(a => a.StatusTribunet == searchInput.Status.Value);

                if (searchInput.StatusPay != null)
                    predicate = predicate.And(a => a.Status == searchInput.StatusPay.Value);

                if (searchInput.StatusReception != null)
                {
                    bool isReception = searchInput.StatusReception == 0 ? false : true;
                    predicate = predicate.And(a => a.IsInvoiceReceptionConfirmed == isReception);
                }

                if (searchInput.NumberInvoice != null)
                {
                    var numero = searchInput.NumberInvoice.Length < 10 ? searchInput.NumberInvoice : searchInput.NumberInvoice.Substring(10);
                    var number = numero.ToString().PadLeft(10, '0');
                    predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);
                }
                if (searchInput.ClientId != null)
                {
                    predicate = predicate.And(a => a.ClientId == searchInput.ClientId);
                }

                if (searchInput.InitialDate != null)
                {
                    predicate = predicate.And(a => a.DueDate >= searchInput.InitialDate);
                }


                if (searchInput.FinalDate != null)
                {
                    predicate = predicate.And(a => a.DueDate <= searchInput.FinalDate);
                }

                if (searchInput.CodigoMoneda != null)
                {
                    string value = searchInput.CodigoMoneda.ToString();

                    if (Enum.TryParse(value, out FacturaElectronicaResumenFacturaCodigoMoneda facturaMoneda))
                    {
                        predicate = predicate.And(a => a.CodigoMoneda == facturaMoneda);
                    }
                }

                if (searchInput.Type != null)
                {
                    if (searchInput.Type== TypeDocument.Invoice)
                        predicate = predicate.And(a => a.TypeDocument ==  TypeDocumentInvoice.Invoice);
                    if (searchInput.Type == TypeDocument.Ticket)
                        predicate = predicate.And(a => a.TypeDocument == TypeDocumentInvoice.Ticket);
                }
                    

                if (searchInput.BranchOfficeId != null)
                {
                    predicate = predicate.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
                }

                if (searchInput.DrawerId != null)
                {
                    predicate = predicate.And(a => a.DrawerId == searchInput.DrawerId);
                }

                var invoiceList = _invoiceRepository.GetAll().Where(predicate);

                var tax = _taxRepository.GetAll();

                var query = (from invoice in invoiceList.Include(a => a.InvoicePaymentTypes).Include(a => a.InvoiceLines)
                             orderby invoice.ConsecutiveNumber descending
                             select new IntegracionZohoSVConta
                             {
                                 Type = invoice.TypeDocument == TypeDocumentInvoice.Invoice ? TypeDocument.Invoice : TypeDocument.Ticket,
                                 ConsecutiveNumber = invoice.ConsecutiveNumber,
                                 ClientId = invoice.ClientId,
                                 ClientName = invoice.ClientName,
                                 ClientIdentification = invoice.ClientIdentificationType != IdentificacionTypeTipo.NoAsiganda ? invoice.ClientIdentification : string.Empty,
                                 DateDocument = invoice.DueDate,
                                 ExpirationDate = invoice.ExpirationDate,
                                 CodigoMoneda = invoice.CodigoMoneda.ToString(),
                                 TotalGravado = invoice.TotalGravado,
                                 TotalExento = invoice.TotalExento,
                                 ConditionSaleType = (FacturaElectronicaCondicionVenta)invoice.ConditionSaleType,
                                 ItemPrice = invoice.InvoiceLines.FirstOrDefault().PricePerUnit,
                                 Cantidad = invoice.InvoiceLines.FirstOrDefault().Quantity,
                                 Servicio = invoice.InvoiceLines.FirstOrDefault().Title,
                                 DescriptionDiscount = invoice.InvoiceLines.FirstOrDefault().Note,
                                 DiscountPercentage = invoice.InvoiceLines.FirstOrDefault().DiscountPercentage,
                                 DiscountAmount = invoice.DiscountAmount,
                                 IdentificationType = invoice.ClientIdentificationType,
                                 IdentificacionExtranjero = invoice.ClientIdentificationType == IdentificacionTypeTipo.NoAsiganda ? invoice.ClientIdentification : string.Empty,
                                 ItemTax1 = invoice.InvoiceLines.FirstOrDefault().Tax.Name,
                                 ItemTax = invoice.InvoiceLines.FirstOrDefault().Tax.Rate,
                                 ItemTaxAmount = invoice.InvoiceLines.FirstOrDefault().TaxAmount,
                                 ItemTotal = invoice.NetaSale,
                                 Total = invoice.Total
                             }).ToList();
                return query;
            }
            else
            {
                return null;
            }
           
        }

        private List<IntegracionZohoSVConta> QueryNoteSVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();
            if (drawers != null)
            {
                var predicateNote = PredicateBuilder.New<Note>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));

                if (searchInput.NumberInvoice != null)
                {
                    var numero = searchInput.NumberInvoice.Length < 10 ? searchInput.NumberInvoice : searchInput.NumberInvoice.Substring(10);
                    var number = numero.ToString().PadLeft(10, '0');
                    predicateNote = predicateNote.And(a => a.ConsecutiveNumber.Substring(10) == number);
                }
                if (searchInput.ClientId != null)
                {
                    predicateNote = predicateNote.And(a => a.Invoice.ClientId == searchInput.ClientId);
                }

                if (searchInput.InitialDate != null)
                {
                    predicateNote = predicateNote.And(a => a.CreationTime >= searchInput.InitialDate);
                }


                if (searchInput.FinalDate != null)
                {
                    predicateNote = predicateNote.And(a => a.CreationTime <= searchInput.FinalDate);
                }

                if (searchInput.CodigoMoneda != null)
                {

                    string value = searchInput.CodigoMoneda.ToString();

                    if (Enum.TryParse(value, out NoteCodigoMoneda noteCodigoMoneda))
                    {
                        predicateNote = predicateNote.And(a => a.CodigoMoneda == noteCodigoMoneda);
                    }
                }

                if (searchInput.Type != null)
                {
                    if (searchInput.Type == TypeDocument.NoteCredito)
                        predicateNote = predicateNote.And(a => a.NoteType == NoteType.Credito);

                    if (searchInput.Type == TypeDocument.NoteDebito)
                        predicateNote = predicateNote.And(a => a.NoteType == NoteType.Debito);
                }

                if (searchInput.BranchOfficeId != null)
                {
                    predicateNote = predicateNote.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
                }

                if (searchInput.DrawerId != null)
                {
                    predicateNote = predicateNote.And(a => a.DrawerId == searchInput.DrawerId);
                }

                var invoiceNote = _noteRepository.GetAll().Where(predicateNote);

                var tax = _taxRepository.GetAll();

                var query = (from note in invoiceNote.Include(a => a.Invoice).Include(a => a.CodigoMoneda).Include(a => a.NotesLines)
                             orderby note.ConsecutiveNumber descending
                             select new IntegracionZohoSVConta
                             {
                                 Type = (TypeDocument)note.NoteType,
                                 ClientId = note.Invoice.ClientId,
                                 ClientName = note.Invoice.ClientName,
                                 ClientIdentification = note.Invoice.ClientIdentificationType != IdentificacionTypeTipo.NoAsiganda ? note.Invoice.ClientIdentification : string.Empty,
                                 ConsecutiveNumber = note.ConsecutiveNumber,
                                 DateDocument = note.CreationTime,
                                 ExpirationDate = note.NoteType == NoteType.Credito ? note.Invoice.ExpirationDate : null,
                                 CodigoMoneda = note.CodigoMoneda.ToString(),
                                 TotalGravado = note.Invoice.TotalGravado,
                                 TotalExento = note.Invoice.TotalExento,
                                 ConditionSaleType = (FacturaElectronicaCondicionVenta)note.Invoice.ConditionSaleType,
                                 ItemPrice = note.NotesLines.FirstOrDefault().PricePerUnit,
                                 Cantidad = note.NotesLines.FirstOrDefault().Quantity,
                                 Servicio = note.NotesLines.FirstOrDefault().Title,
                                 DescriptionDiscount = note.NotesLines.FirstOrDefault().Notes,
                                 DiscountPercentage = note.NotesLines.FirstOrDefault().DiscountPercentage,
                                 DiscountAmount = note.Invoice.DiscountAmount,
                                 IdentificationType = note.Invoice.ClientIdentificationType,
                                 IdentificacionExtranjero = note.Invoice.ClientIdentificationType == IdentificacionTypeTipo.NoAsiganda ? note.Invoice.ClientIdentification : string.Empty,
                                 ItemTax1 = note.NotesLines.Count > 0 ? note.NotesLines.FirstOrDefault().Tax.Name : "",
                                 ItemTax = note.NotesLines.Count > 0 ? note.NotesLines.FirstOrDefault().Tax.Rate : 0,
                                 ItemTaxAmount = note.TaxAmount,
                                 ItemTotal = note.Amount,
                                 Total = note.Total
                             }).ToList();
                return query;
            }
            else
                return null;

        }

        private List<IntegracionZohoSVConta> QueryFacturaZoho(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            var predicate = PredicateBuilder.New<Invoice>(true);


            if (searchInput.Status != null)
                predicate = predicate.And(a => a.StatusTribunet == searchInput.Status.Value);

            if (searchInput.StatusPay != null)
                predicate = predicate.And(a => a.Status == searchInput.StatusPay.Value);

            if (searchInput.StatusReception != null)
            {
                bool isReception = searchInput.StatusReception == 0 ? false : true;
                predicate = predicate.And(a => a.IsInvoiceReceptionConfirmed == isReception);
            }

            if (searchInput.NumberInvoice != null)
            {
                var numero = searchInput.NumberInvoice.Length < 10 ? searchInput.NumberInvoice : searchInput.NumberInvoice.Substring(10);
                var number = numero.ToString().PadLeft(10, '0');
                predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);
            }
            if (searchInput.ClientId != null)
            {
                predicate = predicate.And(a => a.ClientId == searchInput.ClientId);
            }

            if (searchInput.InitialDate != null)
            {
                predicate = predicate.And(a => a.DueDate >= searchInput.InitialDate);
            }


            if (searchInput.FinalDate != null)
            {
                predicate = predicate.And(a => a.DueDate <= searchInput.FinalDate);
            }

            if (searchInput.CodigoMoneda != null)
            {
                string value = searchInput.CodigoMoneda.ToString();

                if (Enum.TryParse(value, out FacturaElectronicaResumenFacturaCodigoMoneda facturaMoneda))
                {
                    predicate = predicate.And(a => a.CodigoMoneda == facturaMoneda);
                }
            }

            if (searchInput.Type != null)
            {
                if (searchInput.Type == TypeDocument.Invoice)
                    predicate = predicate.And(a => a.TypeDocument ==  TypeDocumentInvoice.Invoice);

                if (searchInput.Type == TypeDocument.Ticket)
                    predicate = predicate.And(a => a.TypeDocument == TypeDocumentInvoice.Ticket);
            }

            if (searchInput.BranchOfficeId != null)
            {
                predicate = predicate.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
            }

            if (searchInput.DrawerId != null)
            {
                predicate = predicate.And(a => a.DrawerId == searchInput.DrawerId);
            }

            var invoiceList = _invoiceRepository.GetAll().Where(predicate);

            var tax = _taxRepository.GetAll();

            var query = (from invoice in invoiceList.Include(a => a.Client).Include(a => a.InvoicePaymentTypes)
                         orderby invoice.Number descending
                         select new IntegracionZohoSVConta
                         {
                             Type = TypeDocument.Invoice,
                             ConsecutiveNumber = invoice.ConsecutiveNumber,
                             ClientId = invoice.ClientId,
                             ClientName = invoice.ClientName,
                             ClientIdentification = invoice.ClientIdentificationType != IdentificacionTypeTipo.NoAsiganda ? invoice.ClientIdentification : string.Empty,
                             DateDocument = invoice.DueDate,
                             ExpirationDate = invoice.ExpirationDate,
                             CodigoMoneda = invoice.CodigoMoneda.ToString(),
                             TotalGravado = invoice.TotalGravado,
                             TotalExento = invoice.TotalExento,
                             ConditionSaleType = (FacturaElectronicaCondicionVenta)invoice.ConditionSaleType,
                             InvoiceSVContaLines = (from i in invoice.InvoiceLines
                                                    select new InvoiceSVContaLines
                                                    {
                                                        ItemPrice = i.PricePerUnit,
                                                        Cantidad = i.Quantity,
                                                        Servicio=i.Title,
                                                        DescriptionDiscount=i.Note,
                                                        DiscountPercentage=i.DiscountPercentage,
                                                        DiscountAmount = Math.Round(Math.Round(i.PricePerUnit*i.Quantity)*(i.DiscountPercentage/100)),
                                                        ItemTax1=i.Tax.Name,
                                                        ItemTax=i.Tax.Rate,
                                                        ItemTaxAmount=i.TaxAmount,
                                                        ItemTotal=i.SubTotal,
                                                        Total=i.LineTotal
                                                    }).ToList(),
                             DiscountAmount = invoice.DiscountAmount,
                             IdentificationType = invoice.ClientIdentificationType,
                             IdentificacionExtranjero = (invoice.ClientIdentificationType== IdentificacionTypeTipo.NoAsiganda?invoice.ClientIdentification:string.Empty)
                         }).ToList();
            return query;
        }

        private List<IntegracionZohoSVConta> QueryNoteZoho(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput)
        {
            var predicateNote = PredicateBuilder.New<Note>(true);

            if (searchInput.NumberInvoice != null)
            {
                var numero = searchInput.NumberInvoice.Length < 10 ? searchInput.NumberInvoice : searchInput.NumberInvoice.Substring(10);
                var number = numero.ToString().PadLeft(10, '0');
                predicateNote = predicateNote.And(a => a.ConsecutiveNumber.Substring(10) == number);
            }
            if (searchInput.ClientId != null)
            {
                predicateNote = predicateNote.And(a => a.Invoice.ClientId == searchInput.ClientId);
            }

            if (searchInput.InitialDate != null)
            {
                predicateNote = predicateNote.And(a => a.CreationTime >= searchInput.InitialDate);
            }


            if (searchInput.FinalDate != null)
            {
                predicateNote = predicateNote.And(a => a.CreationTime <= searchInput.FinalDate);
            }

            if (searchInput.CodigoMoneda != null)
            {

                string value = searchInput.CodigoMoneda.ToString();

                if (Enum.TryParse(value, out NoteCodigoMoneda noteCodigoMoneda))
                {
                    predicateNote = predicateNote.And(a => a.CodigoMoneda == noteCodigoMoneda);
                }
            }

            if (searchInput.Type != null)
            {
                if (searchInput.Type == TypeDocument.NoteCredito)
                    predicateNote = predicateNote.And(a => a.NoteType == NoteType.Credito);

                if (searchInput.Type == TypeDocument.NoteDebito)
                    predicateNote = predicateNote.And(a => a.NoteType == NoteType.Debito);
            }
            if (searchInput.BranchOfficeId != null)
            {
                predicateNote = predicateNote.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
            }

            if (searchInput.DrawerId != null)
            {
                predicateNote = predicateNote.And(a => a.DrawerId == searchInput.DrawerId);
            }
            var invoiceNote = _noteRepository.GetAll().Where(predicateNote);

            var tax = _taxRepository.GetAll();

           

            var query = (from note in invoiceNote.Include(a => a.Invoice).Include(a => a.Invoice.Client).Include(a => a.CodigoMoneda)
                         orderby note.CreationTime
                         select new IntegracionZohoSVConta
                         {
                             Type = (TypeDocument)note.NoteType,
                             ClientId = note.Invoice.ClientId,
                             ClientName = note.Invoice.ClientName,
                             ClientIdentification = note.Invoice.ClientIdentification,
                             ConsecutiveNumber = note.ConsecutiveNumber,
                             DateDocument = note.CreationTime,
                             ExpirationDate = note.NoteType == NoteType.Credito ? note.Invoice.ExpirationDate : null,
                             CodigoMoneda = note.CodigoMoneda.ToString(),
                             TotalGravado = note.Invoice.TotalGravado,
                             TotalExento = note.Invoice.TotalExento,
                             ConditionSaleType = (FacturaElectronicaCondicionVenta)note.Invoice.ConditionSaleType,
                             InvoiceSVContaLines = (from i in note.NotesLines
                                                    select new InvoiceSVContaLines
                                                    {
                                                        ItemPrice = i.PricePerUnit,
                                                        Cantidad = i.Quantity,
                                                        Servicio = i.Title,
                                                        DescriptionDiscount = i.Notes,
                                                        DiscountPercentage = i.DiscountPercentage,
                                                        ItemTax1 = i.Tax.Name,
                                                        ItemTax = i.Tax.Rate,
                                                        ItemTaxAmount = i.TaxAmount,
                                                        ItemTotal = i.SubTotal,
                                                        Total = i.LineTotal
                                                    }).ToList(),
                             DiscountAmount = note.Invoice.DiscountAmount,
                             IdentificationType = note.Invoice.ClientIdentificationType,
                             IdentificacionExtranjero = note.Invoice.ClientIdentificationType== IdentificacionTypeTipo.NoAsiganda? note.Invoice.ClientIdentification: string.Empty
                         }).ToList();
            return query;
        }
    }
}
