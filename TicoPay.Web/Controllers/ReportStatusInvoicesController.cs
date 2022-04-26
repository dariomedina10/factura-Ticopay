using Abp.Domain.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using TicoPay.Application.Helpers;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.ReportsSettings;
using TicoPay.ReportStatusInvoices;
using TicoPay.ReportStatusInvoices.Dto;
using TicoPay.Web.Infrastructure;
using TicoPay.Drawers;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class ReportStatusInvoicesController : TicoPayControllerBase
    {
        private readonly IReportStatusInvoicesAppService _reportStatusInvoicesAppService;
        private readonly IReportStatusInvoicesAppService _reportStatusNoteAppService;
        private readonly IInvoiceAppService _InvoiceAppService;
        private readonly IClientManager _clientManager;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Note, Guid> _noteRepository;
        private readonly IReportSettingsAppService _reportSettingsAppService;
        private readonly TenantManager _tenantManager;
        private ReportSettings _facturaReportSettings;
        IDrawersAppService _drawersAppService;

		public ReportStatusInvoicesController(IRepository<Invoice, Guid> invoiceRepository, TenantManager tenantManager, IReportSettingsAppService reportSettingsAppService, IInvoiceAppService InvoiceAppService, IReportStatusInvoicesAppService reportStatusInvoicesAppService, IClientManager clientManager, IRepository<Note, Guid> noteRepository, IDrawersAppService drawersAppService)        {
            _reportStatusInvoicesAppService = reportStatusInvoicesAppService;
            _reportStatusNoteAppService = reportStatusInvoicesAppService;
            _clientManager = clientManager;
            _InvoiceAppService = InvoiceAppService;
            _invoiceRepository = invoiceRepository;
            _noteRepository = noteRepository;
            _tenantManager = tenantManager;
            _reportSettingsAppService = reportSettingsAppService;
            _drawersAppService = drawersAppService;
        }
        // GET: ReportStatusInvoices
        public ActionResult Index()
        {
            ReportStatusInvoicesInputDto<ReportStatusInvoicesDto> model = new ReportStatusInvoicesInputDto<ReportStatusInvoicesDto>();
            try
            {
                model.Query = "";
                DateTime tempDate = DateTime.Now;
              
                model.InitialDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 00, 00, 00);
                model.FinalDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 00);
                model.listStatusRecepcion = EnumHelper.GetSelectList(typeof(StatusReception));
                model.listStatusTaxAdmin= EnumHelper.GetSelectList(typeof(StatusTaxAdministration));
                model.InvoicesList = _reportStatusInvoicesAppService.SearchReportStatusInvoices(model);
                model.Groups = _clientManager.GetAllListGroups();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.Control = "ReportStatusInvoices";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Facturas";
            }
            return View(model);
        }


        [HttpPost]
        public ViewResultBase Search(ReportStatusInvoicesInputDto<ReportStatusInvoicesDto> model)
        {
            try
            {
                DateTime tempInitial;
                DateTime tempFinal;
                if (model.InitialDate != null)
                {
                    tempInitial = model.InitialDate.Value;
                    model.InitialDate = new DateTime(tempInitial.Year, tempInitial.Month, tempInitial.Day, 00, 00, 00);
                }

                if (model.FinalDate != null)
                {
                    tempFinal = model.FinalDate.Value;
                    model.FinalDate = new DateTime(tempFinal.Year, tempFinal.Month, tempFinal.Day, 23, 59, 59);
                }
                model.listStatusRecepcion = EnumHelper.GetSelectList(typeof(StatusReception));
                model.listStatusTaxAdmin = EnumHelper.GetSelectList(typeof(StatusTaxAdministration));
                var entities = _reportStatusInvoicesAppService.SearchReportStatusInvoices(model);               
                model.InvoicesList = entities;
                model.Groups = _clientManager.GetAllListGroups();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(model.BranchOfficeId);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Facturas";
            }
            return View("Index", model);
        }

      
        public ActionResult VerRespuestaXML(Guid id, TypeDocumentInvoice type)
        {
           
            ReportStatusInvoicesInputDto<ReportStatusInvoicesDto> model = new ReportStatusInvoicesInputDto<ReportStatusInvoicesDto>
            {
                Id = id,
                TypeDocumentinvoice= type
            };

            var entities = _reportStatusInvoicesAppService.SearchReportStatusInvoices(model);
            string messageTaxAdministration = entities.FirstOrDefault().MessageTaxAdministration;
            var encodedTextBytes = Convert.FromBase64String(messageTaxAdministration);
            string xml = Encoding.UTF8.GetString(encodedTextBytes);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            MemoryStream xmlStream = new MemoryStream();
            xmlDoc.Save(xmlStream);

            xmlStream.Position = 0;

            string invoiceXml;
            using (StreamReader reader = new StreamReader(xmlStream, Encoding.UTF8))
            {
                invoiceXml = reader.ReadToEnd();
            }

            var contentStreamResult = new ContentResult()
            {
                Content = invoiceXml,
                ContentType = "text/xml"
            };
       
            return contentStreamResult;
        }

        public ActionResult DescargarRespuestaXml(Guid id, TypeDocumentInvoice type)
        {
            ReportStatusInvoicesInputDto<ReportStatusInvoicesDto> model = new ReportStatusInvoicesInputDto<ReportStatusInvoicesDto>
            {
                Id = id,
                TypeDocumentinvoice = type
            };

           
            var entities = _reportStatusInvoicesAppService.SearchReportStatusInvoices(model);
            string messageTaxAdministration = entities.FirstOrDefault().MessageTaxAdministration;
            string fileName = entities.FirstOrDefault().ConsecutiveNumber + ".xml";
            var encodedTextBytes = Convert.FromBase64String(messageTaxAdministration);
            string xml = Encoding.UTF8.GetString(encodedTextBytes);

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(xml);

            MemoryStream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            //xmlDoc.Save(xmlStream);

            var fileStreamResult = new FileStreamResult(xmlStream, "text/xml")
            {
                FileDownloadName = fileName
            };
            return fileStreamResult;
           
            
        }

        public ActionResult DescargarRespuestaXmlNote(string id)
        {

            var note = _noteRepository.GetAll().Where(a => a.ConsecutiveNumber == id).FirstOrDefault();
            Guid idNote = new Guid(Convert.ToString(note.Id));
            string fileName = id + ".xml";


            string xml = "";

            if (note.MessageTaxAdministration != null)
            {
                string messageTaxAdministration = note.MessageTaxAdministration;
                var encodedTextBytes = Convert.FromBase64String(messageTaxAdministration);
                xml = Encoding.UTF8.GetString(encodedTextBytes);
            }
           
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            MemoryStream xmlStream = new MemoryStream();
            xmlDoc.Save(xmlStream);

            xmlStream.Position = 0;

            var fileStreamResult = new FileStreamResult(xmlStream, "text/xml")
            {
                FileDownloadName = fileName
            };
            return fileStreamResult;


        }


        public ActionResult DownloadNote(string id)
        {
            var note = _noteRepository.GetAll().Where(a => a.ConsecutiveNumber == id).FirstOrDefault();
            Guid idNote = new Guid(Convert.ToString(note.Id));
            string containerName = "xmlnote";
            string fileName = id + ".xml";

            var xml = LoadFileFromAzureStorage(containerName, note.ElectronicBill);

            xml.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(xml);

           
            string text = reader.ReadToEnd();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(text);

            MemoryStream xmlStream = new MemoryStream();
            xmlDoc.Save(xmlStream);

            xmlStream.Position = 0;

            var fileStreamResult = new FileStreamResult(xmlStream, "text/xml")
            {
                FileDownloadName = fileName
            };

            return fileStreamResult;
        }

        public ActionResult DownloadInvoice(Guid id)
        {
            var invoice = _invoiceRepository.GetAll().Where(a => a.Id == id).FirstOrDefault();
            Guid idInvoice = new Guid(Convert.ToString(invoice.Id));
            string containerName = "xmlinvoice"; 
            string fileName = invoice.VoucherKey + ".xml";

            var xml = LoadFileFromAzureStorage(containerName, invoice.ElectronicBill);
            
            StreamReader reader = new StreamReader(xml);
            string text = reader.ReadToEnd();

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(text);

            //MemoryStream xmlStream = new MemoryStream();
            //xmlDoc.Save(xmlStream);

            //xmlStream.Position = 0;

            MemoryStream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var fileStreamResult = new FileStreamResult(xmlStream, "text/xml")
            {
                FileDownloadName = fileName
            };

           return fileStreamResult;
        }

        private static Stream LoadFileFromAzureStorage(string containerName, string fileName)
        {
            Stream fileContents = new MemoryStream();
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
                if (blobContainer.Exists())
                {
                    var docName = fileName.Substring(fileName.LastIndexOf("/") + 1);
                    CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(docName);
                    blockBlob.DownloadToStream(fileContents);
                    fileContents.Position = 0;
                }
            }
            catch (Exception ex)
            {
                
            }
            return fileContents;
        }
    }
}