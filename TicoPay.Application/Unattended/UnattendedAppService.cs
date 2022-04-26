using Abp.Application.Services;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using Abp.UI;
using IdentityModel.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Helpers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TicoPay.Address;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Core.Common;
using TicoPay.General;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using TicoPay.ReportsSettings;
using TicoPay.Unattended.Dto;
using static TicoPay.Invoices.UnattendedApi;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Unattended
{
    public class UnattendedAppService : IUnattendedAppService, ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly ITenantAppService _tenantAppService;
        private readonly IRepository<Register, Guid> _registerRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<UnattendedApi, Guid> _unattendedRepository;
        private readonly IReportSettingsAppService _reportSettingsAppService;
        private readonly IRepository<Distrito, int> _distritoRepository;
        private readonly IRepository<Barrio, int> _barrioRepository;
        private ReportSettings _facturaReportSettings;

        public UnattendedAppService(
            TenantManager tenantManager,
            IRepository<Register, Guid> registerRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IInvoiceAppService invoiceAppService,
            IInvoiceManager invoiceManager,
            IRepository<Invoice, Guid> invoiceRepository,
            ITenantAppService tenantAppService,
            IRepository<UnattendedApi, Guid> unattendedRepository,
            IReportSettingsAppService reportSettingsAppService,
            IRepository<Distrito, int> distritoRepository,
            IRepository<Barrio, int> barrioRepository)
        {
            _tenantManager = tenantManager;
            _registerRepository = registerRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _invoiceAppService = invoiceAppService;
            _invoiceManager = invoiceManager;
            _invoiceRepository = invoiceRepository;
            _tenantAppService = tenantAppService;
            _unattendedRepository = unattendedRepository;
            _distritoRepository = distritoRepository;
            _barrioRepository = barrioRepository;
            _reportSettingsAppService = reportSettingsAppService;
        }

        public Uri SaveAzureStorage(string documentname, string ruta, string _container)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(_container);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(documentname);

            if (File.Exists(ruta))
            {
                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fileStream = System.IO.File.OpenRead(@ruta))
                {
                    blockBlob.UploadFromStream(fileStream);
                }
            }

            return blockBlob.Uri;
        }

        private static Stream LoadFileFromAzureStorage(string containerName, string fileName, string path = "")
        {
            Stream fileContents = new MemoryStream();
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
                if (blobContainer.Exists())
                {
                    var docName = path + fileName.Substring(fileName.LastIndexOf("/") + 1);
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

        [UnitOfWork(isTransactional: false)]
        [DisableValidation]
        public void SyncsUnattendedWithTaxAdministration(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            var pendingUnattended = _unattendedRepository
                .GetAll()
                .Where(i => i.SendInvoice && !i.IsDeleted && i.TenantId == tenant.Id
                && (i.StatusTribunet == StatusTaxAdministration.NoEnviada || i.StatusTribunet == StatusTaxAdministration.Recibido || i.StatusTribunet == StatusTaxAdministration.Procesando)
                || (i.TenantId == tenant.Id && (i.StatusTribunet == StatusTaxAdministration.Aceptado || i.StatusTribunet == StatusTaxAdministration.Rechazado) && i.CreationTime.Month >= 4 && i.CreationTime.Year >= 2018))
                .Include(i => i.Tenant)
                .ToList();
            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
                if (pendingUnattended != null && pendingUnattended.Count > 0)
                {
                    ValidateTribunet validateTribunet = new ValidateTribunet();
                    TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet, pss).Result;
                    if (tokenResponse != null && !tokenResponse.IsError)
                    {
                        foreach (var unattended in pendingUnattended)
                        {
                            var currentUnattended = _unattendedRepository.Get(unattended.Id);
                            ElectronicBillResponse response = null;
                            if (unattended.StatusTribunet != StatusTaxAdministration.Aceptado && unattended.StatusTribunet != StatusTaxAdministration.Rechazado && unattended.StatusTribunet != StatusTaxAdministration.Error)
                            {
                                response = validateTribunet.GetComprobanteStatusFromTaxAdministration(currentUnattended, tokenResponse.AccessToken);
                                if (response != null)
                                {
                                    switch (response.IndEstado.ToLower())
                                    {
                                        case "aceptado":
                                            currentUnattended.StatusTribunet = StatusTaxAdministration.Aceptado;
                                            break;
                                        case "rechazado":
                                            currentUnattended.StatusTribunet = StatusTaxAdministration.Rechazado;
                                            break;
                                        case "error":
                                            currentUnattended.StatusTribunet = StatusTaxAdministration.Error;
                                            break;
                                    }
                                }
                                currentUnattended.MessageTaxAdministration = response.RespuestaXml;

                                var result = _unattendedRepository.Update(currentUnattended);
                                SaveUnattendedResponse(currentUnattended.VoucherKey, currentUnattended.MessageTaxAdministration);
                            }
                        _unitOfWorkManager.Current.SaveChanges();
                        }
                    }
                }
                unitOfWork.Complete();
            }
        }
        public void SaveUnattendedResponse(string voucherKey, string repuestaXml)
        {
            var encodedTextBytes = Convert.FromBase64String(repuestaXml);
            string plainText = Encoding.UTF8.GetString(encodedTextBytes);
            string xmlPath = SaveXmlToFile("Respuesta_" + voucherKey + ".xml", plainText);
        }

        private string SaveXmlToFile(string fileName, string xml)
        {
            string path = Path.Combine(WorkPaths.GetXmlSignedClientPath(), fileName);
            XmlDocument document = new XmlDocument() { PreserveWhitespace = true };
            document.LoadXml(xml);
            document.Save(path);
            return path;
        }

        public UnattendedStatusToTribunet UnattendedWithTaxAdministration(string voucherKey, Tenant tenant)
        {
            var query = _unattendedRepository.GetAll();

            if (voucherKey != null)
            {
                query = query.Where(u => u.VoucherKey == voucherKey);
                query = query.Where(u => u.TenantId == tenant.Id);
            }

            var result = query.ToList().FirstOrDefault();

            if (result == null)
            {
                throw new UserFriendlyException("VoucherKey no existe!!");
            }

            if (result.MessageTaxAdministration!=null)
            {
                var encodedTextBytes = Convert.FromBase64String(result.MessageTaxAdministration);
                string xmlDocument = Encoding.UTF8.GetString(encodedTextBytes);

                return new UnattendedStatusToTribunet
                {
                    StatusTribunet = result.StatusTribunet.ToString(),
                    XmlContent = Convert.ToBase64String(encodedTextBytes)
                };
            }
            else
            {
                return new UnattendedStatusToTribunet
                {
                    StatusTribunet = StatusTaxAdministration.Procesando.ToString(),
                    XmlContent = null
                };
            }
        }

        public UnattendedNotification SendXmlTribunet(bool useConsecutive, Tenant tenant, string xmlContent, DateTime dueDate)
        {
            UnattendedNotification notificacion = new UnattendedNotification();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlContent);
            }
            catch (Exception)
            {
                throw new UserFriendlyException("No es un Formato XML Válido");
            }
            
            ValidateTribunet validateHacienda = new ValidateTribunet();

            UnattendedApi @unattendedApi = UnattendedApi.Create(tenant.Id, dueDate);

            @unattendedApi.ValidarDueDate(dueDate);

            Certificate certified = null;

            UnattendedApiDto @unattended = null;

            TipoDocumento tipoDocumento = new TipoDocumento();

            string path = null;
            string pathXml = null;

            //obtiene el certificado si se valida con hacienda
            if (tenant.ValidateHacienda)
                certified = _tenantAppService.GetCertified(tenant.Id);
           
            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    if (useConsecutive)
                    {
                        tipoDocumento = @unattendedApi.DeterminarTipoDocumento(xmlDoc);
                        @unattendedApi.UnattendedConsecutivo(xmlDoc);
                        @unattendedApi.TenantId = tenant.Id;
                        var allRegisters = _invoiceManager.GetallRegisters(tenant.Id);

                        @unattendedApi.SetInvoiceConsecutivo(allRegisters, tenant, tipoDocumento);

                        if (@unattendedApi.DueDate.ToString("dd-MM-yyyy") == DateTimeZone.Now().ToString("dd-MM-yyyy"))
                            @unattendedApi.SetInvoiceNumberKey(@unattendedApi.DueDate, @unattendedApi.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);
                        else
                            @unattendedApi.SetInvoiceNumberKey(@unattendedApi.DueDate, @unattendedApi.ConsecutiveNumber, tenant, (int)VoucherSituation.withoutInternet);
                        @unattendedApi.AddAtribbuteNodoXML(xmlDoc, @unattendedApi, tipoDocumento);

                    }
                    else
                    {
                        @unattendedApi.CheckNodo(xmlDoc, @unattendedApi, tenant);
                    }

                    @unattendedApi.CreateXML(tenant, certified, @unattendedApi.VoucherKey, xmlDoc);

                    path = Path.Combine(WorkPaths.GetXmlSignedClientPath(), @unattendedApi.VoucherKey + ".xml");

                    if ((tenant.ValidateHacienda) && (tenant.TipoFirma == FirmType.Llave))
                    {                        
                        string validate = validateHacienda.SendResponseTribunet("POST", tenant, @unattendedApi.VoucherKey, dueDate, path);
                        if (validate == "-1")
                        {
                            SetInvoiceWithInternet(ref @unattendedApi);
                            @unattendedApi.ChangeVoucherKit(path, @unattendedApi.VoucherKey, tenant, certified);
                            path = null;
                            notificacion.Estado = StatusDocumentUnattended.Rechazado;
                            throw new UserFriendlyException("Documento no fue aceptado por hacienda");
                        }
                        else
                        {
                            @unattendedApi.SendInvoice = true;
                            @unattendedApi.StatusTribunet = StatusTaxAdministration.Procesando;
                            Uri XML = SaveAzureStorage(@unattendedApi.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedClientPath(), @unattendedApi.VoucherKey + ".xml"), "xmlinvoice");
                            @unattendedApi.SetInvoiceXML(XML);
                            _unattendedRepository.Insert(@unattendedApi);
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("ValidateHacienda","Debe tener activada y configurada la validación con Hacienda en el menu de compañía");
                    }
                    unitOfWork.Complete();
                }

                var xml = LoadFileFromAzureStorage("xmlinvoice", @unattendedApi.ElectronicBill);

                StreamReader reader = new StreamReader(xml);
                string docXml = reader.ReadToEnd();

                pathXml = Path.Combine(WorkPaths.GetXmlSignedClientPath(), @unattendedApi.VoucherKey + ".xml");
                Invoice @invoice = Invoice.Create(tenant.Id, dueDate, tenant);
                Client @client = new Client();

                List<FacturaElectronicaMedioPago> listMedioPago = new List<FacturaElectronicaMedioPago>();

                unattendedApi.ArmarEstructuraPDF(docXml, tenant, @invoice, @client, listMedioPago);

                if ((client.Name != null || client.NameComercial != null ) && (tipoDocumento == TipoDocumento.FacturaElectronica) && @client.Barrio != null)
                    UnattendedClientUbication(@client, @unattendedApi);

                @invoice.CreatePDF(@invoice, @client, tenant, listMedioPago, null, null, _facturaReportSettings);

                string pathPdf = Path.Combine(WorkPaths.GetPdfPath(), @unattendedApi.VoucherKey + ".pdf");

                @unattended = new UnattendedApiDto
                {
                    DueDate = @unattendedApi.DueDate,
                    SendInvoice = @unattendedApi.SendInvoice,
                    XmlContent = FileByte(pathXml),
                    PDF = FileByte(pathPdf)
                };

                @unattendedApi.DeleteFile(pathPdf);
                @unattendedApi.DeleteFile(path);
            }
            catch(ArgumentNullException)
            {
                throw new UserFriendlyException("Debe tener activada la validación con Hacienda en el menu de compañía y debe tener configurado el certificado y las credenciales");
            }
            catch (UserFriendlyException ex)
            {
               string xml = VerificarPath(path, pathXml, xmlContent);
                ArmarNotificacion(notificacion, tenant, tipoDocumento, @unattendedApi, ex.Message,xml);
            }

            if (notificacion == null)
            {
                string xml = FileByte(pathXml);
                ArmarNotificacion(notificacion, tenant, tipoDocumento, @unattendedApi, "Documento se aceptó correctamente por la plataforma y fue enviado a hacienda",xml);
            }
           
            notificacion.UnattendedApiDto = @unattended;

            return notificacion;
        }
        
        public string VerificarPath(string path, string pathXml, string xmlContent)
        {
            string result = null;
            if (path != null)
            {
                result = FileByte(path);
            } else if (pathXml != null)
            {
                result = FileByte(pathXml);
            }
            else
            {
                byte[] bytes = Encoding.ASCII.GetBytes(xmlContent);
                result = bytes.ToString();
            }
            return result;
        }

        public string FileByte(string path)
        {
            byte[] pdfBytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(pdfBytes); 
            
        }

        private void SetInvoiceWithInternet(ref UnattendedApi unattendedApi)
        {
            unattendedApi.StatusVoucher = VoucherSituation.withoutInternet;
            int _vouchersituation = (int)VoucherSituation.withoutInternet;
            unattendedApi.VoucherKey = unattendedApi.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + unattendedApi.VoucherKey.Substring(42, 8);
            unattendedApi.SendInvoice = false;
        }

        public UnattendedStatusToTribunet ObtenerPDF(string voucherKey, Tenant tenant)
        {
            var query = _unattendedRepository.GetAll();
            string containerName = "xmlinvoice";

            if (voucherKey != null)
            {
                query = query.Where(u => u.VoucherKey == voucherKey);
                query = query.Where(u => u.TenantId == tenant.Id);
            }

            UnattendedApi @unattendedApi = query.ToList().FirstOrDefault();

            if (@unattendedApi == null)
            {
                throw new UserFriendlyException("VoucherKey no existe!!");
            }

            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);


            var xml = LoadFileFromAzureStorage(containerName, @unattendedApi.ElectronicBill);

            StreamReader reader = new StreamReader(xml);
            string text = reader.ReadToEnd();

            if (text == null)
            {
                throw new UserFriendlyException("Documento XML no existe!!");
            }

            Invoice @invoice = Invoice.Create(tenant.Id, DateTimeZone.Now(), tenant);
            Client @client = new Client();
            List<FacturaElectronicaMedioPago> listMedioPago = new List<FacturaElectronicaMedioPago>();

            unattendedApi.ArmarEstructuraPDF(text, tenant, @invoice, @client, listMedioPago);

            if ((client != null) && (@invoice.TypeDocument == TypeDocumentInvoice.Invoice))
                UnattendedClientUbication(@client, @unattendedApi);

            @invoice.CreatePDF(@invoice, @client, tenant, listMedioPago, null, null, _facturaReportSettings);

            string pdfPath = Path.Combine(WorkPaths.GetPdfPath(), voucherKey + ".pdf");

            string pdf = FileByte(pdfPath);
            @unattendedApi.DeleteFile(pdfPath);

            return new UnattendedStatusToTribunet
            {
                StatusTribunet = @unattendedApi.StatusTribunet.ToString(),
                XmlContent = pdf
            };
        }
        private void ArmarNotificacion(UnattendedNotification notificacion, Tenant tenant, TipoDocumento tipoDocumento, UnattendedApi @unattendedApi, string mensaje, string pathXml)
        {
            notificacion.Emisor = tenant.Name;
            notificacion.TipoDocumento = tipoDocumento;
            notificacion.Estado = @unattendedApi.SendInvoice == true ? StatusDocumentUnattended.Enviado : StatusDocumentUnattended.Rechazado;
            notificacion.MensajeRespuesta = mensaje;
            notificacion.Clave = @unattendedApi.VoucherKey == null ? "0" : @unattendedApi.VoucherKey;
            notificacion.ConsecutivoXML = @unattendedApi.ConsecutiveNumber == null ? "0" : @unattendedApi.ConsecutiveNumber;

        }

        private void UnattendedClientUbication(Client @client, UnattendedApi @unattendedApi)
        {
            Barrio barrio = _barrioRepository.Get(@client.Barrio.Id);
            Distrito distrito = _distritoRepository.Get(@client.Barrio.DistritoID);
            @unattendedApi.AssignBarrioDistrict(barrio, distrito, @client);
        }

    }
}
