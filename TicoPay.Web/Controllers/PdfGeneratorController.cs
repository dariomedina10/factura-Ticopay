using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PdfGenerator;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using System.IO;
using TicoPay.ReportsSettings;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using TicoPay.Core.Common;
using TicoPay.BranchOffices;
using TicoPay.BranchOffices.Dto;
using TicoPay.Drawers;
using Abp.Domain.Repositories;
using TicoPay.Invoices.Dto;
using static TicoPay.MultiTenancy.Tenant;
using TicoPay.Printers;
using TicoPay.Common;

namespace TicoPay.Web.Controllers
{

    public class PdfGeneratorController : PdfViewController
    {
        private readonly IInvoiceAppService _invoiceService;
        
        private readonly TenantManager _tenantManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IReportSettingsAppService _reportSettingsAppService;
        private ReportSettings _facturaReportSettings;
        private ReportSettings _notaReportSettings;
        private readonly IRepository<Bank, Guid> _bankRepository;
        private readonly IRepository<PaymentInvoice, Guid> _paymentinvoiceRepository;
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        private readonly IRepository<BranchOffice, Guid> _branchOfficeRepository;

        public PdfGeneratorController(IRepository<BranchOffice, Guid> branchOfficeRepository, IRepository<Drawer, Guid> drawerRepository, IRepository<PaymentInvoice, Guid> paymentinvoiceRepository, IRepository<Bank, Guid> bankRepository, IInvoiceAppService invoiceService, TenantManager tenantManager, 
            IInvoiceManager invoiceManager, IReportSettingsAppService reportSettingsAppService)
        {
            _invoiceService = invoiceService;
            _tenantManager = tenantManager;
            _invoiceManager = invoiceManager;
            _reportSettingsAppService = reportSettingsAppService;
            _drawerRepository = drawerRepository;
            _branchOfficeRepository = branchOfficeRepository;
            _bankRepository = bankRepository;
            _paymentinvoiceRepository = paymentinvoiceRepository;
        }

        // GET: PdfGenerator
        public ActionResult Index()
        {
            return ViewPdf("prueba", "~/Content/bootstrap.min.css", "TestPage", null, true); ;
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult InvoiceDetails(Guid id, bool isPost)
        {
            Clients.Client client = null;
            var invoice = _invoiceManager.Get(id);
            if (invoice.ClientId != null)
                client = _invoiceService.GetClientPdf(invoice.Client.Id);
            var tenant = _tenantManager.Get(invoice.TenantId);

            var payments = InfoPaymentInvoice(id);
            var listInfoPago = AddPaymentsMethods2(payments);

            var document = string.Empty;
            var drawer = _drawerRepository.GetAll().Where(x => x.TenantId == tenant.Id && x.IsOpen == true).FirstOrDefault();
            var branchOffice = infoBranchOffice(drawer.BranchOfficeId);
            var BranchOfficeInfo = AddBranchOfficeInfo(branchOffice);

            if (isPost)
            {
                return JavaScript("jsWebClientPrint.print('id="+ id + "&tenantId=" + tenant.Id + "&tipoDocumento=" + invoice.TypeDocument + "');");
            }

            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);
            GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
            Stream pdfStream = generatePDF.CreatePDFAsStream(invoice, client, tenant, listInfoPago, BranchOfficeInfo);

            return File(pdfStream, System.Net.Mime.MediaTypeNames.Application.Pdf, invoice.VoucherKey + ".pdf");
        }


        public List<BranchOfficesDto> infoBranchOffice(Guid id)
        {

            var query = (from branchOffices in _branchOfficeRepository.GetAll()
                         where branchOffices.Id == id
                         select new BranchOfficesDto
                         {
                             Id = branchOffices.Id,
                             Location = branchOffices.Location,
                             Name = branchOffices.Name,
                             TenantId = branchOffices.TenantId,

                         }).ToList();

            return query;
        }
        public List<BranchOffice> AddBranchOfficeInfo(List<BranchOfficesDto> listbranchoffice)
        {
            List<BranchOffice> informacionBranchOffice = new List<BranchOffice>();

            foreach (BranchOfficesDto branch in listbranchoffice)
            {
                if (branch.Id != null)
                {
                    BranchOffice BranchDrawOpened = new BranchOffice();
                    BranchDrawOpened.Id = branch.Id;
                    BranchDrawOpened.Name = branch.Name;
                    BranchDrawOpened.Location = branch.Location;
                    BranchDrawOpened.TenantId = branch.TenantId;
                    informacionBranchOffice.Add(BranchDrawOpened);
                }
            }
            return informacionBranchOffice;
        }
        
        public List<PaymentInvoice> AddPaymentsMethods2(List<PaymentInvoceDto> listPaymetnInvoce)
        {
            List<PaymentInvoice> informacionPago = new List<PaymentInvoice>();

            foreach (PaymentInvoceDto paymetnInvoce in listPaymetnInvoce)
            {
                if (paymetnInvoce.UserCard != null)
                {
                    PaymentInvoice paymetnInvoceCard = new PaymentInvoice();
                    paymetnInvoceCard.PaymetnMethodType = paymetnInvoce.PaymetnMethodType;
                    paymetnInvoceCard.UserCard = paymetnInvoce.UserCard;
                    informacionPago.Add(paymetnInvoceCard);
                }
                if (paymetnInvoce.BankId != null)
                {
                    PaymentInvoice paymentInvoiceDeposit = new PaymentInvoice();
                    paymentInvoiceDeposit.PaymetnMethodType = paymetnInvoce.PaymetnMethodType;
                    paymentInvoiceDeposit.UserCard = paymetnInvoce.BankName;
                    paymentInvoiceDeposit.BankId = paymetnInvoce.BankId;
                    informacionPago.Add(paymentInvoiceDeposit);
                }
            }
            return informacionPago;
        }

        public List<PaymentInvoceDto> InfoPaymentInvoice (Guid id)         {

            var query = (from payments in _paymentinvoiceRepository.GetAll()
                        where payments.InvoiceId == id
                        select new PaymentInvoceDto
                        {
                            Id = payments.Id,
                            PaymetnMethodType = payments.PaymetnMethodType,
                            BankId = payments.BankId,
                            BankName = string.Empty,
                            UserCard = payments.UserCard,
                            TenantId = payments.TenantId,
                            
                        }).ToList();

            literalbanco(query);

            return query;
        }

        void literalbanco(List<PaymentInvoceDto> query)
        {
            foreach( var list in query)
            {
                if(list.BankId != null)
                list.BankName = getBankName(list.BankId);
            }
        }

        private string getBankName (Guid? bankId)
        {
            
            var name = _bankRepository.FirstOrDefault(x => x.IsActive && x.Id == bankId);
            var nameBank = name.Name;

            return nameBank;

        }
        public ActionResult NoteDetails(Guid invoiceId, Guid noteId)
        {
            Clients.Client client = null;

            Invoice invoice = _invoiceManager.Get(invoiceId);
            if (invoice.ClientId != null)
                client = _invoiceService.GetClientPdf(invoice.Client.Id);

            var tenant = _tenantManager.Get(invoice.TenantId);
            var note = invoice.Notes.Where(n => n.Id == noteId).FirstOrDefault();

            _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Nota, tenant.ComercialName);
            GeneratePDF generatePDF = new GeneratePDF(_notaReportSettings);
            Stream pdfStream = generatePDF.CreatePDFNoteAsStream(invoice, client, tenant, note, null);

            return File(pdfStream, System.Net.Mime.MediaTypeNames.Application.Pdf, "note_" + note.VoucherKey + ".pdf");
        }

        public ActionResult Print(Guid id, TypeDocumentInvoice type)
        {           
            ViewBag.Id = id;
            ViewBag.Type = type;
            return View("ticketprint");
           
        }


        public FileStreamResult pathprint(Guid id, TypeDocumentInvoice type)
        {        
            if (type== TypeDocumentInvoice.Invoice|| type == TypeDocumentInvoice.Ticket)
            {
                var invoice = _invoiceManager.Get(id);

                Clients.Client client = null;

                var payments = InfoPaymentInvoice(invoice.Id);
                var listInfoPago = AddPaymentsMethods2(payments);

                var drawer = _drawerRepository.GetAll().Where(x => x.TenantId == invoice.TenantId && x.IsOpen == true).FirstOrDefault();
                var branchOffice = infoBranchOffice(drawer.BranchOfficeId);
                var BranchOfficeInfo = AddBranchOfficeInfo(branchOffice);

                if (invoice.ClientId != null)
                    client = _invoiceService.GetClientPdf(invoice.Client.Id);

                var tenant = _tenantManager.Get(invoice.TenantId);

                _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);
                GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
                Stream pdfStream = generatePDF.CreatePDFAsStream(invoice, client, tenant, listInfoPago, BranchOfficeInfo);

                return File(pdfStream, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            else
            {               
               
                var note = _invoiceService.GetNote(id);
                var tenant = _tenantManager.Get(note.TenantId);           

               
                _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Nota, tenant.ComercialName);
                GeneratePDF generatePDF = new GeneratePDF(_notaReportSettings);
                Stream pdfStream = generatePDF.CreatePDFNoteAsStream(note.Invoice, null, tenant, note, null);

                return File(pdfStream, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            
            

        }
    
        //public ActionResult printPos(Guid id, string Sid)
        //{
        //    try
        //    {
        //        var note = _invoiceService.GetNote(id);
        //        var document = string.Empty;
        //        if ((note.Invoice.Tenant.IsPos) && (note != null))
        //        {
        //            var printer = new Printer((PrinterTypes)note.Invoice.Tenant.PrinterType);
        //            DocumentPrint docPrint = new DocumentPrint(note);
        //            HttpApplicationStateBase app = HttpContext.Application;

        //            document = printer.print(docPrint);
        //            app = printer.ClientPrinterSettings(app, Sid, document);

        //            //return JavaScript("jsWebClientPrint.print('id=" + id + "&tenantId=" + note.TenantId + "&tipoDocumento=" + note.Invoice.TypeDocument + "');");


        //        }
        //        return Json(new { codeError = ErrorCodeHelper.Ok, Error = string.Empty });
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { codeError = ErrorCodeHelper.Error, Error = ex.Message});
        //    }
            
            
        //}


        public FileStreamResult pathprintNote(Guid id)
        {
            var invoice = _invoiceManager.Get(id);

            Clients.Client client = null;


            if (invoice.ClientId != null)
                client = _invoiceService.GetClientPdf(invoice.Client.Id);

            var tenant = _tenantManager.Get(invoice.TenantId);

            //// string archivoTemp = Path.GetTempFileName();

            var payments = InfoPaymentInvoice(invoice.Id);
            var listInfoPago = AddPaymentsMethods2(payments);

            var drawer = _drawerRepository.GetAll().Where(x => x.TenantId == invoice.TenantId && x.IsOpen == true).FirstOrDefault();
            var branchOffice = infoBranchOffice(drawer.BranchOfficeId);
            var BranchOfficeInfo = AddBranchOfficeInfo(branchOffice);

            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);
            GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
            Stream pdfStream = generatePDF.CreatePDFAsStream(invoice, client, tenant, listInfoPago, BranchOfficeInfo);
            //, "ticket_" + invoice.VoucherKey + ".pdf"
            return File(pdfStream, System.Net.Mime.MediaTypeNames.Application.Pdf);
            //var ruta = Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf");

            //return Content(ruta);

        }
    }


}
