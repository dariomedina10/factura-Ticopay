using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using AutoMapper;
using PagedList;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Invoices.Dto;
using TicoPay.Services;
using System.IO;
using TicoPay.MultiTenancy;
using TicoPay.General;
using SendMail;
using TicoPay.Invoices.BN;
using Abp.Dependency;
using TicoPay.Users;
using TicoPay.Core.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Threading.Tasks;
using Abp.Application.Features;
using TicoPay.Editions;
using Abp.Notifications;
using Abp.Localization;
using System.Text;
using IdentityModel.Client;
using System.Xml;
using System.Web;
using System.Web.Hosting;
using TicoPay.Common;
using TicoPay.Invoices.XSD;
using Abp.Runtime.Validation;
using System.Globalization;
using TicoPay.Core;
using TicoPay.ReportsSettings;
using TicoPay.Taxes;
using static TicoPay.MultiTenancy.Tenant;
using LinqKit;
using System.Transactions;
using TicoPay.Inventory;
using System.Xml.Serialization;
using FirmaXadesNet;
using FirmaXadesNet.Signature.Parameters;
using System.Security.Cryptography.X509Certificates;
using FirmaXadesNet.Crypto;
using System.Xml.Linq;
using TicoPay.Drawers;
using TicoPay.BranchOffices;
using TicoPay.BranchOffices.Dto;
using BCR;
using Abp.BackgroundJobs;
using TicoPay.BackgroundJobs;

namespace TicoPay.Invoices
{
    [DisableValidation]
    public class InvoiceAppService : ApplicationService, IInvoiceAppService, ITransientDependency
    {
        public const int MaxTitleLength = 160;
        public const int MaxReasonContingency = 180;
        //These members set in constructor using constructor injection.
        private readonly IRepository<PaymentInvoice, Guid> _paymentinvoiceRepository;
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<PaymentNote, Guid> _paymentnoteRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Tax, Guid> _taxRepository;
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Register, Guid> _registerRepository;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IUserAppService _userAppService;
        private readonly TenantManager _tenantManager;
        private readonly IServiceManager _serviceManager;
        private readonly IGroupConceptsManager _groupConceptsManager;
        private readonly IClientManager _clientManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Note, Guid> _noteRepository;
        private readonly EditionManager _editionManager;
        private readonly INotificationPublisher _notiticationPublisher;
        private readonly ITenantAppService _tenantAppService;
        private readonly IReportSettingsAppService _reportSettingsAppService;
        private readonly IRepository<ClientService, Guid> _clientServiceRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Bank, Guid> _bankRepository;
        private ReportSettings _facturaReportSettings;
        private ReportSettings _notaReportSettings;
        private readonly UserManager _userManager;
        private readonly IDrawersAppService _drawersAppService;
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        private readonly IRepository<BranchOffice, Guid> _branchOfficeRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        SendMailTP mail = new SendMailTP(); // clase para envio de correo
        RateOfDay _rateOfDay = new RateOfDay();

        public const decimal rango = (decimal)0.01;
        // definicion de constantes para envio de correo
        public const string subject = "Nueva Factura Electrónica";
        public const string subjectTicket = "Nuevo Tiquete Electrónico";
        public const string emailbody = "<p>Adjunto al correo encontrará su {0} en formato PDF y XML.Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su {1}.</p>";
        public const string emailbodySignature = @"<p>La Factura Nº {0} ha sido generada de forma exitosa, por favor complete los siguientes pasos para realizar 
                                                    su firma digital y el envío a hacienda: </p> <br/>
                                                    {1}
                                                    <p>Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su factura.</p>";
        public const string emailbodyNoteSignature = @"<p>La Nota de {0} Nº {1} ha sido generada de forma exitosa, por favor complete los siguientes pasos para realizar 
                                                    su firma digital y el envío a hacienda: </p> <br/>
                                                    {2}
                                                    <p>Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su nota.</p>";
        public const string emailbodyRecurren = @"<p>Sus facturas por {0} recurrentes han sido generadas de forma exitosa, por favor complete los siguientes pasos para realizar 
                                                    su firma digital y el envío a hacienda: </p> <br/>
                                                    {1}
                                                    <p>Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su factura.</p>";
        //public const string emailSteps = @"<p><h3>Paso 1:</h3>  Descargue e instalar la aplicación Firma TicoPay que se encuentra en el siguiente enlace : 
        //                                                <div style='text-align:center;height:40px;width:250px;background-color:#36733C;margin-top:10px;-webkit-border-radius:5px;-moz-border-radius:5px;border-radius:5px;color:#ffffff;display:block;'>
        //                                                <a href='#' style='font-size:16px;font-weight:bold;text-decoration:none;line-height:40px;width:100%;display:inline-block'><span style='color:#FFFFFF'>Descargar</span></a>
        //                                                </div>
        //                                    </p>
        //                                   <p><h3>Paso 2:</h3>Firme digitalmente sus facturas siguiendo las instrucciones del manual adjunto.</p>
        //                                   <p><h3>Paso 3:</h3>Al realizar la firma, se enviará por correo de forma automática la factura electrónica al cliente en formato PDF y XML, asi como su envío a hacienda. </p>
        //                                   <br/>";
        public const string emailSteps = @"<p><h3>Paso 1:</h3>Descargue e instale la aplicación Firma TicoPay. 
                                            </p>
                                           <p><h3>Paso 2:</h3>Firme digitalmente sus facturas, notas o tiquetes siguiendo las instrucciones del manual provisto.</p>
                                           <p><h3>Paso 3:</h3>Al realizar la firma, se enviará por correo de forma automática el comprobante electrónico al cliente en formato PDF y XML, asi como su envío a hacienda. </p>
                                           <br/>";
        public const string emailfooter = "<p>Por favor enviar soporte de pago a pagos@ticopays.com, si tiene algún incidente puede reportarlo a soporte@ticopays.com</p> <p>Gracias</p> <p><b>Equipo TicoPay</b></p>";
        public const string SmsFacturaACobroMessage = "Tiene una factura a cobro Nro.: {0} por {1} {2}. www.TicoPay.com";
        public const string SmsNoteMessage = "Se ha generado una nota de {0} por {1} {2}. www.TicoPay.com";
        public const string SmsInvoiceLineTitle = "Servicio de SMS de Cobros";
        public const string emailCuentaTicopay = @"<br/><p>Efectuar pagos en Colones (&#8353;) o en Dólares ($),según tipo de cambio de venta del día del Banco Nacional de Costa Rica.</p>
                                                    
                                                    <p><ins><strong>CANALES DEL BANCO NACIONAL:</strong></ins></p>
                                                    <p>Para su comodidad y seguridad puede Pagar sus facturas a través de todos los canales del Banco Nacional: BN Internet Banking, BN Servicios, BN Corresponsales, PAR o Cajas.</p>
	                                                <p>Desde la opción <strong>“Pagos”</strong> selecciona la opción <strong>“Facturación”</strong>, luego <strong>“Ticopay”</strong>. Para consultar sus facturas pendientes digite el <strong>Código del Cliente</strong>, que aparece en su Factura Electrónica, en el espacio “Valor a consultar”.</p>
                                                    
                                                    <p><ins><strong>DEPÓSITOS O TRANSFERENCIAS</ins></strong>
                                                    <p><strong>RAZON SOCIAL : </strong> HNG CARMENTA GLOBAL GROUP SOCIEDAD ANONIMA</p>
	                                                <p><strong>Cédula Jurídica : </strong> 3-101-741788</p>
	                                                
	                                                <table>
		                                                <tr>
                                                            <td style='width:350px'><strong>Cuenta en Colones (₡) – Banco Nacional</strong></td>
		                                                </tr>
                                                        <tr>
			                                                <td style='width:350px'>Cuenta Banco Nacional</td>
			                                                <td style='width:250px'><strong>200-01-111-015692-4</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Nacional</td>
                                                            <td style='width:250px'><strong>15111120010156920</strong></td>
		                                                </tr>	
                                                        <tr>
                                                            <td style='width:350px'><strong>Cuenta en Colones (₡) – Banco Costa Rica</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>001-0463071-8</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>15201001046307189</strong></td>
		                                                </tr>	
	                                                </table><br/>
	                                                
	                                                <table>
		                                                <tr>
                                                            <td style='width:350px'><strong>Cuenta en Dólares ($) – Banco Nacional</strong></td>
		                                                </tr>
                                                        <tr>
			                                                <td style='width:350px'>Cuenta Banco Nacional</td>
			                                                <td style='width:250px'><strong>200-02-111-007852-0</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Nacional</td>
                                                            <td style='width:250px'><strong>15111120020078523</strong></td>
		                                                </tr>
		                                                <tr>
                                                            <td style='width:350px'><strong>Cuenta en Dólares ($) – Banco Costa Rica</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>001-0463074-2</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>15201001046307427</strong></td>
		                                                </tr>
	                                                </table><br/>";

        public const string firstTicopayInvoiceMessage = @"<br/><p><b>Estimado cliente se le recuerda que dispone de tres (03) días para pagar esta factura, de lo contrario su cuenta será desactivada hasta recibir el pago correspondiente.</b></p>";

        // usuario que aplica los pagos en el Banco
        public const string userPayBn = "admin";
        public const string cuentaBanca = "";

        public const string CRON_ANNUAL = "0 0 12 1 {0}/12 ? *";

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>

        public InvoiceAppService(IRepository<BranchOffice, Guid> branchOfficeRepository, IRepository<Drawer, Guid> drawerRepository, IRepository<Bank, Guid> bankRepository, IRepository<Product, Guid> productRepository, IRepository<Invoice, Guid> invoiceRepository, IInvoiceManager invoiceManager,
            IServiceManager serviceManager, IRepository<Service, Guid> serviceRepository, IUnitOfWorkManager unitOfWorkManager,
            IRepository<Client, Guid> clientRepository, IRepository<Register, Guid> registerRepository, TenantManager tenantManager,
            IClientManager clientManager, IRepository<Note, Guid> noteRepository, IRepository<PaymentInvoice, Guid> paymentinvoiceRepository, IRepository<Payment, Guid> paymentRepository,
            IUserAppService userAppService, EditionManager editionManager, INotificationPublisher notiticationPublisher,
            ITenantAppService tenantAppService, IReportSettingsAppService reportSettingsAppService, IRepository<Tax, Guid> taxRepository,
            IGroupConceptsManager groupConceptsManager, UserManager userManager, IRepository<ClientService, Guid> clientServiceRepository, IRepository<PaymentNote, Guid> paymentnoteRepository, IDrawersAppService drawersAppService, IBackgroundJobManager backgroundJobManager)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _serviceManager = serviceManager;
            _serviceRepository = serviceRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _clientRepository = clientRepository;
            _registerRepository = registerRepository;
            _tenantManager = tenantManager;
            _clientManager = clientManager;
            _noteRepository = noteRepository;
            _paymentinvoiceRepository = paymentinvoiceRepository;
            _paymentRepository = paymentRepository;
            _userAppService = userAppService;
            _editionManager = editionManager;
            _notiticationPublisher = notiticationPublisher;
            _tenantAppService = tenantAppService;
            _reportSettingsAppService = reportSettingsAppService;
            LocalizationSourceName = TicoPayConsts.LocalizationSourceName;
            _taxRepository = taxRepository;
            _groupConceptsManager = groupConceptsManager;
            _userManager = userManager;
            _clientServiceRepository = clientServiceRepository;
            _paymentnoteRepository = paymentnoteRepository;
            _productRepository = productRepository;
            _drawersAppService = drawersAppService;
            _drawerRepository = drawerRepository;
            _branchOfficeRepository = branchOfficeRepository;
            _bankRepository = bankRepository;
            _backgroundJobManager = backgroundJobManager;
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

        //public void Create(CreateInvoiceInput input)
        //{
        //    int TenantId;
        //    if (AbpSession.TenantId != null)
        //    {
        //        try
        //        {
        //            TenantId = AbpSession.TenantId.Value;
        //            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(TenantId, TicoPayReports.Factura);

        //            var allServices = input.InvoiceLines;
        //            var allRegisters = _invoiceManager.GetallRegisters(TenantId);
        //            var tenant = _tenantManager.Get(TenantId);
        //            Certificate certified = null;


        //            //obtiene el certificado si se valida con hacienda
        //            if (tenant.ValidateHacienda)
        //                certified = _tenantAppService.GetCertified(TenantId);

        //            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

        //            using (var unitOfWork = _unitOfWorkManager.Begin())
        //            {
        //                // var nextInvoiceNumber = _invoiceManager.CheckInvoiceNumber();
        //                ValidateTribunet validateHacienda = new ValidateTribunet();

        //                Invoice @invoice = Invoice.Create(TenantId, "", DateTimeZone.Now(), input.ClientId, tenant, tenant.ConditionSaleType, tenant.CodigoMoneda);

        //                int line = 1;
        //                decimal _taxamount = 0, _amountdiscountTotal = 0, _totalgravado = 0, _totalexento = 0;
        //                foreach (var cs in allServices)
        //                {
        //                    Service _service = _serviceManager.Get(cs.IdService);

        //                    if (_service == null)
        //                    {
        //                        var msj = "Servicio no encontrado, Id " + cs.IdService.ToString() + " no válido.";
        //                        throw new UserFriendlyException(msj);
        //                    }


        //                    if (_service.Price <= 0)
        //                        continue;

        //                    string title = cs.Servicio;

        //                    if (title.Length > MaxTitleLength)
        //                        title = cs.Servicio.Substring(0, MaxTitleLength-1);

        //                    @invoice.AssignInvoiceLine(TenantId, cs.Precio, cs.Impuesto, cs.Descuento, "", title, cs.Cantidad, LineType.Service, _service, null, @invoice, line++);

        //                    var _amountdiscount = (((cs.Precio * cs.Cantidad) * cs.Descuento) / 100);

        //                    var subtotal = (cs.Precio * cs.Cantidad);

        //                    if (cs.Impuesto > 0)
        //                        _totalgravado = _totalgravado + subtotal;
        //                    else
        //                        _totalexento = _totalexento + subtotal;


        //                    _taxamount = _taxamount + cs.Impuesto;
        //                    _amountdiscountTotal = _amountdiscountTotal + _amountdiscount;
        //                }

        //                //@invoice.SetInvoiceNumber(nextInvoiceNumber);
        //                @invoice.SetInvoiceNumber(_invoiceManager.CheckInvoiceNumber());
        //                @invoice.SetInvoiceConsecutivo(allRegisters, tenant);
        //                @invoice.SetInvoiceNumberKey(@invoice.DueDate, @invoice.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);

        //                //** el param 1 debe enviar el taxamount, el 2 el price*qty - desc y el 3 el descuento
        //                @invoice.SetInvoiceTotalCalculate(_taxamount, _amountdiscountTotal, _totalgravado, _totalexento);

        //                // crear XML 
        //                Client client = _clientManager.Get(@invoice.ClientId.Value);

        //                List<XSD.FacturaElectronicaMedioPago> listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();
        //                listMedioPago.Add((XSD.FacturaElectronicaMedioPago)input.PaymentType);

        //                @invoice.CreateXML(@invoice, client, tenant, listMedioPago, certified);//preguntarle a carlos

        //                @invoice.CreatePDF(@invoice, client, tenant, listMedioPago, _facturaReportSettings);

        //                Uri XML = SaveAzureStorage(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), "xmlinvoice"); //signed
        //                Uri PDF = SaveAzureStorage(invoice.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), "pdfinvoice");

        //                @invoice.SetInvoiceXML(XML, PDF);

        //                _invoiceManager.Create(@invoice);

        //                _unitOfWorkManager.Current.SaveChanges();

        //                PayInvoice((int)input.PaymentType, input.Transaction, invoice.Id);

        //                string voucherk = invoice.VoucherKey;

        //                if (tenant.ValidateHacienda)
        //                {
        //                    //enviar factura a hacienda

        //                    string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, invoice, invoice.VoucherKey, null);
        //                    //string validate = validateHacienda.SendResponseTribunet("GET", null, null, null, "506140817310174178825001000010100000000561KJFKBSHR");

        //                    if (validate == "-1")
        //                    {
        //                        invoice.StatusVoucher = VoucherSituation.withoutInternet;
        //                        int _vouchersituation = (int)VoucherSituation.withoutInternet;
        //                        invoice.VoucherKey = invoice.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + invoice.VoucherKey.Substring(42, 8);
        //                        invoice.SendInvoice = false;
        //                    }
        //                    else
        //                    {
        //                        invoice.StatusVoucher = VoucherSituation.Normal;
        //                        invoice.SendInvoice = true;
        //                    }
        //                }

        //                // Enviar Correo Electronico 
        //                mail.SendMailTicoPay(client.Email, subject, emailbody + BuildConfirmationButton(invoice), emailfooter, "",
        //                    Path.Combine(WorkPaths.GetXmlPath(), voucherk + ".xml"),
        //                    Path.Combine(WorkPaths.GetPdfPath(), voucherk + ".pdf"),
        //                    Path.Combine(WorkPaths.GetQRPath(), voucherk + ".png"),
        //                    Path.Combine(WorkPaths.GetXmlSignedPath(), voucherk + ".xml"), tenant.AlternativeEmail); //signed

        //                if (tenant.SmsNoficicarFacturaACobro && ticoTalkClient.IsAuthenticated)
        //                {
        //                    ticoTalkClient.SendSms(tenant.IdentificationNumber, client.MobilNumber, BuildSmsMessage(tenant.Name, tenant.CodigoMoneda.ToString(), invoice.Number, invoice.Total));
        //                }

        //                unitOfWork.Complete();

        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            throw new UserFriendlyException(ex.GetBaseException().Message);
        //        }

        //    }
        //    else
        //    {
        //        throw new UserFriendlyException("You are not logged in to any company");
        //    }

        //}

        #region Metodo usado por TicoPay Movil para crear una factura
        public InvoiceApiDto Create(CreateInvoiceDTO input)
        {
            Client client = null;
            int TenantId;
            if (AbpSession.TenantId != null)
            {
                try
                {
                    #region ANTES DE ENTRAR EN CALCULOS DE LINEAS
                    TenantId = AbpSession.TenantId.Value;
                    var tenant = _tenantManager.Get(TenantId);

                    _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(TenantId, TicoPayReports.Factura, tenant.ComercialName);

                    var allServices = input.InvoiceLines;
                    Drawer drawer = _drawersAppService.getUserDrawersOpen();
                    if (drawer == null)
                    {
                        drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = (int)AbpSession.TenantId }).FirstOrDefault();
                    }
                    var allRegisters = _invoiceManager.GetRegistersbyDrawer(drawer.Id);

                    tenant = _tenantManager.Get(TenantId);
                    Certificate certified = null;
                    #endregion

                    input.TipoFirma = input.TipoFirma == null ? tenant.FirmaRecurrente : input.TipoFirma; // esto se hace porque no esta implementado los tipos de firmas en movil

                    using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                    {
                        Scope = TransactionScopeOption.RequiresNew,
                        IsTransactional = true,
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
                    {
                        Invoice @invoice;
                        #region Creacion de Invoice
                        if (input.CodigoMoneda != null)
                        {
                            @invoice = Invoice.Create(TenantId, "", DateTimeZone.Now(), input.ClientId, tenant, tenant.ConditionSaleType, (FacturaElectronicaResumenFacturaCodigoMoneda)input.CodigoMoneda, input.TipoFirma, input.TypeDocument,input.ExternalReferenceNumber, input.GeneralObservation);
                        }
                        else
                        {
                            @invoice = Invoice.Create(TenantId, "", DateTimeZone.Now(), input.ClientId, tenant, tenant.ConditionSaleType, tenant.CodigoMoneda, input.TipoFirma, input.TypeDocument, input.ExternalReferenceNumber, input.GeneralObservation);
                        }
                        //Client client = _clientManager.Get(@invoice.ClientId.Value);

                        //invoice.ClientName = client.Name + " " + (client.IdentificationType != IdentificacionTypeTipo.Cedula_Juridica ? client.LastName : string.Empty);
                        //invoice.ClientAddress = (client.Barrio != null ? client.Barrio.NombreBarrio + ", " + client.Barrio.Distrito.NombreDistrito + ", " + client.Address : string.Empty);
                        //invoice.ClientEmail = client.Email;
                        //invoice.ClientMobilNumber = client.MobilNumber;
                        //invoice.ClientIdentificationType = client.IdentificationType;
                        //invoice.ClientIdentification = client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? client.IdentificacionExtranjero : client.Identification;
                        //invoice.ClientPhoneNumber = client.PhoneNumber;

                        if (invoice.TypeDocument == TypeDocumentInvoice.Invoice && @invoice.ClientId != null)
                        {
                            client = _clientManager.Get(@invoice.ClientId.Value);
                            @invoice = headerClient(invoice, client);
                        }
                        else
                        {
                            invoice.ClientName = input.ClientName;
                            invoice.ClientEmail = input.ClientEmail;
                            invoice.ClientMobilNumber = input.ClientMobilNumber;
                            if (!String.IsNullOrWhiteSpace(input.ClientIdentification))
                            {
                                invoice.ClientIdentificationType = (IdentificacionTypeTipo)input.ClientIdentificationType;
                                invoice.ClientIdentification = input.ClientIdentification;
                            }
                            invoice.ClientPhoneNumber = input.ClientPhoneNumber;
                        }

                        if (input.IsContingency)
                        {
                            invoice.IsContingency = input.IsContingency;
                            invoice.ConsecutiveNumberContingency = input.ConsecutiveNumberContingency;
                            invoice.ReasonContingency = input.ReasonContingency.Length > MaxReasonContingency ? input.ReasonContingency.Substring(0, MaxReasonContingency - 1) : input.ReasonContingency;
                            invoice.DateContingency = input.DateContingency;
                        }

                        if(input.ExternalReferenceNumber != null)
                        {
                            invoice.ExternalReferenceNumber = input.ExternalReferenceNumber;
                        }
                        int line = 1;
                        #endregion
                        decimal _taxamount = 0, _amountdiscountTotal = 0, _totalgravado = 0, _totalexento = 0, _subTotalGeneral = 0;

                        #region Operaciones con lineas hay que mejorarlo para integrarlo con el web
                        foreach (var cs in allServices)
                        {
                            //Verifica en el futuro esta implementacion esta un poco $#%%@&#$%%$$##
                            //
                            string title = cs.Servicio;
                            Service _service = null;
                            Product _product = null;
                            LineType lineType = LineType.Service;//Valor por Defecto la no mapeao

                            
                            switch (cs.Tipo)
                            {
                                case LineType.Service:
                                    lineType = LineType.Service;
                                    if (cs.IdService != null && cs.IdService != Guid.Empty)
                                        {
                                        _service = _serviceManager.Get(cs.IdService);
                                        title = _service.Name;
                                        cs.UnidadMedida = _service.UnitMeasurement;
                                        cs.UnidadMedidaOtra = _service.UnitMeasurementOthers;
                                    }
                                    break;
                                case LineType.Product:
                                    lineType = LineType.Product;
                                    if (cs.IdProducto != null && cs.IdProducto != Guid.Empty)
                                    {
                                        _product = _productRepository.Get(cs.IdProducto);
                                        title = _product.Name;
                                        cs.UnidadMedida = _product.UnitMeasurement;
                                    }
                                    break;
                            }
                           

                            var tax = _taxRepository.Get(cs.IdImpuesto);

                            if (title.Length > MaxTitleLength)
                                title = title.Substring(0, MaxTitleLength - 1);

                            // Si hay Descuento General Calculo sobre las lineas del invoice
                            if (input.TypeDiscountGeneral == 1 && input.DiscountGeneral != null && input.DiscountGeneral > 0)
                            {
                                var gdSubtotal = (cs.Precio * cs.Cantidad);
                                var globaldiscount = (gdSubtotal * input.DiscountGeneral) / 100;
                                var totalDiscount = (((input.DiscountGeneral / 100) + (cs.Descuento / 100)) - ((input.DiscountGeneral / 100) * (cs.Descuento / 100))) * 100;
                                cs.Descuento = Decimal.Round((Decimal)totalDiscount, 2, MidpointRounding.AwayFromZero);
                                gdSubtotal = Decimal.Round((Decimal)(gdSubtotal - globaldiscount), MidpointRounding.AwayFromZero);
                                cs.Impuesto = Decimal.Round(((gdSubtotal * tax.Rate) / 100), 2, MidpointRounding.AwayFromZero);
                                cs.Total = gdSubtotal + cs.Impuesto;
                                @invoice.DiscountPercentaje = input.DiscountGeneral.Value;

                            }

                            @invoice.AssignInvoiceLine(TenantId, cs.Precio, cs.Impuesto, cs.Descuento, null,cs.Note, title, cs.Cantidad, LineType.Service, _service, _product, @invoice, line++,
                                tax, tax.Id, (UnidadMedidaType)cs.UnidadMedida, cs.UnidadMedidaOtra);

                            var _amountdiscount = (((cs.Precio * cs.Cantidad) * cs.Descuento) / 100);

                            var subtotal = (cs.Precio * cs.Cantidad);

                            if (cs.Impuesto > 0)
                                _totalgravado = _totalgravado + subtotal;
                            else
                                _totalexento = _totalexento + subtotal;


                            _taxamount = _taxamount + cs.Impuesto;

                            _amountdiscountTotal = _amountdiscountTotal + _amountdiscount;
                            _subTotalGeneral = Decimal.Round(_subTotalGeneral + subtotal, 2, MidpointRounding.AwayFromZero);
                        }
                        #endregion

                        #region Calculos de consecutivos y totales


                        //** el param 1 debe enviar el taxamount, el 2 el price*qty - desc y el 3 el descuento
                        @invoice.SetInvoiceTotalCalculate(_taxamount, _amountdiscountTotal, _totalgravado, _totalexento, 0, 0);
                        #endregion

                        // Es una factura de Credito
                        #region Cuando es a credito se calcula la fecha de vencimiento del recibo OJO se hace diferente que el web
                        if (input.CreditTerm > 0)
                        {
                            invoice.CreditTerm = (int)input.CreditTerm;
                            invoice.ExpirationDate = DateTimeZone.Now().AddDays(invoice.CreditTerm);
                            invoice.ConditionSaleType = FacturaElectronicaCondicionVenta.Credito;
                        }
                        else
                        {
                            invoice.ConditionSaleType = FacturaElectronicaCondicionVenta.Contado;
                        }
                        #endregion

                        #region Se agregan los medios de pagos y se setea el tipo de firma recurrente
                        List<XSD.FacturaElectronicaMedioPago> listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();

                        if (input.ListPaymentType != null)
                        {
                            AddPaymentsMethods(input.ListPaymentType, listMedioPago);
                        }

                        SetFirmTypeRecurrente(input, tenant, invoice);


                        #endregion

                        #region Se guarda la factura se envian las notificaciones por email y SMS se valida con hacienda

                        BeginPostCalculateInvoicesOperations(allRegisters, tenant, invoice, drawer);

                        @invoice.SetInvoiceConsecutivo(allRegisters);

                        invoice.SendInvoice = false;

                        _invoiceManager.Create(@invoice);


                        _unitOfWorkManager.Current.SaveChanges();

                        //esto es lo unico diferente en esta region como se setan los pagos
                        if (listMedioPago.Count > 0)
                            PayInvoiceList(input.ListPaymentType, invoice.Id);


                        //Client client = _clientManager.Get(@invoice.ClientId.Value);

                        // crear XML 
                        unitOfWork.Complete();

                        _backgroundJobManager.Enqueue<UploadAndSendInvoiceJob, UploadAndSendInvoiceJobArgs>(
                            new UploadAndSendInvoiceJobArgs
                            {
                                idinvoice = invoice.Id,
                                TenantId = tenant.Id
                            });

                        #endregion

                        return Mapper.Map<InvoiceApiDto>(@invoice);
                    }
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException(ex.GetBaseException().Message);
                }

            }
            else
            {
                throw new UserFriendlyException("You are not logged in to any company");
            }

        }
        #endregion

        #region Metodo usado por TicoPay Movil para crear facturas en lote
        public async Task CreateInvoices(List<CreateInvoiceDTO> Invoices)
        {
            int TenantId;
            if (AbpSession.TenantId != null)
            {
                try
                {
                    TenantId = AbpSession.TenantId.Value;
                    var tenant = _tenantManager.Get(TenantId);
                    _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(TenantId, TicoPayReports.Factura, tenant.ComercialName);

                    var drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = tenant.Id }).FirstOrDefault();
                    var allRegisters = _invoiceManager.GetRegistersbyDrawer(drawer.Id);
                    

                    Certificate certified = null;

                    var countLLave = Invoices.Where(x => x.TipoFirma == FirmType.Llave).Count();

                    if ((countLLave > 0) && (isdigitalPendingInvoice(TenantId)))
                    {

                        throw new UserFriendlyException("Posee facturas pendientes por firma digital. Por favor complete el proceso de firma y envío a hacienda de estas factura para poder generar nuevas facturas con llave criptográfica.");

                    }

                    if ((tenant.ValidateHacienda) && ((String.IsNullOrEmpty(tenant.UserTribunet) || String.IsNullOrEmpty(tenant.PasswordTribunet))))
                    {
                        throw new UserFriendlyException("Debe ingresar las credenciales para el envío de sus comprobantes a Hacienda. Por favor vaya a Configuración --> Compañía e ingrese estos datos (los mismos son generados en la página de ATV)");
                    }
                    //obtiene el certificado si se valida con hacienda
                    if ((tenant.ValidateHacienda) && ((tenant.TipoFirma == FirmType.Llave) || (tenant.TipoFirma == FirmType.Todos)))
                        certified = _tenantAppService.GetCertified(TenantId);

                    TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);


                    foreach (var input in Invoices)
                    {
                        input.TipoFirma = input.TipoFirma == null ? tenant.FirmaRecurrente : input.TipoFirma;

                        using (var unitOfWork = _unitOfWorkManager.Begin())
                        {
                            Client client = null;
                            var allServices = input.InvoiceLines;


                            // var nextInvoiceNumber = _invoiceManager.CheckInvoiceNumber();
                            ValidateTribunet validateHacienda = new ValidateTribunet();


                            Invoice @invoice = Invoice.Create(TenantId, "", DateTimeZone.Now(), input.ClientId, tenant, tenant.ConditionSaleType, tenant.CodigoMoneda);

                            if (invoice.TypeDocument == TypeDocumentInvoice.Invoice)
                            {
                                client = _clientManager.Get(@invoice.ClientId.Value);
                                @invoice = headerClient(invoice, client);
                            }
                            else
                            {
                                invoice.ClientName = input.ClientName;
                                invoice.ClientEmail = input.ClientEmail;
                                invoice.ClientMobilNumber = input.ClientMobilNumber;
                                if (!String.IsNullOrWhiteSpace(input.ClientIdentification))
                                {
                                    invoice.ClientIdentificationType = (IdentificacionTypeTipo)input.ClientIdentificationType;
                                    invoice.ClientIdentification = input.ClientIdentification;
                                }
                                invoice.ClientPhoneNumber = input.ClientPhoneNumber;
                            }
                            invoice.DrawerId = drawer.Id;
                            int line = 1;
                            decimal _taxamount = 0, _amountdiscountTotal = 0, _totalgravado = 0, _totalexento = 0;
                            foreach (var cs in allServices)
                            {
                                Service _service = _serviceManager.Get(cs.IdService);

                                if (_service == null)
                                {
                                    var msj = "Servicio no encontrado, Id " + cs.IdService.ToString() + " no válido.";
                                    throw new UserFriendlyException(msj);
                                }


                                if (_service.Price <= 0)
                                    continue;

                                string title = cs.Servicio;

                                if (title.Length > MaxTitleLength)
                                    title = cs.Servicio.Substring(0, MaxTitleLength - 1);

                                // Solicitar estos campos en el web api
                                // _service.Tax, _service.TaxId, _service.UnitMeasurement,_service.UnitMeasurementOthers

                                @invoice.AssignInvoiceLine(TenantId, cs.Precio, cs.Impuesto, cs.Descuento, "", "", title, cs.Cantidad, LineType.Service, _service, null, @invoice, line++,
                                    _service.Tax, _service.TaxId, _service.UnitMeasurement, _service.UnitMeasurementOthers);

                                var _amountdiscount = (((cs.Precio * cs.Cantidad) * cs.Descuento) / 100);

                                var subtotal = (cs.Precio * cs.Cantidad);

                                if (cs.Impuesto > 0)
                                    _totalgravado = _totalgravado + subtotal;
                                else
                                    _totalexento = _totalexento + subtotal;


                                _taxamount = _taxamount + cs.Impuesto;
                                _amountdiscountTotal = _amountdiscountTotal + _amountdiscount;
                            }

                            //@invoice.SetInvoiceNumber(nextInvoiceNumber);
                            @invoice.SetInvoiceNumber(_invoiceManager.CheckInvoiceNumber(invoice.TypeDocument));
                            @invoice.SetInvoiceConsecutivo(allRegisters, tenant,true,drawer);
                            int voucherSituation = (int)(invoice.IsContingency ? VoucherSituation.Contigency : VoucherSituation.Normal);
                            @invoice.SetInvoiceNumberKey(@invoice.DueDate, @invoice.ConsecutiveNumber, tenant, voucherSituation);

                            //** el param 1 debe enviar el taxamount, el 2 el price*qty - desc y el 3 el descuento
                            @invoice.SetInvoiceTotalCalculate(_taxamount, _amountdiscountTotal, _totalgravado, _totalexento, 0, 0);

                            // crear XML 


                            List<XSD.FacturaElectronicaMedioPago> listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();

                            if (input.ListPaymentType != null)
                            {
                                foreach (PaymentInvoceDto paymetnInvoce in input.ListPaymentType)
                                {
                                    listMedioPago.Add((XSD.FacturaElectronicaMedioPago)paymetnInvoce.TypePayment);
                                }
                            }
                            else
                            {
                                listMedioPago.Add(XSD.FacturaElectronicaMedioPago.Efectivo);
                            }

                            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (input.TipoFirma == FirmType.Llave)))
                            {
                                @invoice.CreateXML(@invoice, client, tenant, listMedioPago, certified);

                                @invoice.CreatePDF(@invoice, client, tenant, listMedioPago, null, null, _facturaReportSettings);

                                Uri XML = SaveAzureStorage(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), "xmlinvoice"); //signed
                                Uri PDF = SaveAzureStorage(invoice.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), "pdfinvoice");

                                @invoice.SetInvoiceXML(XML, PDF);
                            }
                            invoice.SendInvoice = false;

                            await _invoiceManager.CreateAsync(@invoice);
                            await _unitOfWorkManager.Current.SaveChangesAsync();



                            if (listMedioPago.Count > 0)
                                PayInvoiceList(input.ListPaymentType, invoice.Id);



                            string voucherk = invoice.VoucherKey;

                            if ((tenant.ValidateHacienda) && (input.TipoFirma == FirmType.Llave))
                            {
                                //enviar factura a hacienda

                                string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, invoice, invoice.VoucherKey, null, null);


                                if (validate == "-1")
                                {
                                    invoice.StatusVoucher = VoucherSituation.withoutInternet;
                                    int _vouchersituation = (int)VoucherSituation.withoutInternet;
                                    invoice.VoucherKey = invoice.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + invoice.VoucherKey.Substring(42, 8);
                                    invoice.SendInvoice = false;
                                }
                                else
                                {
                                    invoice.StatusVoucher = VoucherSituation.Normal;
                                    invoice.SendInvoice = true;
                                }

                            }
                            unitOfWork.Complete();
                            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (input.TipoFirma == FirmType.Llave)))
                            {
                                // Enviar Correo Electronico 
                                if ((client!=null) && (!String.IsNullOrWhiteSpace(client.Email)))
                                {
                                    mail.SendMailTicoPay(client.Email, (invoice.TypeDocument == TypeDocumentInvoice.Ticket ? subjectTicket : subject), string.Format(emailbody, invoice.TypeDocument.GetDescription(), invoice.TypeDocument.GetDescription()) + BuildConfirmationButton(invoice), emailfooter, "",
                                      Path.Combine(WorkPaths.GetXmlPath(), voucherk + ".xml"),
                                      Path.Combine(WorkPaths.GetPdfPath(), voucherk + ".pdf"),
                                      Path.Combine(WorkPaths.GetQRPath(), voucherk + ".png"),
                                      Path.Combine(WorkPaths.GetXmlSignedPath(), voucherk + ".xml"), tenant.AlternativeEmail,
                                      tenant.ComercialName, cuentaBanca, (invoice.Client != null) ? invoice.Client.ContactEmail : string.Empty); //signed
                                }
                              

                                if (tenant.SmsNoficicarFacturaACobro && ticoTalkClient.IsAuthenticated)
                                {
                                    ticoTalkClient.SendSms(tenant.TenancyName, tenant.IdentificationNumber, client.PhoneNumber, SmsFacturaACobroMessage);
                                }
                            }
                            else
                            {
                                // Enviar Correo Electronico 
                                mail.SendMailTicoPay(tenant.Email, (invoice.TypeDocument == TypeDocumentInvoice.Ticket ? subjectTicket : subject), String.Format(emailbodySignature, invoice.Number, emailSteps), emailfooter, "", tenant.AlternativeEmail, tenant.ComercialName, "");
                            }

                        }
                    }




                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException(ex.GetBaseException().Message);
                }

            }
            else
            {
                throw new UserFriendlyException("You are not logged in to any company");
            }

        }
        #endregion

        #region Metodo usado por Ticopay Web facturación manual
        [UnitOfWork(isTransactional: false)]
        public Invoice CreateList(CreateInvoiceInput input, List<PaymentInvoceDto> listPaymetnInvoce)
        {
            int TenantId;
            Guid idinvoice;
            List<XSD.FacturaElectronicaMedioPago> listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();
            if (AbpSession.TenantId != null)
            {


                #region ANTES DE ENTRAR EN CALCULOS DE LINEAS
                TenantId = AbpSession.TenantId.Value;
                Tenant tenant = _tenantManager.Get(TenantId);
                var allServices = input.InvoiceLines;
                _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(TenantId, TicoPayReports.Factura, tenant.ComercialName);

                if ((tenant.ValidateHacienda) && ((String.IsNullOrEmpty(tenant.UserTribunet) || String.IsNullOrEmpty(tenant.PasswordTribunet))))
                {
                    throw new UserFriendlyException("Debe ingresar las credenciales para el envío de sus comprobantes a Hacienda. Por favor vaya a Configuración --> Compañía e ingrese estos datos (los mismos son generados en la página de ATV)");
                }
                if (input.Drawer == null)
                {
                    throw new UserFriendlyException("Debe abrir una caja para realizar la facturación electrónica.");
                }
                #endregion
                Invoice @invoice = null;
                using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                {
                    Scope = TransactionScopeOption.RequiresNew,
                    IsTransactional = true,
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }))
                {

                    var allRegisters = _invoiceManager.GetRegistersbyDrawer(input.Drawer.Id);

                    var unitMeasurementDefault = tenant.UnitMeasurementDefault == null ? 0 : tenant.UnitMeasurementDefault;
                    var unitMeasurementOthersDefault = tenant.UnitMeasurementOthersDefault == null ? string.Empty : tenant.UnitMeasurementOthersDefault;



                    #region Creacion de Invoice
                    @invoice = Invoice.Create(TenantId, "", DateTimeZone.Now(), input.ClientId, tenant, (FacturaElectronicaCondicionVenta)input.ConditionSaleType, input.Coin, input.TipoFirma, input.TypeDocument);
                    Client client = null;

                    if (invoice.TypeDocument == TypeDocumentInvoice.Invoice && (invoice.ClientId != null && invoice.ClientId != Guid.Empty))
                    {
                        client = _clientManager.Get(@invoice.ClientId.Value);
                        @invoice = headerClient(invoice, client);
                    }
                    else
                    {
                        invoice.ClientName = input.ClientName;
                        invoice.ClientAddress = input.ClientAddress;
                        invoice.ClientEmail = input.ClientEmail;
                        invoice.ClientMobilNumber = input.ClientMobilNumber;
                        if (!String.IsNullOrWhiteSpace(input.ClientIdentification))
                        {
                            invoice.ClientIdentificationType = (IdentificacionTypeTipo)input.ClientIdentificationType;
                            invoice.ClientIdentification = input.ClientIdentification;
                        }
                        invoice.ClientPhoneNumber = input.ClientPhoneNumber;
                    }
                    if (input.IsContingency)
                    {
                        invoice.IsContingency = input.IsContingency;
                        invoice.ConsecutiveNumberContingency = input.ConsecutiveNumberContingency;
                        invoice.ReasonContingency = input.ReasonContingency.Length > MaxReasonContingency ? input.ReasonContingency.Substring(0, MaxReasonContingency - 1) : input.ReasonContingency;
                        invoice.DateContingency = input.DateContingency;
                    }

                    if (input.Drawer != null)
                    {
                        invoice.DrawerId = input.Drawer.Id;
                    }
                    invoice.GeneralObservation = input.GeneralObservation;

                    int line = 1;
                    #endregion

                    #region Operaciones con lineas
                    foreach (var cs in allServices)
                    {
                        if (cs.UnidadMedida == null)
                        {
                            cs.UnidadMedida = unitMeasurementDefault;
                            cs.UnidadMedidaOtra = unitMeasurementOthersDefault;
                        };

                        Service service;
                        if (cs.IdProduct != null && cs.Tipo == LineType.Service)
                        {
                            service = _serviceManager.Get(cs.IdProduct);
                        }
                        else
                        {
                            service = null;
                        }

                        Product producto;
                        if (cs.IdProduct != null && cs.Tipo == LineType.Product)
                        {
                            producto = _productRepository.Get(cs.IdProduct);
                        }
                        else
                        {
                            producto = null;
                        }
                        string title = cs.Servicio;

                        if (title.Length > MaxTitleLength)
                            title = cs.Servicio.Substring(0, MaxTitleLength - 1);

                        var tax = _taxRepository.Get(cs.IdImpuesto);
                        //var subTotal = Invoice.GetValor(cs.Precio * cs.Cantidad, 1);
                        //var total = Invoice.GetValor((subTotal - (subTotal * (cs.Descuento / 100))), 1);
                        //var impuesto = total * (tax.Rate / 100);
                        //cs.TotalImpuesto = Invoice.GetValor(impuesto, 1);
                        //cs.ReCalcularLineas(cs, tax);
                        @invoice.AssignInvoiceLine(TenantId, cs.Precio, cs.Impuesto, cs.Descuento, cs.DescriptionDiscount, cs.Note, title, cs.Cantidad, cs.Tipo, service, producto, @invoice, line++,
                            tax, cs.IdImpuesto, (UnidadMedidaType)cs.UnidadMedida, cs.UnidadMedidaOtra);

                    }
                    #endregion

                    #region calculo de  totales



                    //** el param 1 debe enviar el taxamount, el 2 el price*qty - desc y el 3 el descuento
                    @invoice.SetInvoiceTotalCalculate(allServices.Sum(d => d.TotalImpuesto),
                                                      allServices.Sum(d => d.TotalDescuento),
                                                      allServices.Where(d => d.Impuesto > 0 && d.Tipo == LineType.Service).Sum(d => (d.Precio * d.Cantidad)),
                                                      allServices.Where(d => d.Impuesto == 0 && d.Tipo == LineType.Service).Sum(d => (d.Precio * d.Cantidad)),
                                                      allServices.Where(d => d.Impuesto > 0 && d.Tipo == LineType.Product).Sum(d => (d.Precio * d.Cantidad)),
                                                      allServices.Where(d => d.Impuesto == 0 && d.Tipo == LineType.Product).Sum(d => (d.Precio * d.Cantidad)));
                    #endregion

                    #region Cuando es a credito se calcula la fecha de vencimiento del recibo OJO se hace diferente que el movil
                    if (input.ConditionSaleType == (int)FacturaElectronicaCondicionVenta.Credito)
                    {
                        if (input.DayCredit != null)
                        {
                            @invoice.CreditTerm = input.DayCredit.Value;
                            @invoice.ExpirationDate = DateTimeZone.Now().AddDays(input.DayCredit.Value);
                        }
                    }
                    else
                    {
                        @invoice.Status = Status.Completed;
                    }


                    if (input.TypeDiscountGeneral == 1)
                    {
                        @invoice.DiscountPercentaje = (decimal)input.DiscountPercentage;
                    }
                    #endregion

                    //invoice.DiscountAmount = (input.DiscountGeneral != null) ? (decimal)input.DiscountGeneral : 0;

                    #region Se agregan los medios de pagos y se setea el tipo de firma recurrente

                    if (listPaymetnInvoce.Count > 0)
                    {
                        AddPaymentsMethods(listPaymetnInvoce, listMedioPago);
                    }

                    SetFirmTypeRecurrente(input, tenant, invoice);



                    #endregion

                    #region Se guarda la factura se envian las notificaciones por email y SMS se valida con hacienda


                    // calculo de consecutivo
                    BeginPostCalculateInvoicesOperations(allRegisters, tenant, invoice, input.Drawer);
                    // se actualiza
                    @invoice.SetInvoiceConsecutivo(allRegisters);

                    invoice.SendInvoice = false;

                    _invoiceManager.Create(@invoice);

                    _unitOfWorkManager.Current.SaveChanges();

                    //esto es lo unico diferente en esta region como se setan los pagos
                    if (input.ConditionSaleType != (int)FacturaElectronicaCondicionVenta.Credito)
                        PayInvoiceList(listPaymetnInvoce, invoice.Id);

                    unitOfWork.Complete();
                    idinvoice = invoice.Id;

                    #endregion
                }

                // Invoice @invoiceNew = _invoiceManager.Get(idinvoice);

                //if (!tenant.IsPos)
                //{
                //    CreateUploadSendElectronicIncoice(idinvoice, tenant.Id);
                //}
                //else
                //{
                    _backgroundJobManager.Enqueue<UploadAndSendInvoiceJob, UploadAndSendInvoiceJobArgs>(
                        new UploadAndSendInvoiceJobArgs {
                            idinvoice = idinvoice,
                            TenantId = tenant.Id
                        });
                //}

                return @invoice;
            }
            else
            {
                throw new UserFriendlyException("You are not logged in to any company");
            }

        }

        public void CreateUploadSendElectronicIncoice(Guid idinvoice, int TenantId)
        {
            Tenant tenant = _tenantManager.Get(TenantId);

            Invoice @invoiceNew = _invoiceRepository.GetAll().Where(x => x.Id == idinvoice && x.TenantId == TenantId).Include("InvoicePaymentTypes")
                .Include("InvoiceLines")
                .Include("InvoiceLines.Tax")
                .Include("InvoicePaymentTypes.Payment")
                .Include("Client.Barrio")
                .Include("Client.Barrio.Distrito")
                .Include("Client.Barrio.Distrito.Canton")
                .Include("Client.Barrio.Distrito.Canton.Provincia")
                .Include("Drawer.BranchOffice").FirstOrDefault();

            var payments = InfoPaymentInvoice(idinvoice);
            var listInfoPago = AddPaymentsMethods2(payments);


            @invoiceNew.ChangeType = tenant.IsConvertUSD ? _rateOfDay.RateDate(@invoiceNew.DueDate).rate : 1;

            var branchOffice = infoBranchOffice(@invoiceNew.Drawer.BranchOffice.Id);
            var BranchOfficeInfo = AddBranchOfficeInfo(branchOffice);

            // crear XML 

            Certificate certified = null;
            certified = BeginCreateAndUploadXMlAndPDF(tenant, certified, @invoiceNew, @invoiceNew.Client, null, listInfoPago, BranchOfficeInfo);

            @invoiceNew = BeginValidateWithHacienda(tenant, certified, @invoiceNew, @invoiceNew.Client);

            BeginSendMailAndTicoTalkSmsNotification(tenant, @invoiceNew, @invoiceNew.Client, @invoiceNew.VoucherKey);
        }


        private static void SetFirmTypeRecurrente(IUnityInvoice input, Tenant tenant, Invoice invoice)
        {
            if (tenant.ValidateHacienda)
            {
                if (input.TipoFirma == FirmType.Firma)
                    @invoice.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
            }
        }

        private static void AddPaymentsMethods(List<PaymentInvoceDto> listPaymetnInvoce, List<FacturaElectronicaMedioPago> listMedioPago)
        {
            foreach (PaymentInvoceDto paymetnInvoce in listPaymetnInvoce)
            {
                listMedioPago.Add((XSD.FacturaElectronicaMedioPago)paymetnInvoce.TypePayment);
            }
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
                    BranchDrawOpened.Name= branch.Name;
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

        public List<PaymentInvoceDto> InfoPaymentInvoice(Guid id)
        {

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
            foreach (var list in query)
            {
                if (list.BankId != null)
                    list.BankName = getBankName(list.BankId);
            }
        }

        private string getBankName(Guid? bankId)
        {

            var name = _bankRepository.FirstOrDefault(x => x.IsActive && x.Id == bankId);
            var nameBank = name.Name;
            ;
            return nameBank;
        }



        private void BeginPostCalculateInvoicesOperations(Register allRegisters, Tenant tenant, Invoice invoice, Drawer drawer=null)        {
            if (drawer==null)
            {
                drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = tenant.Id }).FirstOrDefault();
            }
            @invoice.SetInvoiceNumber(_invoiceManager.CheckInvoiceNumber(invoice.TypeDocument));
            @invoice.SetInvoiceConsecutivo(allRegisters, tenant, false, drawer);
            int voucherSituation = (int)(invoice.IsContingency ? VoucherSituation.Contigency : VoucherSituation.Normal);
            @invoice.SetInvoiceNumberKey(@invoice.DueDate, @invoice.ConsecutiveNumber, tenant, voucherSituation);
        }

        private void BeginSendMailAndTicoTalkSmsNotification(Tenant tenant, Invoice invoice, Client client, string voucherk, bool? isFirstInvoiceInTicopay = null)
        {
            string cuentaBanca = "";
            if (tenant.TenancyName == "ticopay")
            {
                cuentaBanca = (isFirstInvoiceInTicopay != null && (bool)isFirstInvoiceInTicopay) ? firstTicopayInvoiceMessage + emailCuentaTicopay : emailCuentaTicopay;
            }

            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (invoice.TipoFirma != FirmType.Firma)))
            {

                // Enviar Correo Electronico 
                if (invoice.ClientEmail != null && invoice.ClientEmail != "")
                {
                    mail.SendMailTicoPay(invoice.ClientEmail, (invoice.TypeDocument == TypeDocumentInvoice.Ticket ? subjectTicket : subject), string.Format(emailbody, invoice.TypeDocument.GetDescription(), invoice.TypeDocument.GetDescription()) + BuildConfirmationButton(invoice), emailfooter, "",
                   Path.Combine(WorkPaths.GetXmlPath(), voucherk + ".xml"),
                   Path.Combine(WorkPaths.GetPdfPath(), voucherk + ".pdf"),
                   Path.Combine(WorkPaths.GetQRPath(), voucherk + ".png"),
                   Path.Combine(WorkPaths.GetXmlSignedPath(), voucherk + ".xml"), tenant.AlternativeEmail,
                   tenant.ComercialName, cuentaBanca, (invoice.Client != null) ? invoice.Client.ContactEmail : string.Empty); //signed
                }

                if (tenant.SmsNoficicarFacturaACobro && invoice.ConditionSaleType == FacturaElectronicaCondicionVenta.Credito)
                {
                    TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

                    if ((ticoTalkClient.IsAuthenticated) && (invoice.ClientMobilNumber != null))
                    {
                        ticoTalkClient.SendSms(tenant.IdentificationNumber, invoice.ClientMobilNumber, BuildSmsMessage(tenant.Name, tenant.CodigoMoneda.ToString(), invoice.Number, invoice.Total));

                    }
                }
            }
            else
            {
                // Enviar Correo Electronico 
                mail.SendMailTicoPay(tenant.Email, (invoice.TypeDocument == TypeDocumentInvoice.Ticket ? subjectTicket : subject), String.Format(emailbodySignature, invoice.Number, emailSteps), emailfooter, "", tenant.AlternativeEmail, tenant.ComercialName, cuentaBanca);

            }
        }


        private Certificate BeginCreateAndUploadXMlAndPDF(Tenant tenant, Certificate certified, Invoice invoice, Client client, List<FacturaElectronicaMedioPago> listMedioPago, List<PaymentInvoice> listInfoPago, List<BranchOffice> infoBranchOffice)
        {
            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (invoice.TipoFirma != FirmType.Firma)))
            {
                //obtiene el certificado si se valida con hacienda
                if (tenant.ValidateHacienda)
                    certified = _tenantAppService.GetCertified(tenant.Id);

                Uri XML, PDF;

                GenerateAndUploadXMLAndPdf(tenant, certified, invoice, client, listMedioPago, listInfoPago, infoBranchOffice, out XML, out PDF);

                @invoice.SetInvoiceXML(XML, PDF);
            }

            return certified;
        }

        private void GenerateAndUploadXMLAndPdf(Tenant tenant, Certificate certified, Invoice invoice, Client client, List<FacturaElectronicaMedioPago> listMedioPago, List<PaymentInvoice> listInfoPago, List<BranchOffice> infoBranchOffice, out Uri XML, out Uri PDF)
        {
            @invoice.CreateXML(@invoice, client, tenant, listMedioPago, certified);
            @invoice.CreatePDF(@invoice, client, tenant, listMedioPago, listInfoPago, infoBranchOffice,  _facturaReportSettings);
            try
            {
                XML = SaveAzureStorage(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), "xmlinvoice");
            }
            catch
            {
                XML = null;
            }
            try
            {
                PDF = SaveAzureStorage(invoice.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), "pdfinvoice");
            }
            catch
            {
                PDF = null;
            }

        }

        private Invoice BeginValidateWithHacienda(Tenant tenant, Certificate certified, Invoice invoice, Client client)
        {
            if (tenant.ValidateHacienda && invoice.TipoFirma == FirmType.Llave)
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                //enviar factura a hacienda
                string validate = "-1";

                TokenResponse token;

                validate = validateHacienda.SendResponseTribunet("POST", client, tenant, invoice, invoice.VoucherKey, null, null, out token);

                if (validate == "-1")
                {
                    ElectronicBillResponse response = null;

                    if (token!=null)
                    {
                        response = validateHacienda.GetComprobanteStatusFromTaxAdministrationRefreshToken(invoice, token.RefreshToken);
                    }
                    else
                    {
                        response = validateHacienda.GetComprobanteStatusFromTaxAdministration(invoice, tenant.UserTribunet, tenant.PasswordTribunet);
                    }

                    if (response == null || (response.IndEstado != "procesando" && response.IndEstado != "aceptado" && response.IndEstado != "rechazado"))
                    {
                        SetInvoiceWithInternet(ref invoice);

                        Uri XML, PDF;
                        GenerateAndUploadXMLAndPdf(tenant, certified, invoice, client, null, null, null, out XML, out PDF);

                        @invoice.SetInvoiceXML(XML, PDF);
                        invoice.ReGenerateQrFromVoucherKey();
                    }
                    else
                    {
                        invoice.StatusVoucher = VoucherSituation.Normal;
                        invoice.SendInvoice = true;
                    }
                }
                else
                {
                    invoice.StatusVoucher = VoucherSituation.Normal;
                    invoice.SendInvoice = true;
                }
            }

            return invoice;
        }

        private bool ValidateWithHacienda()
        {

            return false;
        }

        #endregion

        private void SetInvoiceWithInternet(ref Invoice invoice)
        {
            invoice.StatusVoucher = VoucherSituation.withoutInternet;
            int _vouchersituation = (int)VoucherSituation.withoutInternet;
            invoice.VoucherKey = invoice.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + invoice.VoucherKey.Substring(42, 8);
            invoice.SendInvoice = false;

            var facturaElectronica = Invoice.CreateInvoiceToSerialize(invoice, invoice.Client, invoice.Tenant, null);
            Stream xmlStream = Invoice.GetXML(facturaElectronica);
            Uri XML;
            try
            {
                XML = SaveAzureStorageFromText(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), xmlStream, "xmlinvoice");
            }
            catch
            {
                XML = null;
            }
            
        }

        private void SetInvoiceWithInternetAux(ref Invoice invoice)
        {
            invoice.StatusVoucher = VoucherSituation.withoutInternet;
            int _vouchersituation = (int)VoucherSituation.withoutInternet;
            invoice.VoucherKey = invoice.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + invoice.VoucherKey.Substring(42, 8);
            invoice.SendInvoice = false;
        }

        public InvoiceDto GetDetailInvoice(Guid input)
        {
            var client = _invoiceRepository.GetAll().Where(a => a.Id == input).Include(a => a.Tenant).Include(a => a.Client).FirstOrDefault();

            if (client == null)
            {
                throw new UserFriendlyException("Could not found the invoice, maybe it's deleted.");
            }
            return Mapper.Map<InvoiceDto>(client);
        }

        public List<Invoice> ObtenerFacturaNCR(Guid ClienteId)
        {
            DateTime fechai = Convert.ToDateTime("14/11/2017");
            DateTime fechaf = Convert.ToDateTime("15/11/2017");

            var invoice = _invoiceRepository.GetAll().Where(a => a.ClientId == ClienteId && a.Status == Status.Parked && a.Balance > 0
            && a.DueDate >= fechai && a.DueDate <= fechaf
            && new[] { 781, 782, 783, 784, 785, 786, 787, 788, 789, 790 }.Contains(a.Number) && a.TenantId == 2);

            if (invoice == null)
            {
                throw new UserFriendlyException("Could not found the invoice, maybe it's deleted.");
            }
            return invoice.MapTo<List<Invoice>>();
        }



        public void UpdateRegister(Register register)
        {
            _registerRepository.Update(register);
        }

        public void UpdateBalanceInvoice(Invoice invoice)
        {
            _invoiceRepository.Update(invoice);
        }

        [UnitOfWork(isTransactional: false)]
        public async void CreateServiceInvoices(IList<Service> services, string Email, string AlternativeEmail, string remitente, FirmType firmType)
        {
            var isCreate = false;
            foreach (var service in services)
            {
                //if (service.IsRecurrent)
                //{
                var invoice = await CreateAllServiceInvoices(service);
                if (invoice)
                    isCreate = true;
                //}

            }
            if ((isCreate) && (firmType == FirmType.Firma))
            {
                sendemailbodyRecurren(Email, AlternativeEmail, "Servicios", remitente);
            }
        }

        [UnitOfWork(isTransactional: false)]
        public async Task<bool> CreateAllServiceInvoices(Service service)
        {
            bool isCreate = false;

            if (service.Price <= 0)
                return false;

            var tenant = _tenantManager.Get(service.TenantId);
            //if (tenant.Edition.Name == EditionManager.FreeEditionName)
            //    return;

            int invoicesMonthlyLimit = GetInvoicesMonthlyLimitAsync(tenant.EditionId.Value).Result;
            var allBillingServices = _serviceManager.GetAllBillingServices(service);     
            var drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = tenant.Id }).FirstOrDefault();
            var allRegisters = _invoiceManager.GetRegistersbyDrawer(drawer.Id);
            ValidateTribunet validateHacienda = new ValidateTribunet();
            Certificate certified = null;

            //obtiene el certificado si se valida con hacienda
            if (tenant.ValidateHacienda)
                certified = _tenantAppService.GetCertified(service.TenantId);

            bool isTenantTicopay = tenant.TenancyName == "ticopay";

            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);

            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

            foreach (var cs in allBillingServices)
            {
                bool isFirstInvoiceInTicopay = false;

                var fechaActual = DateTimeZone.Now();

                if (cs.Quantity > 0 && service.Price > 0 && cs.DiscountPercentage < 100)
                {
                    if (isTenantTicopay)
                    {
                        CheckFirstTicoPayInvoice(cs.Client.CreationTime, cs.ClientId, out isFirstInvoiceInTicopay, service.Name);
                    }

                    if (!isFirstInvoiceInTicopay)
                    {
                        if (!cs.CantCreateInvoice((service.CronExpression != CRON_ANNUAL) ? service.CronExpression : string.Format(CRON_ANNUAL, cs.Client.CreationTime.Month), isFirstInvoiceInTicopay))
                            continue;
                    }

                    if (isTenantTicopay)
                    {
                        if (!isFirstInvoiceInTicopay)
                        {
                            var IdentificationClient = (cs.Client != null) ? cs.Client.Identification : "";
                            var tenantAciveByClient = _tenantManager.GetTenantsActiveByClient(IdentificationClient).ToList();

                            if (tenantAciveByClient.Count == 0)
                                continue;
                        }
                    }

                    if (!await TenantCanCreateInvoices(tenant, invoicesMonthlyLimit))
                        continue;

                    Invoice lastInvoice = null;

                    if (isTenantTicopay && cs.LastGeneratedInvoice != null)
                    {
                        lastInvoice = _invoiceRepository.GetAll().Where(c => c.ClientId == cs.ClientId && c.Number == cs.LastGeneratedInvoice).FirstOrDefault();
                    }

                    if (isTenantTicopay && !isFirstInvoiceInTicopay && (lastInvoice != null && lastInvoice.DueDate <= cs.WorkerLastEjecutionDate && lastInvoice.DueDate <= cs.WorkerNextEjecutionDate && fechaActual < cs.WorkerNextEjecutionDate))
                    {
                        continue;
                    }

                    try
                    {

                        Guid idinvoice;
                        List<XSD.FacturaElectronicaMedioPago> listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();
                        Client client = null;
                        using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                        {
                            Scope = TransactionScopeOption.RequiresNew,
                            IsTransactional = true,
                            IsolationLevel = IsolationLevel.ReadUncommitted
                        }))
                        {

                            var @invoice = Invoice.Create(service.TenantId, "", DateTimeZone.Now(), cs.ClientId, tenant, tenant.ConditionSaleType, tenant.CodigoMoneda);
                            @invoice.ChangeType = tenant.IsConvertUSD ? _rateOfDay.RateDate(@invoice.DueDate).rate : 1;
                            client = _clientManager.Get(cs.ClientId);
                            @invoice = headerClient(@invoice, client);
                            #region Operaciones con lineas

                            decimal subtotal = (service.Price * cs.Quantity);
                            decimal _totalDiscount = Decimal.Round((cs.DiscountPercentage * subtotal) / 100, 2, MidpointRounding.AwayFromZero);
                            decimal _tax = Decimal.Round((service.Tax.Rate * (subtotal - _totalDiscount)) / 100, 2, MidpointRounding.AwayFromZero);

                            string title = cs.Service.Name;

                            if (title.Length > MaxTitleLength)
                                title = cs.Service.Name.Substring(0, MaxTitleLength - 1);

                            var serviceLine = _serviceManager.Get(cs.Service.Id);
                            @invoice.AssignInvoiceLine(service.TenantId, cs.Service.Price, _tax, cs.DiscountPercentage, null, "", title, cs.Quantity, LineType.Service, serviceLine, null, @invoice, 1, serviceLine.Tax, service.TaxId, service.UnitMeasurement, service.UnitMeasurementOthers);
                            #endregion

                            #region calculo de  totales
                            //** el param 1 debe enviar el taxamount, el 2 el price*qty - desc y el 3 el descuento
                            decimal _totalgravado = 0, _totalexento = 0;

                            if (_tax > 0)
                                _totalgravado = _totalgravado + subtotal;
                            else
                                _totalexento = _totalexento + subtotal;

                            if (isTenantTicopay)
                            {
                                var smsDebtInvoiceLine = GetSmsDebt(ticoTalkClient, tenant, client).Result;
                                if (smsDebtInvoiceLine.SmsCount > 0)
                                {
                                    @invoice.AssignInvoiceLine(service.TenantId, smsDebtInvoiceLine.SmsCost, 0, 0, null, "", SmsInvoiceLineTitle, smsDebtInvoiceLine.SmsCount, LineType.Service, serviceLine, null, invoice, 2,
                                        serviceLine.Tax, service.TaxId, service.UnitMeasurement, service.UnitMeasurementOthers);
                                    _totalexento += (smsDebtInvoiceLine.SmsCost * smsDebtInvoiceLine.SmsCount);
                                }
                            }

                            @invoice.SetInvoiceTotalCalculate(_tax, _totalDiscount, _totalgravado, _totalexento, 0, 0);
                            #endregion

                            #region Se agregan los medios de pagos y se setea el tipo de firma recurrente
                            listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();
                            listMedioPago.Add(XSD.FacturaElectronicaMedioPago.Efectivo);

                            if (tenant.ValidateHacienda)
                            {
                                @invoice.TipoFirma = tenant.FirmaRecurrente;
                                if (tenant.FirmaRecurrente == FirmType.Firma)
                                    @invoice.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
                            }
                            #endregion

                            #region Se guarda la factura 


                            // calculo de consecutivo
                            BeginPostCalculateInvoicesOperations(allRegisters, tenant, invoice);
                            // se actualiza
                            @invoice.SetInvoiceConsecutivo(allRegisters, tenant, true, drawer);

                            //@invoice.SetInvoiceConsecutivo(allRegisters, tenant);
                            //@invoice.SetInvoiceNumberKey(@invoice.DueDate, @invoice.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);

                            invoice.SendInvoice = false;
                            @invoice.RegisterId = allRegisters.Id;
                            @invoice.SetInvoiceNumber(_invoiceManager.CheckInvoiceNumber(invoice.TypeDocument));

                            _invoiceManager.Create(@invoice);
                            _unitOfWorkManager.Current.SaveChanges();
                            isCreate = true;

                            #endregion

                            #region cronExpression (actualización)
                            var csCurrent = _clientServiceRepository.Get(cs.Id);
                            csCurrent.LastGeneratedInvoice = invoice.Number;
                            if (!isFirstInvoiceInTicopay)
                            {
                                if (csCurrent.State != ClientServiceState.Adjustment)
                                {
                                    var cronExpression = (service.CronExpression != CRON_ANNUAL) ? service.CronExpression : string.Format(CRON_ANNUAL, client.CreationTime.Month);
                                    csCurrent.SetNewEjecutionDates(cronExpression);
                                }
                                else
                                {
                                    csCurrent.KillAdjusmentEjecution();
                                }
                            }
                            else
                            {
                                if ((service.CronExpression != CRON_ANNUAL))
                                {
                                    csCurrent.SetNextMonthNewEjecutionDate();
                                }
                                else
                                {
                                    csCurrent.SetNextYearNewEjecutionDate();
                                }
                            }
                            _serviceManager.UpdateClientService(csCurrent);
                            #endregion

                            idinvoice = invoice.Id;
                            var payments = InfoPaymentInvoice(idinvoice);
                            var listInfoPago = AddPaymentsMethods2(payments);

                            var drawer2 = _drawerRepository.GetAll().Where(x => x.TenantId == tenant.Id && x.IsOpen == true).FirstOrDefault();
                            var branchOffice = infoBranchOffice(drawer.BranchOfficeId);
                            var BranchOfficeInfo = AddBranchOfficeInfo(branchOffice);

                            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (tenant.FirmaRecurrente == FirmType.Llave)))
                            {

                                    // crear XML 
                                    invoice.CreateXML(invoice, client, tenant, listMedioPago, certified);
                                    invoice.CreatePDF(invoice, client, tenant, listMedioPago, listInfoPago, BranchOfficeInfo, _facturaReportSettings);

                                    Uri XML = SaveAzureStorage(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), "xmlinvoice");
                                    Uri PDF = SaveAzureStorage(invoice.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), "pdfinvoice");

                                invoice.SetInvoiceXML(XML, PDF);
                            }
                            unitOfWork.Complete();
                        }

                        Invoice @invoiceNew = _invoiceManager.Get(idinvoice);

                        @invoiceNew = BeginValidateWithHacienda(tenant, certified, @invoiceNew, client);

                        BeginSendMailAndTicoTalkSmsNotification(tenant, @invoiceNew, client, @invoiceNew.VoucherKey, isFirstInvoiceInTicopay);

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

            }
            return isCreate;
        }



        private void CheckFirstTicoPayInvoice(DateTime tenantCreationDate, Guid clientId, out bool isFirstInvoiceInTicopay, string planName)
        {
            var actualDate = DateTimeZone.Now();

            var tenantCentralTime = DateTimeZone.Convert(tenantCreationDate);

            var bottomInvoiceLimit = new DateTime(tenantCentralTime.Year, tenantCentralTime.Month, 1);

            var topInvoiceLimit = new DateTime(tenantCentralTime.Year, tenantCentralTime.Month, 15);
            if (planName == EditionManager.PymeJrAnnualEditionDisplayName || planName == EditionManager.Pyme2AnnualEditionDisplayName ||
                planName == EditionManager.Pyme1AnnualEditionDisplayName || planName == EditionManager.ProfesionalJrAnualEditionDisplayName ||
                planName == EditionManager.ProfesionalAnnualEditionDisplayName || planName == EditionManager.BusinessAnnualEditionDisplayName)
            {
                topInvoiceLimit = new DateTime(tenantCentralTime.Year, tenantCentralTime.Month, DateTime.DaysInMonth(tenantCentralTime.Year, tenantCentralTime.Month),23,59,59);
            }

            if (actualDate >= bottomInvoiceLimit && actualDate <= topInvoiceLimit)
            {
                var recentlyInvoice = _invoiceRepository.Count(d => d.Tenant.Id == 2 && d.ClientId == clientId && (d.Client.CreationTime >= bottomInvoiceLimit && d.Client.CreationTime <= topInvoiceLimit) && (d.DueDate >= bottomInvoiceLimit && d.DueDate <= topInvoiceLimit));

                if (recentlyInvoice == 0)
                {

                    isFirstInvoiceInTicopay = true;
                    return;
                }
            }
            isFirstInvoiceInTicopay = false;
        }

        private async Task<bool> TenantCanCreateInvoices(Tenant tenant, int invoicesMonthlyLimit)
        {
            if (tenant == null || tenant.EditionId == null)
            {
                return false;
            }

            bool canCreateInvoice = true;
            int totalInvoicesInMonth = await GetTotalInvoicesInMonthAsync(tenant.Id);

            if (invoicesMonthlyLimit == 0 || totalInvoicesInMonth >= invoicesMonthlyLimit) //Limite alcanzado
            {
                bool sendInvoicesMonthlyLimitNotification = DateTime.UtcNow.AddDays(tenant.InvoicesMonthlyLimitNotificationInterval * -1) > tenant.LastInvoicesMonthlyLimitNotificationDate.GetValueOrDefault();
                if (sendInvoicesMonthlyLimitNotification)
                {
                    tenant.LastInvoicesMonthlyLimitNotificationDate = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(tenant.Email))
                        mail.SendNoReplyMail(new string[] { tenant.Email, tenant.AlternativeEmail }, "Límite de facturación mensual alcanzado", GetInvoicesMonthlyLimitEmailBody(tenant.Name));
                    _notiticationPublisher.Publish("InvoicesMonthlyLimit", new MessageNotificationData(L("InvoicesMonthlyLimitMessage")), severity: NotificationSeverity.Warn);

                    await _tenantManager.UpdateAsync(tenant);
                }
                canCreateInvoice = false;
            }
            else if ((totalInvoicesInMonth / invoicesMonthlyLimit) > 0.7) //70% del limite alcanzado
            {
                bool sendNearInvoicesMonthlyLimitNotification = DateTime.UtcNow.AddDays(tenant.NearInvoicesMonthlyLimitNotificationInterval * -1) > tenant.LastNearInvoicesMonthlyLimitNotificationDate;
                if (sendNearInvoicesMonthlyLimitNotification)
                {
                    tenant.LastNearInvoicesMonthlyLimitNotificationDate = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(tenant.Email))
                        mail.SendNoReplyMail(new string[] { tenant.Email, tenant.AlternativeEmail }, "Límite de facturación mensual cercano", GetNearInvoicesMonthlyLimitEmailBody(tenant.Name));
                    _notiticationPublisher.Publish("NearInvoicesMonthlyLimit", new MessageNotificationData(L("NearInvoicesMonthlyLimitMessage")), severity: NotificationSeverity.Warn);

                    await _tenantManager.UpdateAsync(tenant);
                }
                canCreateInvoice = true;
            }

            return canCreateInvoice;
        }

        //public void CreateAllServiceInvoices(Service service)
        //{
        //    if (AbpSession.TenantId != null)
        //    {
        //        var allBillingServices = _serviceManager.GetAllBillingServices(service);

        //        foreach (var cs in allBillingServices)
        //        {
        //            var @invoice = Invoice.Create(AbpSession.TenantId.Value, "", DateTime.Now, cs.ClientId);
        //            @invoice.AssignInvoiceLine(AbpSession.TenantId.Value, cs.Service.Price, cs.Service.Tax.Rate, 1, "", cs.Service.Name, 1, LineType.Service, cs.Service, null, @invoice, "");
        //            var @invoice = Invoice.Create(AbpSession.TenantId.Value, "", DateTime.Now, cs.ClientId);
        //            @invoice.AssignInvoiceLine(AbpSession.TenantId.Value, cs.Service.Price, cs.Service.Tax.Rate, 0, "", cs.Service.Name, 1, LineType.Service, cs.Service, null, @invoice, "");


        //            _invoiceManager.Create(@invoice);
        //        }
        //    }
        //    else
        //    {
        //        throw new UserFriendlyException("No TenantId.");
        //    }
        //}

        public ListResultDto<InvoiceDto> GetInvoices()
        {
            var entities = _invoiceRepository.GetAllList();
            return new ListResultDto<InvoiceDto>(entities.MapTo<List<InvoiceDto>>());
        }
        /// <summary>
        /// Facturas pendientes por pago -- banco
        /// </summary>
        /// <returns></returns>
        [UnitOfWork]
        public List<InvoicePendingPayBN> GetInvoicesPendingPay(ClientBN client)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            var query = from invoice in _invoiceRepository.GetAll()
                        join notes in _noteRepository.GetAll() on invoice.Id equals notes.InvoiceId into n
                        from nc in n.DefaultIfEmpty()
                        where invoice.ClientId == client.Id && invoice.Status == Status.Parked && invoice.Balance > 0 && nc.NoteType != NoteType.Credito
                        orderby invoice.DueDate
                        select new InvoicePendingPayBN
                        {
                            Id = invoice.Id,
                            Balance = invoice.Balance,
                            Number = invoice.ConsecutiveNumber,
                            DueDate = invoice.DueDate,
                            Status = invoice.Status,
                            CreditTerm = invoice.CreditTerm
                        };

            var entities = query.Take(2).ToList();

            return entities;
        }
        /// <summary>
        /// Facturas Pendientes PAR - banco
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [UnitOfWork]
        public IList<InvoicePendingPayBN> GetInvoicesPendingPayPAR(int index, List<int> listTenant, int PageSize, out bool indicador)
        {
            var today = DateTimeZone.Now();
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            var query = from invoice in _invoiceRepository.GetAll()
                        join clients in _clientRepository.GetAll() on invoice.ClientId equals clients.Id
                        into temp
                        from j in temp.DefaultIfEmpty()
                        where listTenant.Contains(invoice.TenantId) && j.PagoAutomaticoBn == true && invoice.Balance > 0 && invoice.DueDate <= today && invoice.Status == Status.Parked
                        select new InvoicePendingPayBN
                        {
                            Id = invoice.Id,
                            Balance = invoice.Balance,
                            Number = invoice.ConsecutiveNumber,
                            DueDate = invoice.DueDate,
                            Status = invoice.Status,
                            Code = (long?)j.Code,
                            ClientId = (Guid?)j.Id
                        };

            var entities = query.ToList();

            var realTotalCount = entities.Count();
            int pagecount = realTotalCount > 0 ? (int)Math.Ceiling(realTotalCount / (double)PageSize) : 0;

            indicador = (index < (pagecount - 1));

            if (realTotalCount <= 0)
                return new List<InvoicePendingPayBN>();

            return entities.Skip(index * PageSize).Take(PageSize).MapTo<List<InvoicePendingPayBN>>();
        }
        /// <summary>
        /// Obtiene las facturas por numero de factura -- banco
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [UnitOfWork]
        public InvoicePendingPayBN GetInvoicesByNumber(ClientBN client, string invoicenumber) // chequear payment
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            var query = from invoice in _invoiceRepository.GetAll()
                        join payments in _paymentinvoiceRepository.GetAll() on invoice.Id equals payments.InvoiceId
                        into temp
                        from j in temp.DefaultIfEmpty()
                        where invoice.ClientId == client.Id && invoice.ConsecutiveNumber == invoicenumber && invoice.Balance > 0
                        select new InvoicePendingPayBN
                        {
                            Id = invoice.Id,
                            Balance = invoice.Balance,
                            Number = invoice.ConsecutiveNumber,
                            DueDate = invoice.DueDate,
                            Status = invoice.Status,
                            PaymentDate = (DateTime?)j.Payment.PaymentDate,
                            PaymentId = (Guid?)j.Id
                        };

            return query.ToList().FirstOrDefault();
        }
        /// <summary>
        /// Verifica q no haya facturas mas viejeas por pagar - banco
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [UnitOfWork]
        public List<InvoicePendingPayBN> GetInvoicesPendingOld(ClientBN client, DateTime invoicedate)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            var query = from invoice in _invoiceRepository.GetAll()
                        where invoice.ClientId == client.Id && invoice.Balance > 0 && invoice.DueDate < invoicedate && invoice.Status == Status.Parked
                        orderby invoice.DueDate
                        select new InvoicePendingPayBN
                        {
                            Id = invoice.Id,
                            Number = invoice.ConsecutiveNumber,
                        };

            var entities = query.Take(2).ToList();

            return entities;
        }

        public InvoiceDetailOutput GetDetail(EntityDto<Guid> input)
        {
            var @client = _invoiceRepository.FirstOrDefault(input.Id);

            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }

            return @client.MapTo<InvoiceDetailOutput>();
        }

        public InvoiceDto Get(Guid input)
        {
            var @client = _invoiceRepository.GetAll().Where(a => a.Id == input).Include(a => a.Notes).FirstOrDefault();
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the invoice " + input.ToString() + ", maybe it's deleted.");
            }
            return Mapper.Map<InvoiceDto>(client);
        }

        public Invoice GetInvoice(Guid input)
        {
            var @client = _invoiceRepository.GetAll().Where(a => a.Id == input).Include(a => a.Notes).FirstOrDefault();
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the invoice " + input.ToString() + ", maybe it's deleted.");
            }
            return client;
        }

        public InvoiceApiDto GetPaidInvoice(Guid input)
        {
            var @client = _invoiceRepository.GetAll().Where(a => a.Id == input).Include(a => a.Notes).FirstOrDefault();
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the invoice " + input.ToString() + ", maybe it's deleted.");
            }
            return Mapper.Map<InvoiceApiDto>(client);
        }

        /// <summary>
        /// Searches for clients and returns page result
        /// </summary>
        /// <param name="searchInput"></param>
        /// <returns></returns>
        public IPagedList<InvoiceDto> SearchInvoices(SearchInvoicesInput searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();
            if (drawers != null)
            {
                int defaultPageSize = 10;
                int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

                var entities = _invoiceRepository.GetAll().Where(x => drawers.Any(y => y.Value == x.DrawerId));
                if (searchInput.ClientId != null)
                    entities = entities.Where(a => a.ClientId == searchInput.ClientId);

                if (searchInput.Status != null)
                    entities = entities.Where(a => a.Status == searchInput.Status);

                if (searchInput.StartDueDate != null)
                    entities = entities.Where(a => a.DueDate >= searchInput.StartDueDate);

                if (searchInput.EndDueDate != null)
                {
                    DateTime endDueDate = searchInput.EndDueDate.Value.AddDays(1);
                    entities = entities.Where(a => a.DueDate < endDueDate);
                }
                if (searchInput.InvoiceSelect != null)
                {
                    entities = entities.Where(a => a.Id == searchInput.InvoiceSelect);
                }

                if (searchInput.GroupsId != null)
                    entities = entities.Where(a => a.Client.Groups.Any(g => g.GroupId == searchInput.GroupsId));

            if (searchInput.TypeDocument != null)
                entities = entities.Where(a => a.TypeDocument == searchInput.TypeDocument.Value);

            if (searchInput.ConsecutiveNumber != null)
            {
                var numero = searchInput.ConsecutiveNumber.Length < 10 ? searchInput.ConsecutiveNumber : searchInput.ConsecutiveNumber.Substring(10);
                var number = numero.ToString().PadLeft(10, '0');
                entities = entities.Where(a => a.ConsecutiveNumber.Substring(10) == number);
            }

                if (searchInput.BranchOfficeId != null)
                {
                    entities = entities.Where(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
                }

                if (searchInput.DrawerId != null)
                {
                    entities = entities.Where(a => a.DrawerId == searchInput.DrawerId);
                }

                entities = entities.Include(a => a.Client).Include(a => a.Notes).Include(a => a.InvoiceLines).Include(a => a.InvoiceHistoryStatuses).OrderByDescending(a => a.ConsecutiveNumber);

                if (entities == null)
                {
                    throw new UserFriendlyException("Could not found the invoices, maybe it's deleted.");
                }

                var list = entities.ToList();
                return list.MapTo<List<InvoiceDto>>().ToPagedList(currentPageIndex, defaultPageSize);
            }
            else
                return null;
           
        }


        public ListResultDto<InvoiceApiDto> SearchInvoicesApi(SearchInvoicesApi searchInput)
        {
            IList<DrawerUser> drawerAvailable = _drawersAppService.getUserDrawers(null);
            List<Guid?> drawerIds = null;
            if (!(drawerAvailable == null && drawerAvailable.Count == 0))
            {
                drawerIds = drawerAvailable.Select(d => d.DrawerId).ToList();
            }
            else
            {
                var drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = (int)AbpSession.TenantId }).FirstOrDefault();
                drawerIds = new List<Guid?>();
                drawerIds.Add(drawer.Id);
            }
            if (drawerIds != null)
            {
                var entities = _invoiceRepository.GetAll().Where(x => drawerIds.Any(d => d.Value == x.DrawerId));
                if (searchInput.ClientId != null)
                    entities = entities.Where(a => a.ClientId == searchInput.ClientId);

                if (searchInput.InvoiceId != null)
                    entities = entities.Where(a => a.Id == searchInput.InvoiceId);

                if (searchInput.Status != null)
                    entities = entities.Where(a => a.Status == searchInput.Status);

                if (searchInput.ExternalReferenceNumber != null)
                    entities = entities.Where(a => a.ExternalReferenceNumber == searchInput.ExternalReferenceNumber);

                if (searchInput.StartDueDate != null)
                    entities = entities.Where(a => a.DueDate >= searchInput.StartDueDate);

                if (searchInput.EndDueDate != null)
                {
                    DateTime endDueDate = ((DateTime)searchInput.EndDueDate).AddDays(1);
                    entities = entities.Where(a => a.DueDate < endDueDate);
                }

                if (searchInput.TipoFirma != null)
                    entities = entities.Where(a => a.TipoFirma == searchInput.TipoFirma);

                if (searchInput.EstatusFirma != null)
                    entities = entities.Where(a => a.StatusFirmaDigital == searchInput.EstatusFirma);

                if ((searchInput.StartDueDate == null) && (searchInput.EndDueDate == null) && (searchInput.ClientId == null) && (searchInput.InvoiceId == null))
                {
                    var minDate = DateTimeZone.Now().AddMonths(-2);
                    entities = entities.Where(a => a.DueDate >= minDate);
                }

                List<InvoiceApiDto> posSearchEntities = null;
                if (searchInput.IsPOSSearch == null || (searchInput.IsPOSSearch != null && !((bool)searchInput.IsPOSSearch)))
                {
                    entities = entities.Include(a => a.Client).Include(a => a.Notes).Include(a => a.InvoiceLines).Include(a => a.InvoiceHistoryStatuses).Include(a => a.InvoicePaymentTypes);
                }
                else if (searchInput.IsPOSSearch != null && (bool)searchInput.IsPOSSearch)
                {
                    entities = entities.Include(a => a.Client);
                    posSearchEntities = entities.Select(i => new InvoiceApiDto {
                        DueDate = i.DueDate,
                        Client = (i.Client != null) ? new ClientDto { IdentificationType = i.Client.IdentificationType, Identification = i.Client.Identification, IdentificacionExtranjero = i.Client.IdentificacionExtranjero, Name = i.Client.Name, LastName = i.Client.LastName } : null,
                        ClientName = i.ClientName,
                        ClientIdentification = i.ClientIdentification,
                        ConsecutiveNumber = i.ConsecutiveNumber,
                        VoucherKey = i.VoucherKey,
                        CodigoMoneda = i.CodigoMoneda,
                        TenantName = i.Tenant.Name}).ToList();

                    return new ListResultDto<InvoiceApiDto>(posSearchEntities);
                }

                if (entities == null)
                {
                    throw new UserFriendlyException("Could not found the invoices, maybe it's deleted.");
                }

                var list = entities.ToList();
                return new ListResultDto<InvoiceApiDto>(list.MapTo<List<InvoiceApiDto>>());
            }
            else
            {
                throw new UserFriendlyException("The tenant doesn't have any Drawers.");
            }
           
        }

        public ListResultDto<NoteApiDto> SearchNotesApi(SearchNotesApi searchInput)
        {
            Drawer drawer = _drawersAppService.getUserDrawersOpen();
            if (drawer == null)
            {
                drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = (int)AbpSession.TenantId }).FirstOrDefault();
            }
            if (drawer != null)
            {
                var predicate = PredicateBuilder.New<Note>(true).And(x => x.DrawerId == drawer.Id);

                if (searchInput.InvoiceId != null)
                {
                    predicate = predicate.And(d => d.InvoiceId == searchInput.InvoiceId);
                }

                if (searchInput.NoteId != null)
                {
                    predicate = predicate.And(d => d.Id == searchInput.NoteId);
                }

                if (searchInput.Status != null)
                {
                    predicate = predicate.And(d => d.StatusTribunet == searchInput.Status);
                }

                if (searchInput.StartDueDate != null)
                {
                    predicate = predicate.And(d => d.CreationTime >= searchInput.StartDueDate);
                }

                if (searchInput.EndDueDate != null)
                {
                    DateTime endDueDate = ((DateTime)searchInput.EndDueDate).AddDays(1);
                    predicate = predicate.And(d => d.CreationTime < endDueDate);
                }

                if (searchInput.TipoFirma != null)
                {
                    predicate = predicate.And(d => d.TipoFirma == searchInput.TipoFirma);
                }

                if (searchInput.EstatusFirma != null)
                {
                    predicate = predicate.And(d => d.StatusFirmaDigital == searchInput.EstatusFirma);
                }

                if ((searchInput.StartDueDate == null) && (searchInput.EndDueDate == null) && (searchInput.NoteId == null) && (searchInput.InvoiceId == null))
                {
                    var minDate = DateTimeZone.Now().AddMonths(-2);
                    predicate = predicate.And(d => d.CreationTime >= minDate);
                }

                var Notes = _noteRepository.GetAll().AsExpandable().Where(predicate);

                if (Notes == null)
                {
                    throw new UserFriendlyException("Could not found the notes, maybe it's deleted.");
                }

            var list = Notes.ToList().OrderBy(x => x.ConsecutiveNumber);
                return new ListResultDto<NoteApiDto>(list.MapTo<List<NoteApiDto>>());
            }
            else
            {
                throw new UserFriendlyException("The tenant doesn't have any Drawers.");
            }

            
        }

        public IList<InvoiceDto> SearchInvoicesPending(string identification, long tenantId)
        {
            var client = _invoiceManager.GetClientToIdentification(identification, tenantId);
            if (client.TenantId != tenantId)
                throw new UserFriendlyException("No se ha encontrado ningún cliente con este numero de identificación");
            var @entities = _invoiceRepository.GetAll().Where(a => a.ClientId == client.Id && a.Status == Status.Parked);
            @entities = @entities.Include(a => a.Notes).Include(a => a.InvoiceLines).Include(a => a.Client).Include(a => a.InvoiceHistoryStatuses);

            if (@entities == null)
            {
                throw new UserFriendlyException("Could not found the invoices, maybe it's deleted.");
            }

            return @entities.MapTo<List<InvoiceDto>>().ToList();
        }

        public IList<InvoiceLineDto> GetAllListInvoiceLines(Guid invoiceId)
        {
            return _invoiceManager.GetInvoiceLines(invoiceId).MapTo<List<InvoiceLineDto>>();
        }

        public Client GetClientPdf(Guid ClienteId)
        {

            return _clientManager.Get(ClienteId);
        }

        public IList<Moneda> GetAllListMoney()
        {

            return _invoiceManager.GetAllMoney();

        }

        public string GetTenant()
        {
            var @tenant = _invoiceManager.GetTenant(AbpSession.TenantId.Value);
            if (@tenant == null)
            {
                throw new UserFriendlyException("Could not found the tenant, maybe it's deleted.");
            }
            return @tenant.Name;
        }

        public void DeleteNotes(Guid invoiceId, Guid noteId)
        {
            _invoiceManager.DeleteNotes(invoiceId, noteId);
        }

        public IPagedList<Client> SearchClients(string query, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value : 1;

            int defaultPageSize = 12;
            if (query == null)
                query = "";

            var @entities = _invoiceManager.SearchClients(query);
            if (@entities == null)
            {
                throw new UserFriendlyException("Could not found the clients, maybe it's deleted.");
            }

            return @entities.MapTo<List<Client>>().ToPagedList(currentPageIndex, defaultPageSize);
        }

        public IList<Service> GetAllServices()
        {
            return _invoiceManager.GetAllServices();
        }

        public IList<Bank> GetAllBanks()
        {
            return _bankRepository.GetAll().ToList();
        }

        [UnitOfWork]
        public void PayInvoice(int typePayment, decimal balance, string trans, Invoice invoice, decimal balanceDebit, Guid? bankId, string userCard)
        {
            //var invoice = _invoiceRepository.Get(invoiceId);


            if (invoice == null)
                throw new UserFriendlyException("Could not found the invoice, maybe it's deleted.");

            decimal montoapplyInvoice = 0;
            var notes = invoice.Notes.Where(x => x.NoteType == NoteType.Debito && x.Status == Status.Parked && x.ConsecutiveNumberReference == null);

            PaymetnMethodType temp = (PaymetnMethodType)typePayment;

            //monto total por aplicar a la factura
            montoapplyInvoice = balance > (invoice.Balance - balanceDebit) ? (invoice.Balance - balanceDebit) : balance;

            //utiliza los pagos que han sido reversados
            if (temp == PaymetnMethodType.PositiveBalance)
            {
                if (invoice.ClientId != null)
                {
                    var paymentList = GetPaymentReverse(invoice.ClientId.Value, invoice.CodigoMoneda);

                    foreach (var paymentItem in paymentList)
                    {

                        //PaymentInvoice paymentInvoice1 = _paymentinvoiceRepository.Get(paymentItem.Id);
                        //paymentItem.ParentPaymentInvoiceId = paymentItem.Invoice.Id;
                        if (montoapplyInvoice > 0)
                        {
                            var montoapply = paymentItem.Balance > montoapplyInvoice ? montoapplyInvoice : paymentItem.Balance;
                            paymentItem.AssignInvoice(invoice.TenantId, montoapply, invoice, temp, bankId, userCard);
                            montoapplyInvoice -= montoapply;

                            // actualiza la factura
                            invoice.Balance -= montoapply;
                        }
                        // si el pago queda con saldo positivo, se aplica a las notas
                        if (paymentItem.Balance > 0)
                        {
                            foreach (var item in notes)
                            {
                                var montoapplynote = paymentItem.Balance > (item.Balance) ? (item.Balance) : paymentItem.Balance;

                                paymentItem.AssignNote(item.TenantId, montoapplynote, item, temp);

                                item.Balance -= montoapplynote;

                                // actualiza la factura
                                invoice.Balance -= montoapplynote;

                                if (item.Balance == 0)
                                    item.SetStatus(Status.Completed);

                                _noteRepository.Update(item);
                                if (paymentItem.Balance == 0)
                                    break;
                            }
                        }

                        paymentItem.IsPaymentReversed = false;
                        paymentItem.IsPaymentUsed = true;
                        _paymentRepository.Update(paymentItem);
                        notes = invoice.Notes.Where(x => x.NoteType == NoteType.Debito && x.Status == Status.Parked && x.ConsecutiveNumberReference == null);
                    }
                }
            }
            else
            {
                // se crea el pago
                Client client = null;
                if (invoice.ClientId != null)
                    client = _clientManager.Get(invoice.ClientId.Value);

                var payment = Payment.Create(invoice.TenantId, client, balance, PaymentType.Payment, "Pago por TicoPay", invoice.CodigoMoneda, PaymentOrigin.ticopay);
                payment.SetPaymetnMethodType(temp);
                payment.SetTransaction(trans);

                // si la factura tiene un saldo pendiente por cancelar se le aplica el pago
                if (montoapplyInvoice > 0)
                {
                    //Asigna el pago de la factura
                    payment.AssignInvoice(invoice.TenantId, montoapplyInvoice, invoice, temp, bankId, userCard);

                    //actualiza el usuario que realiza el pago
            var user = _invoiceManager.GetUser(AbpSession.UserId.Value);
            invoice.SetUserNamePayment(user.UserName);

                    // actualiza la factura
                    invoice.Balance -= montoapplyInvoice;

                }

                // si el pago queda con saldo pendiente se aplia a las notas de debito pendientes por pagar 
                if (payment.Balance > 0)
                {

                    foreach (var item in notes)
                    {
                        var montoapplynote = payment.Balance > (item.Balance) ? (item.Balance) : payment.Balance;

                        payment.AssignNote(item.TenantId, montoapplynote, item, temp);

                        item.Balance -= montoapplynote;
                        // actualiza la factura

                        invoice.Balance -= montoapplynote;

                        if (item.Balance == 0)
                            item.SetStatus(Status.Completed);

                        _noteRepository.Update(item);
                        if (payment.Balance == 0)
                            break;
                    }
                }



            // inserta en la tabla PaymentInvoice
                _paymentRepository.Insert(payment);

            }


            if (invoice.Balance == 0)
            invoice.SetStatus(Status.Completed);

            _invoiceRepository.Update(@invoice);


        }

        [UnitOfWork]
        public void PayInvoiceList(List<PaymentInvoceDto> listPaymetnInvoce, Guid invoiceId)
        {
            //var invoice = _invoiceRepository.Get(invoiceId);
            var invoice = _invoiceRepository.GetAll().Where(X => X.Id == invoiceId).Include(a => a.Notes).FirstOrDefault();
            var balanceDebit = invoice.Notes.Where(x => x.NoteType == NoteType.Debito && x.Status == Status.Parked && x.ConsecutiveNumberReference == null).Sum(y => y.Total);

            if (invoice == null)
                throw new UserFriendlyException("Could not found the invoice, maybe it's deleted.");

            foreach (PaymentInvoceDto paymetnInvoce in listPaymetnInvoce)
            {
                PayInvoice(paymetnInvoce.TypePayment, paymetnInvoce.Balance, paymetnInvoce.Trans, invoice, balanceDebit, paymetnInvoce.BankId, paymetnInvoce.UserCard);
                        }
                    }



        /// <summary>
        /// Reversar factura banco - banco
        /// </summary>
        /// <param name="factura"></param>
        /// <param name="reference"></param>
        [UnitOfWork]
        public void ReverseInvoice(InvoicePendingPayBN factura, string reference) // chequear payment
        {
            var paymentinvoice = _paymentinvoiceRepository.Get(factura.PaymentId.Value);

            var payment = paymentinvoice.Payment;
            payment.Reference = reference;

            _paymentinvoiceRepository.Update(paymentinvoice); // actualiza la referencia
            _paymentRepository.Update(payment);

            _paymentRepository.Delete(payment);// reversa el pago
            _paymentinvoiceRepository.Delete(factura.PaymentId.Value);
        }

        /// <summary>
        /// Pagar factura BN - banco
        /// </summary>
        /// <param name="factura"></param>
        /// <param name="codigoAgencia"></param>
        /// <param name="trama"></param>
        /// <returns></returns>
        [UnitOfWork]
        public int PayInvoiceBn(InvoicePendingPayBN factura, int codigoAgencia, string trama) // chequear payment
        {

            var invoice = _invoiceRepository.Get(factura.Id);

            var aplicacionPago = AplicacionPago.ParserAplicacionPago(trama);

            if ((invoice == null) || (aplicacionPago == null))
                return -1;

            // se obtine el balance de las notas de debitos pendientes
            var balanceDebit = invoice.Notes.Where(x => x.NoteType == NoteType.Debito && x.Status == Status.Parked && x.ConsecutiveNumberReference == null).Sum(y => y.Total);

            var payment = Payment.Create(invoice.TenantId, invoice.Client, invoice.Balance, PaymentType.Payment, "Pago por Banco Nacional", invoice.CodigoMoneda, PaymentOrigin.BNpay);

            //var paymentInvoice = PaymentInvoice.Create(invoice.TenantId, invoice.Balance, invoice, PaymentInvoiceType.Payment, "Pago por Banco Nacional", invoice.CodigoMoneda);

            if (aplicacionPago.NumeroCheque != 0)
                payment.SetPaymetnMethodType(PaymetnMethodType.Check);    //paymentInvoice.PayInvoiceCheck();
            else
                payment.SetPaymetnMethodType(PaymetnMethodType.Deposit);    //paymentInvoice.PayInvoiceDeposit();



            var pay = (PaymetnMethodType)payment.PaymetnMethodType;

            // se calcula el monto a a plicar a la factura
            var montoapplyInvoice = invoice.Balance - balanceDebit;
            payment.AssignInvoice(invoice.TenantId, montoapplyInvoice, invoice, payment.PaymetnMethodType, null, null);

            // si el pago queda con saldo pendiente se aplia a las notas de debito pendientes por pagar 
            if (payment.Balance > 0)
            {
                var notes = invoice.Notes.Where(x => x.NoteType == NoteType.Debito && x.Status == Status.Parked && x.ConsecutiveNumberReference == null);

                foreach (var item in notes)
                {
                    var montoapplynote = payment.Balance > (item.Balance) ? (item.Balance) : payment.Balance;

                    payment.AssignNote(item.TenantId, montoapplynote, item, payment.PaymetnMethodType);

                    item.Balance -= montoapplynote;

                    if (item.Balance == 0)
                        item.SetStatus(Status.Completed);

                    _noteRepository.Update(item);
                    if (payment.Balance == 0)
                        break;
                }
            }


            payment.CodigoAgencia = codigoAgencia;
            payment.CodigoBanco = aplicacionPago.CodigoBanco;
            payment.CodigoBancoEmisor = aplicacionPago.CodigoBancoEmisor;
            payment.CodigoTransaccion = aplicacionPago.CodigoTransaccion;
            payment.ConsecutivoTransaccion = aplicacionPago.ConsecutivoTransaccion;
            payment.NotaCredito = aplicacionPago.NotaCredito;
            payment.NumeroCheque = aplicacionPago.NumeroCheque;
            payment.NumeroCuenta = aplicacionPago.NumeroCuenta;
            payment.SetTransaction(aplicacionPago.ConsecutivoTransaccion.ToString());

            _paymentRepository.Insert(payment);

            // actualiza la factura
            try
            {
                var user = _userAppService.GetUserByRole(userPayBn);
                invoice.SetUserNamePayment(user.UserName);
            }
            catch (Exception)
            {
                invoice.SetUserNamePayment("Banco Nacional");
            }
            invoice.SetStatus(Status.Completed);
            invoice.Balance = 0;
            _invoiceRepository.Update(invoice);

            return 0;
        }

        public void VoidInvoice(Guid invoiceId)
        {
            var @invoice = _invoiceRepository.Get(invoiceId);
            if (@invoice == null)
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");

            @invoice.SetStatus(Status.Voided);
            _invoiceRepository.Update(@invoice);
        }

        public Service GetService(Guid serviceId)
        {
            var service = _invoiceManager.GetServiceSoftDelete(serviceId);
            return service;
        }

        public ClientDto GetClient(Guid clientId)
        {
            var client = _clientRepository.Get(clientId);
            return Mapper.Map<ClientDto>(client);
        }

        public ClientDto GetClient(string identification, long tenantId)
        {
            var client = _invoiceManager.GetClientToIdentification(identification, tenantId);
            if (client.TenantId != tenantId)
                throw new UserFriendlyException("No se ha encontrado ningún cliente con este numero de identificación");
            return Mapper.Map<ClientDto>(client);
        }

        public IPagedList<Register> SearchRegisters(string query, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            int defaultPageSize = 12;
            if (query == null)
                query = "";

            var entities = _invoiceManager.SearchRegisters(query);
            if (entities == null)
            {
                throw new UserFriendlyException("No se encontraron cajas con esas caracteristicas.");
            }

            return entities.MapTo<List<Register>>().ToPagedList(currentPageIndex, defaultPageSize);
        }

        [UnitOfWork(isTransactional: false)]
        public async void CreateGroupConceptsInvoices(GroupConcepts groupConcepts)
        {
            if (groupConcepts != null && groupConcepts.CanCreateInvoice())
            {
                if (groupConcepts.Services == null || groupConcepts.Services.Count == 0)
                {
                    Logger.WarnFormat("El grupo de servicio {0} no tiene servicios asignados.", groupConcepts.Name);
                    return;
                }
                var allBillingGroupConcepts = _groupConceptsManager.GetAllBillingGroupConcepts(groupConcepts);

                if (allBillingGroupConcepts == null || allBillingGroupConcepts.Count == 0)
                {
                    Logger.WarnFormat("El grupo de servicio {0} no tiene clientes asignados.", groupConcepts.Name);
                    return;
                }
                var tenant = _tenantManager.Get(groupConcepts.TenantId);

                var drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = tenant.Id }).FirstOrDefault();
                var register = _invoiceManager.GetRegistersbyDrawer(drawer.Id);
               
                int invoicesMonthlyLimit = GetInvoicesMonthlyLimitAsync(tenant.EditionId.Value).Result;
                //using (var unitOfWork = _unitOfWorkManager.Begin())
                //{
                    var billed = false;
                    foreach (var item in allBillingGroupConcepts)
                    {
                    if (item.Quantity > 0 && item.DiscountPercentage < 100)
                    {
                        Client client = item.Client;
                        if (!await TenantCanCreateInvoices(tenant, invoicesMonthlyLimit))
                            continue;

                        var invoice = CreateInvoice(tenant, register, client, item, "", drawer);
                        if (invoice != null)
                        {
                            //_unitOfWorkManager.Current.SaveChanges();
                            billed = true;
                            Logger.InfoFormat("Se ha creado con éxito la factura {0} del grupo de servicios {1} para el cliente {2} {3}", invoice.Number, groupConcepts.Name, client.Name, client.LastName);
                        }
                        else
                        {
                            Logger.WarnFormat("No se pudo crear la factura del grupo de servicios {0} para el cliente {1} {2}.", groupConcepts.Name, client.Name, client.LastName);
                        }
                    }

                }

                    using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                    {
                        Scope = TransactionScopeOption.RequiresNew,
                        IsTransactional = true,
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
                    {
                    groupConcepts.SetNewEjecutionDates();
                        _unitOfWorkManager.Current.SaveChanges();
                    unitOfWork.Complete();
                    }
                    //unitOfWork.Complete();
                    if ((billed) && (tenant.FirmaRecurrente == FirmType.Firma))
                    {
                        // Enviar Correo Electronico 
                        mail.SendMailTicoPay(tenant.Email, subject, String.Format(emailbodyRecurren, "Grupos de Servicios", emailSteps), emailfooter, "", tenant.AlternativeEmail, tenant.ComercialName, cuentaBanca);
                    }
                //}
                }
            }

        [UnitOfWork(isTransactional: false)]
        private Invoice CreateInvoice(Tenant tenant, Register register, Client client, ClientGroupConcept groupConcepts, string note, Drawer drawer)
        {
            if (tenant == null || client == null || groupConcepts == null || register == null)
            {
                return null;
            }

            Certificate certified = null;
            

            ValidateTribunet validateHacienda = new ValidateTribunet();

            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);
            Guid idinvoice;
            List<XSD.FacturaElectronicaMedioPago> listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();
            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.RequiresNew,
                IsTransactional = true,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
            Invoice invoice = Invoice.Create(tenant.Id, note, DateTimeZone.Now(), client.Id, tenant, tenant.ConditionSaleType, tenant.CodigoMoneda);



            invoice = headerClient(invoice, client);
            int numberline = 1;
            decimal taxamount = 0, totalgravado = 0, totalexento = 0, totalDiscount = 0;


            foreach (Service svr in groupConcepts.Group.Services)
            {
                var service = _serviceManager.Get(svr.Id);
                if (service.Price <= 0)
                    continue;


                decimal subtotal = (service.Price * groupConcepts.Quantity);
                decimal discountamount = Decimal.Round((groupConcepts.DiscountPercentage * subtotal) / 100, 2, MidpointRounding.AwayFromZero);
                decimal tax = Decimal.Round((service.Tax.Rate * (subtotal - discountamount)) / 100, 2, MidpointRounding.AwayFromZero);

                //decimal tax = Decimal.Round((service.Tax.Rate * (service.Price * groupConcepts.Quantity)) / 100,2, MidpointRounding.AwayFromZero);
               // decimal discountamount = Decimal.Round((groupConcepts.DiscountPercentage * (service.Price * groupConcepts.Quantity)) / 100,2, MidpointRounding.AwayFromZero);
                string title = service.Name;

                if (title.Length > MaxTitleLength)
                    title = service.Name.Substring(0, MaxTitleLength - 1);

                invoice.AssignInvoiceLine(service.TenantId, service.Price, tax, groupConcepts.DiscountPercentage, "", "", title, groupConcepts.Quantity, LineType.Service, service, null, invoice, numberline++, service.Tax, service.TaxId, service.UnitMeasurement, service.UnitMeasurementOthers);

                if (tax > 0)
                    totalgravado = Decimal.Round(totalgravado + subtotal, 2, MidpointRounding.AwayFromZero);
                else
                    totalexento = Decimal.Round(totalexento + subtotal, 2, MidpointRounding.AwayFromZero);

                taxamount = Decimal.Round(taxamount + tax, 2, MidpointRounding.AwayFromZero);
                totalDiscount = Decimal.Round(totalDiscount + discountamount, 2, MidpointRounding.AwayFromZero);
            }

                invoice.SetInvoiceTotalCalculate(taxamount, totalDiscount, totalgravado, totalexento, 0, 0);

                listMedioPago = new List<XSD.FacturaElectronicaMedioPago>();
            listMedioPago.Add(XSD.FacturaElectronicaMedioPago.Efectivo);

            if (tenant.ValidateHacienda)
            {
                @invoice.TipoFirma = tenant.FirmaRecurrente;
                if (tenant.FirmaRecurrente == FirmType.Firma)
                    @invoice.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
            }


                // calculo de consecutivo
                BeginPostCalculateInvoicesOperations(register, tenant, invoice);
                // se actualiza
                @invoice.SetInvoiceConsecutivo(register, tenant, false, drawer);

                //invoice.SetInvoiceConsecutivo(register, tenant);
                //invoice.SetInvoiceNumberKey(invoice.DueDate, invoice.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);

                invoice.SendInvoice = false;

               /// invoice.SetInvoiceNumber(_invoiceManager.CheckInvoiceNumber());

                _invoiceManager.Create(invoice);

                _unitOfWorkManager.Current.SaveChanges();


                // crear XML 
                certified = BeginCreateAndUploadXMlAndPDF(tenant, certified, invoice, client, listMedioPago, null, null);


                unitOfWork.Complete();

                idinvoice = invoice.Id;


            }

            Invoice @invoiceNew = _invoiceManager.Get(idinvoice);
            string voucherk = @invoiceNew.VoucherKey;

            if ((tenant.ValidateHacienda) && (tenant.FirmaRecurrente == FirmType.Llave))
            {

                //enviar factura a hacienda
                string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, @invoiceNew, @invoiceNew.VoucherKey, null, null);

                if (validate == "-1")
                {
                    @invoiceNew.StatusVoucher = VoucherSituation.withoutInternet;
                    int _vouchersituation = (int)VoucherSituation.withoutInternet;
                    @invoiceNew.VoucherKey = @invoiceNew.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + @invoiceNew.VoucherKey.Substring(42, 8);
                    @invoiceNew.SendInvoice = false;
                }
                else
                {
                    @invoiceNew.StatusVoucher = VoucherSituation.Normal;
                    @invoiceNew.SendInvoice = true;
                }

            }

            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (tenant.FirmaRecurrente == FirmType.Llave)))
            {

                if ((client != null) && (!String.IsNullOrWhiteSpace(client.Email)))
                {
                    //enviar correo
                    mail.SendMailTicoPay(client.Email, subject, string.Format(emailbody, @invoiceNew.TypeDocument.GetDescription(), @invoiceNew.TypeDocument.GetDescription()) + BuildConfirmationButton(@invoiceNew), emailfooter, "",
                    Path.Combine(WorkPaths.GetXmlPath(), voucherk + ".xml"),
                    Path.Combine(WorkPaths.GetPdfPath(), voucherk + ".pdf"),
                    Path.Combine(WorkPaths.GetQRPath(), voucherk + ".png"),
                    Path.Combine(WorkPaths.GetXmlSignedPath(), voucherk + ".xml"), tenant.AlternativeEmail,
                    tenant.ComercialName, "", (client != null) ? client.ContactEmail : string.Empty);
                }

                if (tenant.SmsNoficicarFacturaACobro && ticoTalkClient.IsAuthenticated)
                {
                    ticoTalkClient.SendSms(tenant.IdentificationNumber, client.MobilNumber, BuildSmsMessage(tenant.Name, tenant.CodigoMoneda.ToString(), @invoiceNew.Number, @invoiceNew.Total));
                }
            }

            return @invoiceNew;
        }

        public Task<int> GetTotalInvoicesInMonthAsync(int tenantId)
        {
            var actualDate = DateTimeZone.Now();
            DateTime startDate = new DateTime(actualDate.Year, actualDate.Month, 1);
            DateTime endDate = new DateTime(actualDate.Year, actualDate.Month, DateTime.DaysInMonth(actualDate.Year, actualDate.Month), 23, 59, 59);
            int totalInvoicesInMonth = _invoiceRepository
                .GetAll()
                .Count(i => !i.IsDeleted && i.TenantId == tenantId && i.DueDate >= startDate && i.DueDate <= endDate);

            return Task.FromResult(totalInvoicesInMonth);
        }

        public async Task<int> GetInvoicesMonthlyLimitAsync(int editionId)
        {
            int invoicesMonthlyLimit = 0;
            if (editionId > 0)
            {
                var invoicesMonthlyLimitStr = await _editionManager.GetFeatureValueOrNullAsync(editionId, "InvoicesMonthlyLimit");
                int.TryParse(invoicesMonthlyLimitStr, out invoicesMonthlyLimit);
            }
            return invoicesMonthlyLimit;
        }

        private string GetInvoicesMonthlyLimitEmailBody(string clientName)
        {
            StringBuilder body = new StringBuilder();
            body.Append("<p><b>Hola</b> " + clientName + "</p>");
            body.Append("<br/>");
            body.Append("<p>" + L("InvoicesMonthlyLimitMessage") + "</p>");
            body.Append("<br/>");
            body.Append("<p>" + emailfooter + "</p>");
            return body.ToString();
        }

        private string GetNearInvoicesMonthlyLimitEmailBody(string clientName)
        {
            StringBuilder body = new StringBuilder();
            body.Append("<p><b>Hola</b> " + clientName + "</p>");
            body.Append("<br/>");
            body.Append("<p>" + L("NearInvoicesMonthlyLimitMessage") + "</p>");
            body.Append("<br/>");
            body.Append("<p>" + emailfooter + "</p>");
            return body.ToString();
        }

        public void ResendFailedInvoices(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            ValidateTribunet validateTribunet = new ValidateTribunet();
            TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet, pss).Result;
            var notSendedInvoices = _invoiceRepository
                .GetAll()
                .Where(i => !i.SendInvoice && !i.IsDeleted && i.ElectronicBill != null && i.TenantId == tenant.Id && ((i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Firmada) || (i.TipoFirma == FirmType.Llave && i.StatusFirmaDigital == null)))
                .Include(i => i.Tenant)
                .Include(i => i.Client)
                .ToList();
            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
                if (notSendedInvoices != null && notSendedInvoices.Count > 0)
                {
                    if (tokenResponse != null && !tokenResponse.IsError)
                    {
                        foreach (var invoice in notSendedInvoices)
                        {
                            var validUntil = DateTimeZone.Now().AddSeconds(tokenResponse.ExpiresIn);
                            if (DateTimeZone.Now() >= validUntil)
                            {
                                tokenResponse = ValidateTribunet.RefreshToken(tokenResponse.RefreshToken);
                            }
                            bool success = validateTribunet.ResendInvoice(invoice, invoice.Client, invoice.DueDate, invoice.Tenant, tokenResponse.AccessToken);
                            if (success)
                            {
                                invoice.SendInvoice = true;
                                _unitOfWorkManager.Current.SaveChanges();
                                // _invoiceRepository.Update(invoice);
                            }
                            else
                            {
                                ElectronicBillResponse response = validateTribunet.GetComprobanteStatusFromTaxAdministrationRefreshToken(invoice, tokenResponse.RefreshToken);

                                if (!(response == null || (response.IndEstado != "procesando" && response.IndEstado != "aceptado" && response.IndEstado != "rechazado")))
                                {
                                    invoice.StatusVoucher = VoucherSituation.Normal;
                                    invoice.SendInvoice = true;
                                }
                            }
                        }
                    }
                }
                unitOfWork.Complete();
            }
        }

        public void CheckSaveIssuesInAzure(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            var notSaveInAzure = _invoiceRepository
                .GetAll()
                .Where(i => i.SendInvoice && !i.IsDeleted && (!i.SavedInvoiceOrTicketXML || !i.SavedInvoiceOrTicketPDF) && i.TenantId == tenant.Id && ((i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Firmada) || (i.TipoFirma == FirmType.Llave && i.StatusFirmaDigital == null)))
                .Include(i => i.Tenant)
                .Include(i => i.Client)
                .ToList();
            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
                if (notSaveInAzure != null && notSaveInAzure.Count > 0)
                {
                    foreach (var invoice in notSaveInAzure)
                    {
                        Uri XML, PDF;
                        if (!invoice.SavedInvoiceOrTicketXML)
                        {
                            try
                            {
                                XML = SaveAzureStorage(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), "xmlinvoice");

                                if (XML != null)
                                {
                                    invoice.ElectronicBill = XML.ToString();
                                    invoice.SavedInvoiceOrTicketXML = true;
                                }
                            }
                            catch
                            {
                            }
                        }

                        if (!invoice.SavedInvoiceOrTicketPDF)
                        {
                            try
                            {
                                PDF = SaveAzureStorage(invoice.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), "pdfinvoice");

                                if (PDF != null)
                                {
                                    invoice.ElectronicBillPDF = PDF.ToString();
                                    invoice.SavedInvoiceOrTicketPDF = true;
                                }
                            }
                            catch
                            {
                            }
                        }

                        _unitOfWorkManager.Current.SaveChanges();
                    }
                    unitOfWork.Complete();
                }
            }
        }

        public void ResendFailedInvoicesRepair(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            var notSendedInvoices = _invoiceRepository
                .GetAll()
                .Where(i => !i.SendInvoice && !i.IsDeleted && i.ElectronicBill == null && i.TenantId == tenant.Id
                && (i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Pendiente) && (i.Number >= 10113 && i.Number <= 10161))
                .Select(i => i.Id)
                .ToList();
            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
                if (notSendedInvoices != null && notSendedInvoices.Count > 0)
                {
                    foreach (var InvoiceID in notSendedInvoices)
                    {
                        Invoice invoice = _invoiceRepository.GetAll().Where(x => x.Id == InvoiceID).Include("Client").Include("InvoicePaymentTypes").Include("InvoiceLines").Include("InvoiceLines.Tax").Include("InvoicePaymentTypes.Payment").FirstOrDefault();

                        invoice.TipoFirma = FirmType.Llave;

                        invoice.StatusFirmaDigital = null;

                        SetInvoiceWithInternetAux(ref invoice);

                        Certificate certified = null;

                        certified = _tenantAppService.GetCertified(tenant.Id);

                        Uri XML, PDF;

                        GenerateAndUploadXMLAndPdf(tenant, certified, invoice, invoice.Client, null, null, null, out XML, out PDF);

                        @invoice.SetInvoiceXML(XML, PDF);

                        ValidateTribunet validateTribunet = new ValidateTribunet();
                        TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet, pss).Result;
                        if (tokenResponse != null && !tokenResponse.IsError)
                        {

                            bool success = validateTribunet.ResendInvoice(invoice, invoice.Client, invoice.DueDate, invoice.Tenant, tokenResponse.AccessToken);
                            if (success)
                            {
                                invoice.SendInvoice = true;
                            }


                        }
                        BeginSendMailAndTicoTalkSmsNotification(tenant, invoice, invoice.Client, invoice.VoucherKey);
                        _unitOfWorkManager.Current.SaveChanges();

                    }
                }
                unitOfWork.Complete();
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableValidation]
        public void SyncsInvoicesWithTaxAdministration(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            var pendingInvoices = _invoiceRepository
                .GetAll()
                .Where(i => i.SendInvoice && !i.IsDeleted && i.TenantId == tenant.Id
                && (i.StatusTribunet == StatusTaxAdministration.NoEnviada || i.StatusTribunet == StatusTaxAdministration.Recibido || i.StatusTribunet == StatusTaxAdministration.Procesando)
                && ((i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Firmada) || (i.TipoFirma == FirmType.Llave && i.StatusFirmaDigital == null))
                || (i.TenantId == tenant.Id && (i.StatusTribunet == StatusTaxAdministration.Aceptado || i.StatusTribunet == StatusTaxAdministration.Rechazado) && (i.Notes.Any(s => s.SendInvoice && s.StatusTribunet != StatusTaxAdministration.Aceptado && s.StatusTribunet != StatusTaxAdministration.Rechazado && s.StatusTribunet != StatusTaxAdministration.Error) && i.CreationTime.Month >= 4 && i.CreationTime.Year >= 2018)))
                .Include(i => i.Tenant)
                .Include(i => i.Client)
                .ToList();

            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {

                if (pendingInvoices != null && pendingInvoices.Count > 0)
                {
                    ValidateTribunet validateTribunet = new ValidateTribunet();
                    TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet, pss).Result;
                    if (tokenResponse != null && !tokenResponse.IsError)
                    {
                        foreach (var invoice in pendingInvoices)
                        {
                            var currentInvoice = _invoiceManager.Get(invoice.Id);
                            ElectronicBillResponse response = null;
                            #region Se busca la respuesta de la factura
                            if (invoice.StatusTribunet != StatusTaxAdministration.Aceptado && invoice.StatusTribunet != StatusTaxAdministration.Rechazado && invoice.StatusTribunet != StatusTaxAdministration.Error)
                            {
                                response = validateTribunet.GetComprobanteStatusFromTaxAdministration(currentInvoice, tokenResponse.AccessToken);
                                if (response != null)
                                {
                                    if (response.IndEstado.ToLower() == "aceptado")
                                        currentInvoice.StatusTribunet = StatusTaxAdministration.Aceptado;
                                    else if (response.IndEstado.ToLower() == "rechazado")
                                        currentInvoice.StatusTribunet = StatusTaxAdministration.Rechazado;
                                    else if (response.IndEstado.ToLower() == "error")
                                        currentInvoice.StatusTribunet = StatusTaxAdministration.Error;

                                    currentInvoice.MessageTaxAdministration = response.RespuestaXml;

                                    var result = _invoiceRepository.Update(currentInvoice);

                                    string consecutiveNumber = invoice.ConsecutiveNumber;
                                    if (invoice.Tenant != null && !invoice.Tenant.ValidateHacienda)
                                        consecutiveNumber = Convert.ToInt64(invoice.ConsecutiveNumber.Substring(10, 10)).ToString();


                                    if ((currentInvoice.StatusTribunet == StatusTaxAdministration.Aceptado) && (invoice.ClientEmail != null))
                                        SendTaxAdministrationAcceptedInvoiceEmail(invoice.ClientName, invoice.VoucherKey, consecutiveNumber, invoice.ClientEmail, response.RespuestaXml, invoice.TypeDocument);
                                    else if (currentInvoice.StatusTribunet == StatusTaxAdministration.Rechazado)
                                        SendTaxAdministrationRejectedInvoiceEmail(invoice.Tenant.Name, invoice.VoucherKey, consecutiveNumber, new string[] { invoice.Tenant.Email, GetTenantUserAdminEmail(tenant.Id) }, response.RespuestaXml, invoice.TypeDocument);
                                    else if (currentInvoice.StatusTribunet == StatusTaxAdministration.Error)
                                        SendTaxAdministrationErrorInvoiceEmail(invoice.Tenant.Name, invoice.VoucherKey, consecutiveNumber, new string[] { invoice.Tenant.Email, GetTenantUserAdminEmail(tenant.Id) }, response.RespuestaXml, invoice.TypeDocument);


                                }
                            }
                            else if (invoice.StatusTribunet == StatusTaxAdministration.Aceptado || invoice.StatusTribunet == StatusTaxAdministration.Rechazado)
                            {
                                #region Se busca la respuesta de las notas de esa factura
                                foreach (var note in invoice.Notes.Where(i => i.SendInvoice && i.StatusTribunet != StatusTaxAdministration.Aceptado && i.StatusTribunet != StatusTaxAdministration.Rechazado && i.StatusTribunet != StatusTaxAdministration.Error && i.CreationTime.Month >= 4 && i.CreationTime.Year >= 2018))
                                {
                                    var currentNote = _noteRepository.Get(note.Id);

                                    response = validateTribunet.GetComprobanteStatusFromTaxAdministration(note, tokenResponse.AccessToken);
                                    if (response != null)
                                    {
                                        if (response.IndEstado.ToLower() == "aceptado")
                                            currentNote.StatusTribunet = StatusTaxAdministration.Aceptado;
                                        else if (response.IndEstado.ToLower() == "rechazado")
                                            currentNote.StatusTribunet = StatusTaxAdministration.Rechazado;
                                        else if (response.IndEstado.ToLower() == "error")
                                            currentNote.StatusTribunet = StatusTaxAdministration.Error;
                                    }

                                    currentNote.MessageTaxAdministration = response.RespuestaXml;
                                    var result = _noteRepository.Update(currentNote);

                                    string consecutiveNumber = note.ConsecutiveNumber;
                                    if (invoice.Tenant != null && !invoice.Tenant.ValidateHacienda)
                                        consecutiveNumber = Convert.ToInt64(note.ConsecutiveNumber.Substring(10, 10)).ToString();

                                    if ((currentNote.StatusTribunet == StatusTaxAdministration.Aceptado) && (note.Invoice.ClientEmail != null))
                                        SendTaxAdministrationAcceptedInvoiceEmail(invoice.ClientName, note.VoucherKey, consecutiveNumber, invoice.ClientEmail, response.RespuestaXml, (note.NoteType == NoteType.Credito) ? TypeDocumentInvoice.NotaCredito : TypeDocumentInvoice.NotaDebito);
                                    else if (currentNote.StatusTribunet == StatusTaxAdministration.Rechazado)
                                        SendTaxAdministrationRejectedInvoiceEmail(invoice.Tenant.Name, note.VoucherKey, consecutiveNumber, new string[] { invoice.Tenant.Email, GetTenantUserAdminEmail(tenant.Id) }, response.RespuestaXml, (note.NoteType == NoteType.Credito) ? TypeDocumentInvoice.NotaCredito : TypeDocumentInvoice.NotaDebito);
                                    else if (currentNote.StatusTribunet == StatusTaxAdministration.Error)
                                        SendTaxAdministrationErrorInvoiceEmail(invoice.Tenant.Name, note.VoucherKey, consecutiveNumber, new string[] { invoice.Tenant.Email, GetTenantUserAdminEmail(tenant.Id) }, response.RespuestaXml, (note.NoteType == NoteType.Credito) ? TypeDocumentInvoice.NotaCredito : TypeDocumentInvoice.NotaDebito);
                                }
                                #endregion
                            }
                            #endregion

                        }
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
                unitOfWork.Complete();
            }
        }

        private void SendTaxAdministrationErrorInvoiceEmail(string tenantName, string voucherKey, string invoiceNumber, string[] to, string respuestaXml, TypeDocumentInvoice documentType)
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("<p>");
                body.AppendFormat("<p>Estimado {0}!</p>", tenantName);
                body.AppendFormat("<p>Le informamos que su " + documentType.GetDescription() + " número {0} ha sido " + ((documentType != TypeDocumentInvoice.Ticket) ? "reportada" : " reportado") + " con error por parte de la administración tributaria, por favor verifique.</p>", invoiceNumber);
                body.Append("<p>Se adjunta el documento de respuesta enviado por la administración tributaria.</p>");
                body.Append("</p>");

                var encodedTextBytes = Convert.FromBase64String(respuestaXml);
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);

                string xmlPath = SaveXmlToFile("Respuesta_" + voucherKey + ".xml", plainText);
                SendMailTP sendMail = new SendMailTP();
                sendMail.SendNoReplyMail(to, "Error en " + documentType.GetDescription(), body.ToString(), new string[] { xmlPath });
            }
            catch (Exception)
            {
            }
        }

        private void SendTaxAdministrationRejectedInvoiceEmail(string tenantName, string voucherKey, string invoiceNumber, string[] to, string respuestaXml, TypeDocumentInvoice documentType)
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("<p>");
                body.AppendFormat("<p>Estimado {0}!</p>", tenantName);
                body.AppendFormat("<p>Le informamos que su " + documentType.GetDescription() + " número {0} ha sido " + ((documentType != TypeDocumentInvoice.Ticket) ? "rechazada" : "rechazado") + " por la administración tributaria" + ((documentType == TypeDocumentInvoice.Invoice) ? ", por favor genere la nota de crédito correspondiente" : "") + ".</p>", invoiceNumber);
                body.Append("<p>Se adjunta el documento de respuesta enviado por la administración tributaria.</p>");
                body.Append("</p>");

                var encodedTextBytes = Convert.FromBase64String(respuestaXml);
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);

                string xmlPath = SaveXmlToFile("Respuesta_" + voucherKey + ".xml", plainText);
                SendMailTP sendMail = new SendMailTP();
                sendMail.SendNoReplyMail(to, documentType.GetDescription() + ((documentType != TypeDocumentInvoice.Ticket) ? " Rechazada" : " Rechazado"), body.ToString(), new string[] { xmlPath });
            }
            catch (Exception)
            {
            }
        }

        private void SendTaxAdministrationAcceptedInvoiceEmail(string clientName, string voucherKey, string invoiceNumber, string to, string respuestaXml, TypeDocumentInvoice documentType)
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("<p>");
                body.AppendFormat("<p>Estimado {0}!</p>", clientName);
                body.AppendFormat("<p>Le informamos que su " + documentType.GetDescription() + " número {0} ha sido " + ((documentType != TypeDocumentInvoice.Ticket) ? "aceptada" : "aceptado") + " por la administración tributaria.</p>", invoiceNumber);
                body.Append("<p>Se adjunta el documento de respuesta enviado por la administración tributaria.</p>");
                body.Append("</p>");

                var encodedTextBytes = Convert.FromBase64String(respuestaXml);
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);

                string xmlPath = SaveXmlToFile("Respuesta_" + voucherKey + ".xml", plainText);
                SendMailTP sendMail = new SendMailTP();
                sendMail.SendNoReplyMail(new string[] { to }, documentType.GetDescription() + ((documentType != TypeDocumentInvoice.Ticket) ? " Aceptada" : " Aceptado"), body.ToString(), new string[] { xmlPath });
            }
            catch (Exception)
            {
            }
        }

        private string GetTenantUserAdminEmail(int tenantId)
        {
            string adminEmail = string.Empty;
            var admin = _userAppService.GetUserByRole("Admin", tenantId);
                if (admin != null)
                {
                    adminEmail = admin.EmailAddress;
                }
            return adminEmail;
        }

        private string SaveXmlToFile(string fileName, string xml)
        {
            string path = Path.Combine(WorkPaths.GetXmlSignedPath(), fileName);
            XmlDocument document = new XmlDocument() { PreserveWhitespace = true };
            document.LoadXml(xml);
            document.Save(path);
            return path;
        }

        public static string BuildConfirmationButton(Invoice invoice)
        {
            IBaseUrlResolver baseUrlResolver = IocManager.Instance.Resolve<IBaseUrlResolver>();
            string baseUrl = baseUrlResolver.GetBaseUrl();
            string confirmationLink = $"{baseUrl}Invoice/ConfirmarRecepcion?tenantId={invoice.TenantId}&clientId={invoice.ClientId}&invoiceId={invoice.Id}";

            StringBuilder html = new StringBuilder();
            html.Append("<p>");
            html.Append("<p>Por favor haga clic en el siguiente botón para validar que recibió el Documento Electrónico correctamente.</p>");
            html.Append("<div style='text-align:center;height:40px;width:250px;background-color:#36733C;margin-top:10px;-webkit-border-radius:5px;-moz-border-radius:5px;border-radius:5px;color:#ffffff;display:block;'>");
            html.AppendFormat("<a href='{0}' style='font-size:16px;font-weight:bold;text-decoration:none;line-height:40px;width:100%;display:inline-block'><span style='color:#FFFFFF'>Acuse de Recibo</span></a>", confirmationLink);
            html.Append("</div>");
            html.Append("<p>Si el botón no funciona, por favor copie y pegue en la barra de búsqueda de su explorador la siguiente URL:</p>");
            html.AppendFormat("<p>{0}</p>", confirmationLink);
            html.Append("</p>");
            return html.ToString();
        }

        public bool IsInvoiceReceptionConfirmed(Guid clientId, Guid invoiceId)
        {
            Invoice invoice = null;

            if(clientId.Equals(Guid.Empty))
                invoice = _invoiceRepository.GetAll().Where(i=> i.Id == invoiceId).FirstOrDefault(); 
            else
                invoice = _invoiceRepository.GetAll().Where(i => i.ClientId == clientId && i.Id == invoiceId).FirstOrDefault(); 
            if (invoice != null)
            {
                return invoice.IsInvoiceReceptionConfirmed;
            }
            return false;
        }

        public async Task ConfirmInvoiceReceptionAsync(Guid clientId, Guid invoiceId)
        {
            Invoice invoice = null;

            if(clientId.Equals(Guid.Empty))
                invoice = await _invoiceRepository.GetAll().Where(i => i.Id == invoiceId).FirstOrDefaultAsync();
            else
                invoice = await _invoiceRepository.GetAll().Where(i => i.ClientId == clientId && i.Id == invoiceId).FirstOrDefaultAsync();
            if (invoice != null)
            {
                invoice.IsInvoiceReceptionConfirmed = true;
                await _invoiceRepository.UpdateAsync(invoice);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        public async Task<bool> InvoicesMonthlyLimitReachedAsync(int tenantId)
        {
            bool limitReached = false;
            var tenant = _tenantManager.Get(tenantId);
            if (tenant == null || tenant.EditionId == null)
            {
                limitReached = true;
            }
            int invoicesMonthlyLimit = await GetInvoicesMonthlyLimitAsync(tenant.EditionId.Value);
            int totalInvoicesInMonth = await GetTotalInvoicesInMonthAsync(tenant.Id);
            limitReached = (invoicesMonthlyLimit == 0 || totalInvoicesInMonth >= invoicesMonthlyLimit);
            return limitReached;
        }

        [UnitOfWork]
        public void ResendInvoice(Guid invoiceId)
        {
            var invoice = _invoiceRepository.Get(invoiceId);
            var tenant = _tenantManager.Get(AbpSession.TenantId.Value);
            string cuentaBanca = "";
            if (tenant.TenancyName == "ticopay")
            {
                cuentaBanca = emailCuentaTicopay;
            }
            if ( (invoice != null) && ( ((invoice.Client != null) && (!String.IsNullOrWhiteSpace(invoice.Client.Email))) || (!String.IsNullOrWhiteSpace(invoice.ClientEmail)) ) )
            {
                var email = ((invoice.Client != null) && (!String.IsNullOrWhiteSpace(invoice.Client.Email))) ? invoice.Client.Email : invoice.ClientEmail;
                _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(invoice.TenantId, TicoPayReports.Factura, tenant.ComercialName);
                
                var payments = InfoPaymentInvoice(invoice.Id);
                var listInfoPago = AddPaymentsMethods2(payments);
                
                var drawer = _drawerRepository.GetAll().Where(x => x.TenantId == tenant.Id && x.Id == invoice.DrawerId).FirstOrDefault();
                var branchOffice = infoBranchOffice(drawer.BranchOfficeId);
                var BranchOfficeInfo = AddBranchOfficeInfo(branchOffice);

                GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
                var xml = LoadFileFromAzureStorage("xmlinvoice", invoice.ElectronicBill);

                var pdf = generatePDF.CreatePDFAsStream(invoice, invoice.Client, invoice.Tenant, listInfoPago, BranchOfficeInfo);

                mail.SendMailTicoPay(email, (invoice.TypeDocument == TypeDocumentInvoice.Ticket ? subjectTicket : subject), string.Format(emailbody, invoice.TypeDocument.GetDescription(), invoice.TypeDocument.GetDescription()) + BuildConfirmationButton(invoice), emailfooter, "",
                    pdf, invoice.VoucherKey + ".pdf", xml, invoice.VoucherKey + ".xml", tenant.ComercialName, cuentaBanca);
                xml.Close();
                pdf.Close();
            }
        }

        [UnitOfWork]
        public void ResendNote(Guid noteId)
        {
            Note note = _noteRepository.Get(noteId);
            Tenant tenant = _tenantManager.Get(AbpSession.TenantId.Value);

            if ((note != null) && (((note.Invoice.Client != null) && (!String.IsNullOrWhiteSpace(note.Invoice.Client.Email))) || (!String.IsNullOrWhiteSpace(note.Invoice.ClientEmail))))
            {
                var email = ((note.Invoice.Client != null) && (!String.IsNullOrWhiteSpace(note.Invoice.Client.Email))) ? note.Invoice.Client.Email : note.Invoice.ClientEmail;
                GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
                var xml = LoadFileFromAzureStorage("xmlnote", note.ElectronicBill);

                var pdf = generatePDF.CreatePDFNoteAsStream(note.Invoice, note.Invoice.Client, note.Invoice.Tenant, note, null);

                mail.SendMailTicoPay(email, Note.subject, Note.emailbody, Note.emailfooter, "",
                    pdf, note.VoucherKey + ".pdf", xml, note.VoucherKey + ".xml", tenant.ComercialName, cuentaBanca);
                xml.Close();
                pdf.Close();
            }
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

        public List<Payment> GetPaymentReverse(Guid clientId, FacturaElectronicaResumenFacturaCodigoMoneda codigoModeda)
        {
            var entities = _paymentRepository.GetAllList(c => c.ClientId == clientId && c.IsPaymentReversed == true && c.CodigoMoneda == codigoModeda);
            return entities;
        }

        public static string BuildSmsMessage(string tenantName, string codigoMoneda, int invoiceNumber, decimal invoiceTotal)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(tenantName))
            {
                if (string.IsNullOrWhiteSpace(codigoMoneda))
                {
                    codigoMoneda = "COL";
                }
                string smsMessage = string.Format(SmsFacturaACobroMessage, invoiceNumber, invoiceTotal.ToString("N2", new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }), codigoMoneda);
                string senderName = tenantName.Substring(0, Math.Min(tenantName.Length, (122 - smsMessage.Length)));
                result = $"{senderName}: " + smsMessage;
            }
            return result;
        }

        private static string BuildNoteSmsMessage(string tenantName, NoteType noteType, string codigoMoneda, int noteNumber, decimal noteTotal)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(tenantName))
            {
                if (string.IsNullOrWhiteSpace(codigoMoneda))
                {
                    codigoMoneda = "COL";
                }
                string smsMessage = string.Format(SmsNoteMessage, noteType.GetDescription(), noteTotal.ToString("N2", new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }), codigoMoneda);
                string senderName = tenantName.Substring(0, Math.Min(tenantName.Length, (122 - smsMessage.Length)));
                result = $"{senderName}: " + smsMessage;
            }
            return result;
        }

        private async Task<SmsDebtDto> GetSmsDebt(TicoTalkClient ticoTalkClient, Tenant tenant, Client client)
        {
            SmsDebtDto output = new SmsDebtDto();
            var clientTenant = _tenantAppService.GetBy(t => t.IdentificationNumber == client.Identification);
            if (clientTenant != null && clientTenant.SmsNoficicarFacturaACobro)
            {
                DateTime fromDate = DateTime.Today;
                var lastInvoice = await _invoiceRepository.GetAll().Where(i => i.ClientId == client.Id).OrderByDescending(i => i.Id).FirstOrDefaultAsync();
                if (lastInvoice != null)
                {
                    fromDate = lastInvoice.DueDate;
                }
                output = await ticoTalkClient.GetSmsDebt(fromDate, DateTime.Today.AddDays(1), client.Identification);
            }
            return output;
        }


        public bool IsNoteReceptionConfirmed(Guid invoiceId, Guid noteId)
        {
            var note = _noteRepository.GetAll().Where(i => i.InvoiceId == invoiceId && i.Id == noteId).FirstOrDefault();
            if (note != null)
            {
                return note.IsNoteReceptionConfirmed;
            }
            return false;
        }

        public async Task ConfirmNoteReceptionAsync(Guid invoiceId, Guid noteId)
        {
            var note = _noteRepository.GetAll().Where(i => i.InvoiceId == invoiceId && i.Id == noteId).FirstOrDefault();
            if (note != null)
            {
                note.IsNoteReceptionConfirmed = true;
                await _noteRepository.UpdateAsync(note);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        private string BuildNoteConfirmationButton(Note note)
        {
            IBaseUrlResolver baseUrlResolver = IocManager.Instance.Resolve<IBaseUrlResolver>();
            string baseUrl = baseUrlResolver.GetBaseUrl();
            string confirmationLink = $"{baseUrl}Invoice/ConfirmarRecepcionNota?tenantId={note.TenantId}&invoiceId={note.InvoiceId}&noteId={note.Id}";

            StringBuilder html = new StringBuilder();
            html.Append("<p>");
            html.Append("<p>Por favor haga clic en el siguiente botón para validar que recibió el Documento Electrónico correctamente.</p>");
            html.Append("<div style='text-align:center;height:40px;width:250px;background-color:#36733C;margin-top:10px;-webkit-border-radius:5px;-moz-border-radius:5px;border-radius:5px;color:#ffffff;display:block;'>");
            html.AppendFormat("<a href='{0}' style='font-size:16px;font-weight:bold;text-decoration:none;line-height:40px;width:100%;display:inline-block'><span style='color:#FFFFFF'>Acuse de Recibo</span></a>", confirmationLink);
            html.Append("</div>");
            html.Append("<p>Si el botón no funciona, por favor copie y pegue en la barra de búsqueda de su explorador la siguiente URL:</p>");
            html.AppendFormat("<p>{0}</p>", confirmationLink);
            html.Append("</p>");
            return html.ToString();
        }



        /***
         *
         *
         * 
            _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Nota);
            var typenoteTenant = tenant.TipoFirma == FirmType.Todos ? tenant.FirmaRecurrente : tenant.TipoFirma;

            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                note = Note.Create(AbpSession.TenantId.Value, amountDecimal, invoiceId, "", (NoteCodigoMoneda)tenant.CodigoMoneda, (NoteReason)reason, (NoteType)type, montotax, totalNota);
                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType, false);
                note.SetInvoiceNumberKey(note.CreationTime, note.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);
                asunto = Note.subject + note.NoteType.ToString() + " Electrónica";

                if ((!tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (invoice.TipoFirma == FirmType.Llave))) {
                    //obtiene el certificado si se valida con hacienda
                    if (tenant.ValidateHacienda)
                        certified = _tenantAppService.GetCertified(AbpSession.TenantId.Value);
                    // crear XML  y PDF

                    note.CreateXMLNote(invoice, client, tenant, note, tax, certified);

                    note.CreatePDFNote(invoice, client, tenant, note, tax, _notaReportSettings);

                    Uri XML = SaveAzureStorage(note.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml"), "xmlnote");
                    Uri PDF = SaveAzureStorage(note.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), "note_" + note.VoucherKey + ".pdf"), "pdfnote");

                    note.SetInvoiceXMLPDF(XML, PDF);
                }
                note.TipoFirma = invoice.TipoFirma!=null? invoice.TipoFirma: typenoteTenant;
                note.StatusFirmaDigital = (invoice.TipoFirma == FirmType.Llave) ? StatusFirmaDigital.Firmada : StatusFirmaDigital.Pendiente;

                _noteRepository.Insert(note);

                if ((NoteType)type == NoteType.Credito)
                    invoice.Balance = Decimal.Round(invoice.Balance - note.Total, 2);
                else
                    invoice.Balance = Decimal.Round(invoice.Balance + note.Total, 2);


                string voucherk = note.VoucherKey;


                //verifica si esta activo la validacion con hacienda
                if ((tenant.ValidateHacienda) && (invoice.TipoFirma == FirmType.Llave)) 
                {
                    //enviar note a hacienda

                    string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, invoice, note.VoucherKey, note);
                    //string validate = validateHacienda.SendResponseTribunet("GET", null, null, null, "506140817310174178825001000010100000000561KJFKBSHR");

                    if (validate == "-1")
                    {
                        note.StatusVoucher = VoucherSituation.withoutInternet;
                        int _vouchersituation = (int)VoucherSituation.withoutInternet;
                        note.VoucherKey = note.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + note.VoucherKey.Substring(42, 8);
                        note.SendInvoice = false;
                    }
                    else
                    {
                        note.StatusVoucher = VoucherSituation.Normal;
                        note.SendInvoice = true;
                    }
                }
                UpdateBalanceInvoice(invoice);

                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType);
                unitOfWork.Complete();

                if ((!tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (invoice.TipoFirma == FirmType.Llave))) {
                    // Enviar Correo Electronico 
                    mail.SendMailTicoPay(client.Email, asunto, Note.emailbody + BuildNoteConfirmationButton(note), Note.emailfooter, "",
                        Path.Combine(WorkPaths.GetXmlPath(), "note_" + voucherk + ".xml"),
                        Path.Combine(WorkPaths.GetPdfPath(), "note_" + voucherk + ".pdf"),
                        Path.Combine(WorkPaths.GetQRPath(), "note_" + voucherk + ".png"),
                        Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + voucherk + ".xml"), tenant.AlternativeEmail);

                    if (tenant.SmsNoficicarFacturaACobro && ticoTalkClient.IsAuthenticated)
                    {
                        ticoTalkClient.SendSms(tenant.IdentificationNumber, client.MobilNumber, BuildNoteSmsMessage(tenant.Name, note.NoteType, tenant.CodigoMoneda.ToString(), invoice.Number, invoice.Total));
                    }
                }else
                {
                    //envio correo
                }
         * 
         */

        public async Task SaveXMLFirmaDigital(Invoice invoice, string xmlContent)
        {
            Tenant tenant = _tenantManager.Get(AbpSession.TenantId.Value);

            _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName((int)AbpSession.TenantId, TicoPayReports.Factura, tenant.ComercialName);

            invoice.CreatePDF(invoice, _facturaReportSettings);

            Uri XML = SaveAzureStorageFromText(invoice.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), xmlContent, "xmlinvoice"); //signed
            Uri PDF = SaveAzureStorage(invoice.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), "pdfinvoice");

            invoice.SetInvoiceXML(XML, PDF);

            invoice.StatusFirmaDigital = StatusFirmaDigital.Firmada;

            try
            {
                await _invoiceRepository.UpdateAsync(@invoice);
            }
            catch (Exception ex)
            {
                invoice = _invoiceManager.Get(invoice.Id);
                invoice.StatusFirmaDigital = StatusFirmaDigital.Error;

                await _invoiceRepository.UpdateAsync(invoice);

                throw new UserFriendlyException("Error actualizando la factura", ex);
            }

            string voucherk = invoice.VoucherKey;

            tenant = @invoice.Tenant;

            bool ValidateHacienda = tenant.ValidateHacienda;
            Client client = @invoice.Client;

            if (ValidateHacienda)
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                string validate = validateHacienda.SendResponseTribunet("POST", client, @invoice.Tenant, invoice, invoice.VoucherKey, null, null);
                if (validate == "-1")
                {
                    invoice.StatusVoucher = VoucherSituation.withoutInternet;
                    int _vouchersituation = (int)VoucherSituation.withoutInternet;
                    invoice.VoucherKey = invoice.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + invoice.VoucherKey.Substring(42, 8);
                    invoice.SendInvoice = false;
                    invoice.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
                }
                else
                {
                    invoice.StatusVoucher = VoucherSituation.Normal;
                    invoice.SendInvoice = true;
                }
            }

            // Enviar Correo Electronico 
            if (invoice.ClientEmail != null)
            {
                mail.SendMailTicoPay(invoice.ClientEmail, (invoice.TypeDocument == TypeDocumentInvoice.Ticket ? InvoiceAppService.subjectTicket : InvoiceAppService.subject), string.Format(InvoiceAppService.emailbody, invoice.TypeDocument.GetDescription(), invoice.TypeDocument.GetDescription()) + InvoiceAppService.BuildConfirmationButton(invoice), InvoiceAppService.emailfooter, "",
              Path.Combine(WorkPaths.GetXmlPath(), voucherk + ".xml"),
              Path.Combine(WorkPaths.GetPdfPath(), voucherk + ".pdf"),
              Path.Combine(WorkPaths.GetQRPath(), voucherk + ".png"),
              Path.Combine(WorkPaths.GetXmlSignedPath(), voucherk + ".xml"), tenant.AlternativeEmail, tenant.ComercialName, "", (invoice.Client != null) ? invoice.Client.ContactEmail : string.Empty); //signed
            }

            if (tenant.SmsNoficicarFacturaACobro)
            {
                TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);
                if ((ticoTalkClient.IsAuthenticated) && (invoice.ClientMobilNumber != null))
                {
                    ticoTalkClient.SendSms(tenant.IdentificationNumber, invoice.ClientMobilNumber, InvoiceAppService.BuildSmsMessage(tenant.Name, tenant.CodigoMoneda.ToString(), invoice.Number, invoice.Total));
                }
            }
        }


        public async Task SaveXMLFirmaDigital(Note note, string xmlContent)
        {
            Tenant tenant = _tenantManager.Get(AbpSession.TenantId.Value);
            _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName((int)AbpSession.TenantId, TicoPayReports.Nota, tenant.ComercialName);

            note.CreatePDFNote(note, _notaReportSettings);

            Uri XML = SaveAzureStorageFromText(note.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml"), xmlContent, "xmlnote"); //signed
            Uri PDF = SaveAzureStorage(note.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), "note_" + note.VoucherKey + ".pdf"), "pdfnote");

            note.SetInvoiceXMLPDF(XML, PDF);

            note.StatusFirmaDigital = StatusFirmaDigital.Firmada;

            try
            {
                await _noteRepository.UpdateAsync(note);
            }
            catch (Exception ex)
            {
                note = _noteRepository.Get(note.Id);
                note.StatusFirmaDigital = StatusFirmaDigital.Error;

                await _noteRepository.UpdateAsync(note);

                throw new UserFriendlyException("Error actualizando la factura", ex);
            }

            string voucherk = note.VoucherKey;

            tenant = note.Invoice.Tenant;

            bool ValidateHacienda = tenant.ValidateHacienda;
            Client client = note.Invoice.Client;

            if (ValidateHacienda)
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, note.Invoice, note.VoucherKey, note, null);
                if (validate == "-1")
                {
                    note.StatusVoucher = VoucherSituation.withoutInternet;
                    int _vouchersituation = (int)VoucherSituation.withoutInternet;
                    note.VoucherKey = note.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + note.VoucherKey.Substring(42, 8);
                    note.SendInvoice = false;
                }
                else
                {
                    note.StatusVoucher = VoucherSituation.Normal;
                    note.SendInvoice = true;
                }
            }
            string asunto = Note.subject + note.NoteType.GetDescription() + " Electrónica";
            if (note.Invoice.ClientEmail != null)
            {
                mail.SendMailTicoPay(note.Invoice.ClientEmail, asunto, Note.emailbody + BuildNoteConfirmationButton(note), Note.emailfooter, "",
                Path.Combine(WorkPaths.GetXmlPath(), "note_" + voucherk + ".xml"),
                Path.Combine(WorkPaths.GetPdfPath(), "note_" + voucherk + ".pdf"),
                Path.Combine(WorkPaths.GetQRPath(), "note_" + voucherk + ".png"),
                Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + voucherk + ".xml"), tenant.AlternativeEmail, tenant.ComercialName, "", (note.Invoice.Client != null) ? note.Invoice.Client.ContactEmail : string.Empty);
            }


            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

            if ((tenant.SmsNoficicarFacturaACobro && ticoTalkClient.IsAuthenticated) && (note.Invoice.ClientMobilNumber != null))
            {
                ticoTalkClient.SendSms(tenant.IdentificationNumber, note.Invoice.ClientMobilNumber, BuildNoteSmsMessage(tenant.Name, note.NoteType, tenant.CodigoMoneda.ToString(), 0, note.Total));
            }
        }

        public Uri SaveAzureStorageFromText(string documentname, string ruta, string content, string _container)
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


            blockBlob.UploadText(content, System.Text.Encoding.UTF8);

            using (FileStream target = File.Open(ruta, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(target, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(content);
                }
            }


            return blockBlob.Uri;
        }

        public Uri SaveAzureStorageFromText(string documentname, string ruta, Stream content, string _container)
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


            blockBlob.UploadFromStream(content);

            return blockBlob.Uri;
        }

        public void sendemailbodyRecurren(string email, string AlternativeEmail, string type, string remitente)
        {
            mail.SendMailTicoPay(email, subject, String.Format(emailbodyRecurren, type, emailSteps), emailfooter, "", AlternativeEmail, remitente, cuentaBanca);
        }

        public bool isdigitalPendingInvoice(int tenatId)
        {
            bool isPending = false;

            var invoice = _invoiceRepository.GetAll().Where(i => (i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Pendiente) && i.IsDeleted == false && i.TenantId == tenatId).FirstOrDefault();
            if (invoice != null)
                isPending = true;
            return isPending;
        }

        public bool isdigitalPendingNote(int tenatId)
        {
            bool isPending = false;

            var note = _noteRepository.GetAll().Where(i => i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Pendiente && i.IsDeleted == false && i.TenantId == tenatId).FirstOrDefault();
            if (note != null)
                isPending = true;
            return isPending;
        }

        public async Task<bool> TenantCantDoInvoices(string username, FirmType? firmType)
        {
            User user = null;
            Tenant output = null;
            try
            {
                output = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
                user = await _userManager.FindByNameOrEmailAsync(username);

                if (!(output.BarrioId != null && output.CountryID != null && output.Address != null && user.IsEmailConfirmed))
                {
                    throw new UserFriendlyException(ErrorCodeHelper.CantCreateInvoices, ((user.IsEmailConfirmed) ? "" : "Confirme su correo electrónico en su buzón de correo. " + ((output.BarrioId != null && output.CountryID != null && output.Address != null) ? "" : "Complete su dirección fiscal.")));
                }
            }
            catch (Exception)
            {
                throw new UserFriendlyException(ErrorCodeHelper.CantCreateInvoices, "Por favor confirme su usuario y verifique su dirección fiscal.");
            }

            bool monthlyLimitReached = await InvoicesMonthlyLimitReachedAsync(AbpSession.TenantId.Value);
            if (monthlyLimitReached)
            {
                throw new UserFriendlyException(ErrorCodeHelper.InvoicesMonthlyLimitReached, "Has alcanzado el limite mensual de facturas. Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.");
            }

            if ((firmType == FirmType.Llave) && (isdigitalPendingInvoice(output.Id)))
            {
                throw new UserFriendlyException(ErrorCodeHelper.InvoicesWithoutFirm, "Posee facturas pendientes por firma digital. Por favor complete el proceso de firma y envío a hacienda de estas factura para poder generar nuevas facturas con llave criptográfica.");
            }

            return true;
        }

        public bool HasPayFirstInvoice(Tenant tenant)
        {
            Client client = _clientRepository.GetAll().Where(d => d.Identification == tenant.IdentificationNumber && d.TenantId == 2).FirstOrDefault();

            var tenantCreationDate = client.CreationTime;

            var bottomInvoiceLimit = DateTimeZone.Convert(tenantCreationDate);
            var topInvoiceLimit = bottomInvoiceLimit.AddMonths(1);

            var recentlyInvoice = _invoiceRepository.Count(d => d.Tenant.Id == 2 && d.ClientId == client.Id && d.DueDate >= bottomInvoiceLimit && d.DueDate <= topInvoiceLimit && d.Status == Status.Parked);

            return recentlyInvoice > 1 ? true : false;

        }

        public List<int> GetTenantsIdWithInvoicesNotSended()
        {
            var notSendedInvoices = _invoiceRepository
            .GetAll()
            .Where(i => !i.SendInvoice && !i.IsDeleted && i.ElectronicBill != null && ((i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Firmada) || (i.TipoFirma == FirmType.Llave && i.StatusFirmaDigital == null)))
            .Select(i => i.TenantId)
            .GroupBy(i => i)
            .Select(i => i.Key)
            .ToList();

            return notSendedInvoices;
        }


        #region note
        /// <summary>
        /// Método para hacer una NCR a una factura pagada
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="amount"></param>
        /// <param name="reason"></param>
        /// <param name="type"></param>
        /// <param name="montotax"></param>
        /// <param name="totalNota"></param>
        /// <param name="tenantid"></param>
        public void CreateNoteCorreccion(Guid invoiceId, double amount, int reason, int type, decimal montotax, decimal totalNota, int tenantid)
        {
            DateTime dueDate = DateTime.Now;
            Status status = Status.Completed;
            FacturaElectronicaCondicionVenta conditionSaleType = FacturaElectronicaCondicionVenta.Contado;

            CreateCorrectionNote(tenantid, invoiceId, (decimal)amount, montotax, (NoteReason)reason, (NoteType)type,  dueDate,  status,  conditionSaleType);
    }

        public Note CreateCorrectionNote(int tenantId, Guid invoiceId, decimal amount, decimal taxAmount, NoteReason reason, NoteType type, DateTime dueDate, Status status, FacturaElectronicaCondicionVenta conditionSaleType)
        {
            var tenant = _tenantManager.Get(tenantId);
            _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenantId, TicoPayReports.Nota, tenant.ComercialName);
            Client client = null;
            var allRegisters = _invoiceManager.GetallRegisters(tenantId);
            var invoice = _invoiceManager.Get(invoiceId);
            if (invoice.ClientId != null)
             client = _clientManager.Get(invoice.ClientId.Value);
            //var service = _serviceManager.Get(invoice.InvoiceLines.First().ServiceId.Value);
            var tax = invoice.InvoiceLines.First().Tax;
            var typenoteTenant = tenant.TipoFirma == FirmType.Todos ? tenant.FirmaRecurrente : tenant.TipoFirma;

            Certificate certified = null;

            if ((tenant.ValidateHacienda) && ((String.IsNullOrEmpty(tenant.UserTribunet) || String.IsNullOrEmpty(tenant.PasswordTribunet))))
            {
                throw new UserFriendlyException("Debe ingresar las credenciales para el envío de sus comprobantes a Hacienda. Por favor vaya a Configuración --> Compañía e ingrese estos datos (los mismos son generados en la página de ATV)");
            }

            if ((invoice.TipoFirma == FirmType.Llave) && (isdigitalPendingNote(invoice.TenantId)))
            {
                throw new Abp.Runtime.Validation.AbpValidationException("Existen notas pendientes por firma digital.");
            }

            Note note = null;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                note = Note.Create(tenantId, amount, invoiceId, "", (NoteCodigoMoneda)invoice.CodigoMoneda, (NoteReason)reason, (NoteType)type, taxAmount, amount + taxAmount,
                    dueDate, status, conditionSaleType, 0, invoice);
                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType, false);
                note.SetInvoiceNumberKey(note.CreationTime, note.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);

                

                if (tenant.ValidateHacienda)
                {
                    note.TipoFirma = invoice.TipoFirma != null ? invoice.TipoFirma : typenoteTenant;
                    if (invoice.TipoFirma == FirmType.Firma)
                        note.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
                }


                if ((!tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (invoice.TipoFirma == FirmType.Llave)))
                {

                    //obtiene el certificado si se valida con hacienda
                    if (tenant.ValidateHacienda)
                        certified = _tenantAppService.GetCertified(tenantId);


                    // crear XML  y PDF

                    note.CreateXMLNote(invoice, client, tenant, note, certified);

                    note.CreatePDFNote(invoice, client, tenant, note, _notaReportSettings);

                    Uri XML = SaveAzureStorage(note.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml"), "xmlnote");
                    Uri PDF = SaveAzureStorage(note.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), "note_" + note.VoucherKey + ".pdf"), "pdfnote");
                    note.SetInvoiceXMLPDF(XML, PDF);
                }

                note.SendInvoice = false;
                _noteRepository.Insert(note);

                if (type == NoteType.Credito)
                    invoice.Balance = Invoice.GetValor(invoice.Balance - note.Total);
                else
                    invoice.Balance = Invoice.GetValor(invoice.Balance + note.Total);

                //verifica si esta activo la validacion con hacienda
                if ((tenant.ValidateHacienda) && (invoice.TipoFirma == FirmType.Llave))
                {
                    //enviar note a hacienda
                    string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, invoice, note.VoucherKey, note, null);
                    if (validate == "-1")
                    {
                        note.StatusVoucher = VoucherSituation.withoutInternet;
                        int _vouchersituation = (int)VoucherSituation.withoutInternet;
                        note.VoucherKey = note.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + note.VoucherKey.Substring(42, 8);
                        note.SendInvoice = false;
                    }
                    else
                    {
                        note.StatusVoucher = VoucherSituation.Normal;
                        note.SendInvoice = true;
                    }
                }
                UpdateBalanceInvoice(invoice);

                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType);
                unitOfWork.Complete();

                // Enviar Correo Electronico 
                string asunto = Note.subject + note.NoteType.GetDescription() + " Electrónica";
                if ((!tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (invoice.TipoFirma == FirmType.Llave)))
                {

                    if ((client != null) && (!String.IsNullOrWhiteSpace(client.Email)))
                    {
                        string voucherk = note.VoucherKey;
                        mail.SendMailTicoPay(client.Email, asunto, Note.emailbody + BuildNoteConfirmationButton(note), Note.emailfooter, "",
                            Path.Combine(WorkPaths.GetXmlPath(), "note_" + voucherk + ".xml"),
                            Path.Combine(WorkPaths.GetPdfPath(), "note_" + voucherk + ".pdf"),
                            Path.Combine(WorkPaths.GetQRPath(), "note_" + voucherk + ".png"),
                            Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + voucherk + ".xml"), tenant.AlternativeEmail,
                            tenant.ComercialName, "", (client != null) ? client.ContactEmail : string.Empty);
                    }
                    
                }
                else
                {
                    //envio correo
                    mail.SendMailTicoPay(tenant.Email, asunto, String.Format(emailbodyNoteSignature, note.NoteType.GetDescription(), note.ConsecutiveNumber, emailSteps), emailfooter, "", tenant.AlternativeEmail, tenant.ComercialName, cuentaBanca);
                }

            }
            return note;
        }

      
        public NoteDto CreateNote(NoteDto input/*Guid invoiceId, double amount, int reason, int type, decimal montotax, decimal totalNota*/, bool noAfectarBalance = false)
        {
            //decimal amountDecimal = Convert.ToDecimal(amount);
            Client client = null;
            
            var tenant = _tenantManager.Get(AbpSession.TenantId.Value);
            var invoice = _invoiceManager.Get((Guid)input.InvoiceId);
         

            if (invoice.ClientId!=null)
                client = _clientManager.Get(invoice.ClientId.Value);

            if ((tenant.ValidateHacienda) && ((String.IsNullOrEmpty(tenant.UserTribunet) || String.IsNullOrEmpty(tenant.PasswordTribunet))))
            {
                throw new UserFriendlyException("Debe ingresar las credenciales para el envío de sus comprobantes a Hacienda. Por favor vaya a Configuración --> Compañía e ingrese estos datos (los mismos son generados en la página de ATV) en el módulo de configuración de compañía");
            }

            if (input.Drawer == null)
            {
                var drawer = _drawersAppService.GetDrawers(new Drawers.Dto.SearchDrawerInput { CodeFilter = "00001", TenantId = tenant.Id }).FirstOrDefault();
                if (drawer!=null)
                {
                    input.Drawer = drawer;
                }
                else
                {
                    throw new UserFriendlyException("Ha ocurrido un error al buscar la caja por defecto.");
                }
            }
       
            Note note;



            if (input.NoteReasons == NoteReason.Reversa_documento && input.NoteType == NoteType.Debito)
            {
                throw new Abp.Runtime.Validation.AbpValidationException("La razón de una nota de débito no puede ser Reversar Documento.");
            }
            if (input.NoteType == NoteType.Debito && invoice.Status == Status.Completed && ((input.NoteReasons != NoteReason.Reversa_documento) && (input.NoteReasons != NoteReason.Corregir_Monto_Factura)))
            {
                throw new Abp.Runtime.Validation.AbpValidationException("No se puede aplicar una nota de débito a una factura o tiquete pagado.");
            }

            decimal invoiceTotal = invoice.GetTotal();
            if (input.NoteType == NoteType.Credito && ((invoiceTotal + rango) < (input.Amount + input.TaxAmount) || (invoiceTotal + rango) < input.Total))
            {
                throw new Abp.Runtime.Validation.AbpValidationException("El monto de la nota no puede ser superior al monto de la factura o tiquete.");
            }

            if ((invoice.TipoFirma == FirmType.Llave) && (isdigitalPendingNote(invoice.TenantId)))
            {
                throw new Abp.Runtime.Validation.AbpValidationException("Existen notas pendientes por firma digital");
            }
            var allRegisters = _invoiceManager.GetRegistersbyDrawer(input.Drawer.Id);

            _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Nota, tenant.ComercialName);

            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();



                note = Note.Create(AbpSession.TenantId.Value, input.Amount, (Guid)input.InvoiceId, "", (NoteCodigoMoneda)invoice.CodigoMoneda, input.NoteReasons, input.NoteType, input.TaxAmount, input.Total,
                    input.DueDate, input.Status, input.ConditionSaleType, input.CreditTerm, invoice);
                //VerificarCalculoNote(note,input);

                note = AddLinesNote(tenant, input, note);

                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType, false,input.Drawer);
                note.SetInvoiceNumberKey(note.CreationTime, note.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);
                note.NoteReasonsOthers = input.NoteReasonsOthers;
                note.TotalNote(note, invoice);
                if (input.IsContingency)
                {
                    note.IsContingency = input.IsContingency;
                    note.ConsecutiveNumberContingency = input.ConsecutiveNumberContingency;
                    note.ReasonContingency = input.ReasonContingency.Length > MaxReasonContingency ? input.ReasonContingency.Substring(0, MaxReasonContingency - 1) : input.ReasonContingency;
                    note.DateContingency = input.DateContingency;
                }

                if (input.Drawer != null)
                {
                    note.DrawerId = input.Drawer.Id;
                }
                if(input.ExternalReferenceNumber != null)
                {
                    note.ExternalReferenceNumber = input.ExternalReferenceNumber;
                }                    
                note.SendInvoice = false;
                note= _noteRepository.Insert(note);             

                if (noAfectarBalance == false)
                    invoice = UpdatebalanceNote(invoice, input, note);

                if (invoice.Balance <= 0)
                    {
                    invoice.Status = Status.Returned;
                    if (noAfectarBalance == false)
                    {                        
                        UpdateBalanceInvoice(invoice);
                    }

                    var paymentList = _paymentinvoiceRepository.GetAll().Where(a => a.InvoiceId == input.InvoiceId).Include(a => a.Invoice).ToList();

                    foreach (var payment in paymentList)
                    {
                        //PaymentInvoice paymentInvoice = _paymentinvoiceRepository.Get(payment.Id);
                        payment.IsPaymentReversed = true;
                        payment.Payment.IsPaymentReversed = true;
                        payment.Payment.IsPaymentUsed = false;
                        payment.Payment.PaymentType = PaymentType.Refund;
                        payment.Payment.Balance = payment.Payment.Amount;
                        _paymentRepository.Update(payment.Payment);
                        _paymentinvoiceRepository.Update(payment);
                    }

                    var paymentNoteList = _paymentnoteRepository.GetAll().Where(a => a.Note.InvoiceId == input.InvoiceId).Include(a => a.Note).ToList();

                    foreach (var payment in paymentNoteList)
                    {
                        // PaymentInvoice paymentInvoice = _paymentnoteRepository.Get(payment.Id);
                        payment.IsPaymentReversed = true;
                        payment.Payment.IsPaymentReversed = true;
                        payment.Payment.IsPaymentUsed = false;
                        payment.Payment.PaymentType = PaymentType.Refund;
                        payment.Payment.Balance = payment.Payment.Amount;
                        _paymentRepository.Update(payment.Payment);
                        _paymentnoteRepository.Update(payment);
                    }
                }
                
                if (tenant.IsConvertUSD)
                {
                    note.ChangeType = invoice.ChangeType;
                }

                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType);
                unitOfWork.Complete();
            }

            note = SaveXMLPDFNote(tenant, invoice, client, input, note, false);
            note = SendNoteHacienda(tenant, client, invoice, note);

            return Mapper.Map<NoteDto>(note);
        }

        public NoteDto CreateReverseNoteFromInvoice(InvoiceDto invoiceOrTicket, string externalReferenceNumber = "N/A")
        {
            NoteDto nota = new NoteDto();
            nota.InvoiceId = invoiceOrTicket.Id;
            nota.NumberInvoiceRef = invoiceOrTicket.ConsecutiveNumber;
            nota.ExternalReferenceNumber = externalReferenceNumber;
            nota.Amount = invoiceOrTicket.SubTotal;
            nota.TaxAmount = invoiceOrTicket.TotalTax;
            nota.DiscountAmount = invoiceOrTicket.DiscountAmount;
            nota.Total = invoiceOrTicket.Total;
            nota.CodigoMoneda = (NoteCodigoMoneda) invoiceOrTicket.CodigoMoneda;
            nota.NoteType = NoteType.Credito;
            nota.IsNoteReceptionConfirmed = false;
            nota.NoteReasons = NoteReason.Reversa_documento;
            nota.NoteReasonsOthers = null;
            nota.TipoFirma = invoiceOrTicket.TipoFirma;
            nota.DueDate = DateTime.UtcNow;
            nota.Status = Status.Completed;
            nota.ConditionSaleType = FacturaElectronicaCondicionVenta.Otros;
            nota.CreditTerm = 0;
            nota.NotesLines = new List<NoteLineDto>();
            #region LineNote
            NoteLineDto lineaNotaDetalle;
            int numeroLinea = 1;
            foreach (InvoiceLineDto item in invoiceOrTicket.InvoiceLines)
            {
                lineaNotaDetalle = new NoteLineDto();
                lineaNotaDetalle.PricePerUnit = item.PricePerUnit;
                lineaNotaDetalle.SubTotal = item.SubTotal;
                lineaNotaDetalle.LineTotal = item.LineTotal;
                // lineaNotaDetalle.Tax = item.Tax;
                lineaNotaDetalle.TaxId = item.TaxId;
                lineaNotaDetalle.TaxAmount = item.TaxAmount;
                lineaNotaDetalle.Total = item.Total;
                // lineaNotaDetalle.DiscountPercentage = 0;
                lineaNotaDetalle.DiscountPercentage = item.DiscountPercentage;
                lineaNotaDetalle.Note = item.Note;
                lineaNotaDetalle.Title = item.Title;
                lineaNotaDetalle.Quantity = item.Quantity;
                lineaNotaDetalle.LineType = item.LineType;
                if(item.ProductId != null)
                {
                    lineaNotaDetalle.ProductId = item.ProductId;
                }
                if(item.ServiceId != null)
                {
                    lineaNotaDetalle.ServiceId = item.ServiceId;
                }
                lineaNotaDetalle.LineNumber = numeroLinea;
                // lineaNotaDetalle.CodeTypes = CodigoTypeTipo.Otros;
                lineaNotaDetalle.DescriptionDiscount = null;                
                lineaNotaDetalle.ExonerationId = null;
                // lineaNotaDetalle.Service = null;
                lineaNotaDetalle.UnitMeasurement = item.UnitMeasurement;
                if (item.UnitMeasurementOthers != null)
                {
                    lineaNotaDetalle.UnitMeasurementOthers = item.UnitMeasurementOthers;
                }
                else
                {
                    lineaNotaDetalle.UnitMeasurementOthers = null;
                }
                nota.NotesLines.Add(lineaNotaDetalle);
                numeroLinea++;
            }
            #endregion
            // nota.CreditTerm = 0;
            return nota;
        }



        public void ApplyReverse(NoteDto input) //Quitar
        {
            CreateNote(input);
        }

        public Note ReverseNote(NoteDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            // var invoice = Get((Guid)model.InvoiceId);
            var invoice = _invoiceManager.Get((Guid)model.InvoiceId);
            if (invoice == null)
            {
                throw new AbpValidationException("No se encontró la factura con Id: " + model.InvoiceId);
            }
            var origenNote = _noteRepository.Get(model.DocumentRef.Id); //invoice.Notes.Where(n => n.Id == model.DocumentRef.Id).FirstOrDefault();
            if ((origenNote == null) || (origenNote.InvoiceId != model.InvoiceId))
            {
                throw new AbpValidationException("No se encontró la nota con Id: " + model.DocumentRef.Id);
            }
            if (model.Total == origenNote.Total && model.NoteReasons != NoteReason.Reversa_documento)
            {
                throw new AbpValidationException($"Si el total de la nota de reversa es igual al total de la nota origen, la razón debe ser {NoteReason.Reversa_documento.GetDescription()}");
            }
            if ((model.Amount + model.TaxAmount) != origenNote.Total && model.NoteReasons != NoteReason.Corregir_Monto_Factura)
            {
                throw new AbpValidationException($"Si el total de la nota de reversa es diferente al total de la nota origen, la razón debe ser {NoteReason.Corregir_Monto_Factura.GetDescription()}");
            }
            if (model.NoteType == origenNote.NoteType)
            {
                throw new AbpValidationException($"La nota de reversa no puede ser del mismo tipo de la nota original.");
            }
            model.Drawer = _drawerRepository.Get(invoice.DrawerId.Value);
            if (model.Drawer == null)
            {
                throw new UserFriendlyException("Debe abrir una caja para realizar notas de crédito o débito.");
            }
            return CreateReverseNote(invoice.TenantId, invoice,  model);
        }

        private Note CreateReverseNote(int tenantId, Invoice invoice,  NoteDto input)
        {
           
            Client client = null;
            var tenant = _tenantManager.Get(tenantId);
            _notaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenantId, TicoPayReports.Nota, tenant.ComercialName);

            var allRegisters = _invoiceManager.GetRegistersbyDrawer(input.Drawer.Id);

            if (invoice.ClientId!=null)
             client = _clientManager.Get(invoice.ClientId.Value);           
            Note note = null;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                note = Note.Create(tenantId, input.Amount, (Guid)input.InvoiceId, "", (NoteCodigoMoneda)input.CodigoMoneda, input.NoteReasons, input.NoteType, input.TaxAmount, input.Total,
                    input.DueDate, input.Status, input.ConditionSaleType, input.CreditTerm, invoice);
                note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType,true,input.Drawer);
                note.SetInvoiceNumberKey(note.CreationTime, note.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);
                note.ConsecutiveNumberReference = input.DocumentRef.ConsecutiveNumber; //referencia a la nota original
                note.NoteReasonsOthers = input.NoteReasonsOthers;

                if (input.IsContingency)
                {
                    note.IsContingency = input.IsContingency;
                    note.ConsecutiveNumberContingency = input.ConsecutiveNumberContingency;
                    note.ReasonContingency = input.ReasonContingency.Length > MaxReasonContingency ? input.ReasonContingency.Substring(0, MaxReasonContingency - 1) : input.ReasonContingency;
                    note.DateContingency = input.DateContingency;
                }
                if (input.Drawer != null)
                {
                    note.DrawerId = input.Drawer.Id;
                }

                note = AddLinesNote(tenant, input, note);
                note = SaveXMLPDFNote(tenant, invoice, client, input, note, true);
                note.SendInvoice = false;
                _noteRepository.Insert(note);
                invoice = UpdatebalanceNote(invoice, input, note);
                note = SendNoteHacienda(tenant, client, invoice, note);
                _unitOfWorkManager.Current.SaveChanges();
                unitOfWork.Complete();
            }
            return note;
        }



        private Note AddLinesNote(Tenant tenant, NoteDto input, Note note)
        {
            var unitMeasurementDefault = tenant.UnitMeasurementDefault == null ? 0 : tenant.UnitMeasurementDefault;
            var unitMeasurementOthersDefault = tenant.UnitMeasurementOthersDefault == null ? string.Empty : tenant.UnitMeasurementOthersDefault;
            int line = 1;
            decimal descuentoLinea = 0;
            decimal impuestoLinea = 0;
            foreach (var cs in input.NotesLines)
            {
                if (cs.UnitMeasurement == null)
                {
                    cs.UnitMeasurement = unitMeasurementDefault;
                    cs.UnitMeasurementOthers = unitMeasurementOthersDefault;
                };

                string title = cs.Title;

                if (title.Length > MaxTitleLength)
                    title = cs.Title.Substring(0, MaxTitleLength - 1);

                var tax = _taxRepository.Get((Guid)cs.TaxId);

                //decimal subTotal = Invoice.GetValor(cs.PricePerUnit * cs.Quantity, 1);
                //decimal descuento = Invoice.GetValor(subTotal * (cs.DiscountPercentage / 100), 1);
                //descuentoLinea += descuento;
                //decimal total = Invoice.GetValor((subTotal - descuento), 1);
                //decimal impuesto = total * (tax.Rate / 100);
                //cs.TaxAmount = Invoice.GetValor(impuesto, 1);
                //impuestoLinea += cs.TaxAmount;

                note.AssignNoteLine(tenant.Id, cs.PricePerUnit, cs.TaxAmount, cs.DiscountPercentage, cs.DescriptionDiscount, cs.Note, title, cs.Quantity, LineType.Service, null, null, note, line++,
                    tax, cs.TaxId, (UnidadMedidaType)cs.UnitMeasurement, cs.UnitMeasurementOthers);

            }
            //input.DiscountAmount = descuentoLinea;
            //input.TaxAmount = impuestoLinea;
            note.SetInvoiceTotalCalculate(input.TaxAmount, input.DiscountAmount, input.NotesLines.Where(d => d.TaxAmount > 0).Sum(d => (d.PricePerUnit * d.Quantity)), input.NotesLines.Where(d => d.TaxAmount == 0).Sum(d => (d.PricePerUnit * d.Quantity)));

            return note;
        }

        private Note SaveXMLPDFNote(Tenant tenant, Invoice invoice, Client client, NoteDto input, Note note, bool isreverseNote)
        {
            Certificate certified = null;
            var typenoteTenant = tenant.TipoFirma == FirmType.Todos ? tenant.FirmaRecurrente : tenant.TipoFirma;

            if (tenant.ValidateHacienda)
            {
                note.TipoFirma = input.TipoFirma != null ? input.TipoFirma : typenoteTenant;
                if (note.TipoFirma == FirmType.Firma)
                    note.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
            }

            if ((!tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (note.TipoFirma == FirmType.Llave)))
            {
                //obtiene el certificado si se valida con hacienda
                if (tenant.ValidateHacienda)
                    certified = _tenantAppService.GetCertified(tenant.Id);

                // crear XML  y PDF
                if (isreverseNote)
                {
                    var noteOrigen = _noteRepository.Get(input.DocumentRef.Id);
                    note.CreateXMLAnulaNoteCR(noteOrigen, client, tenant, note, certified);
                }
                else
                    note.CreateXMLNote(invoice, client, tenant, note, certified);

                note.CreatePDFNote(invoice, client, tenant, note, _notaReportSettings);

                Uri XML = SaveAzureStorage(note.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml"), "xmlnote");
                Uri PDF = SaveAzureStorage(note.VoucherKey + ".pdf", Path.Combine(WorkPaths.GetPdfPath(), "note_" + note.VoucherKey + ".pdf"), "pdfnote");
                note.SetInvoiceXMLPDF(XML, PDF);


            }

            return note;
        }


        private Invoice UpdatebalanceNote(Invoice invoice, NoteDto input, Note note)
        {
            if (input.NoteType == NoteType.Credito)
            {
                var totalNota = Invoice.GetValor(note.Total);
                invoice.Balance = Invoice.GetValor(invoice.Balance - totalNota);
                if (note.ConsecutiveNumberReference != null)
                {
                    var origenNote = _noteRepository.Get(input.DocumentRef.Id);
                    if (origenNote != null)
                    {
                        origenNote.Balance = 0;
                        origenNote.Status = Status.Completed;
                        _noteRepository.Update(origenNote);
                    }
                }

            }
            else
                invoice.Balance = Invoice.GetValor(invoice.Balance + note.Total);

            UpdateBalanceInvoice(invoice);

            return invoice;
        }

        private Note SendNoteHacienda(Tenant tenant, Client client, Invoice invoice, Note note)
        {
            string asunto = Note.subject + note.NoteType.GetDescription() + " Electrónica";
            if ((!tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (note.TipoFirma == FirmType.Llave)))
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                //verifica si esta activo la validacion con hacienda

                if (tenant.ValidateHacienda)
                {
                    //enviar note a hacienda
                    string validate = validateHacienda.SendResponseTribunet("POST", client, tenant, invoice, note.VoucherKey, note, null);
                    if (validate == "-1")
                    {
                        note.StatusVoucher = VoucherSituation.withoutInternet;
                        int _vouchersituation = (int)VoucherSituation.withoutInternet;
                        note.VoucherKey = note.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + note.VoucherKey.Substring(42, 8);
                        note.SendInvoice = false;
                    }
                    else
                    {
                        note.StatusVoucher = VoucherSituation.Normal;
                        note.SendInvoice = true;
                    }
                }


                // Enviar Correo Electronico 

                string voucherk = note.VoucherKey;
                if ((client != null) && (!String.IsNullOrWhiteSpace(client.Email)) || (invoice.ClientEmail != null))
                {
                    var email = (client != null) && (!String.IsNullOrWhiteSpace(client.Email)) ? client.Email : invoice.ClientEmail;
                    mail.SendMailTicoPay(email, asunto, Note.emailbody + BuildNoteConfirmationButton(note), Note.emailfooter, "",
                    Path.Combine(WorkPaths.GetXmlPath(), "note_" + voucherk + ".xml"),
                    Path.Combine(WorkPaths.GetPdfPath(), "note_" + voucherk + ".pdf"),
                    Path.Combine(WorkPaths.GetQRPath(), "note_" + voucherk + ".png"),
                    Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + voucherk + ".xml"), tenant.AlternativeEmail, tenant.ComercialName, "", (client != null) ? client.ContactEmail : string.Empty);
                }

            }
            else
            {
                //envio de correo
                mail.SendMailTicoPay(tenant.Email, asunto, String.Format(emailbodyNoteSignature, note.NoteType.GetDescription(), note.ConsecutiveNumber, emailSteps), emailfooter, "", tenant.AlternativeEmail, tenant.ComercialName, cuentaBanca);
            }

            return note;
        }

        public decimal GetNoteAmountAlreadyReversed(string ConsecutiveNumber)
        {
            decimal totalAmount = 0;
            var queryResult = _noteRepository.GetAll().Where(n => n.ConsecutiveNumberReference == ConsecutiveNumber).ToList();
            if (queryResult != null)
            {
                    totalAmount = queryResult.Sum(n => n.Amount);
            }

            return totalAmount;
        }

        public Note GetNote(Guid id)
        {
            //return _noteRepository.Get(id);
            return _noteRepository.GetAll().Where(x => x.Id == id).Include("Invoice").Include("Invoice.Client").Include("NotesLines")
                .Include("NotesLines.Tax").Include("NotePaymentTypes").Include("NotePaymentTypes.Payment").Include("Drawer.BranchOffice")
                .FirstOrDefault();
        }

        private Invoice headerClient(Invoice invoice, Client client)
        {
            invoice.ClientName = client.Name + " " + (client.IdentificationType != IdentificacionTypeTipo.Cedula_Juridica ? client.LastName : string.Empty);
            invoice.ClientAddress = (client.Barrio != null ? client.Barrio.NombreBarrio + ", " + client.Barrio.Distrito.NombreDistrito + ", " + client.Address : string.Empty);
            invoice.ClientEmail = client.Email;
            invoice.ClientMobilNumber = client.MobilNumber;
            invoice.ClientIdentificationType = client.IdentificationType;
            invoice.ClientIdentification = client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? client.IdentificacionExtranjero : client.Identification;
            invoice.ClientPhoneNumber = client.PhoneNumber;

            return invoice;
        }
        #endregion note
    }
}
