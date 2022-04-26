using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TicoPay.Vouchers.Dto;

using System.Xml.Serialization;
using Abp.Domain.Repositories;
using Abp.Application.Services;
using TicoPay.MultiTenancy;
using Abp.Domain.Uow;
using static TicoPay.MultiTenancy.Tenant;
using System.IO;
using TicoPay.Core.Common;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.Clients;
using TicoPay.Common;
using SendMail;
using PagedList;
using Abp.UI;
using LinqKit;
using System.Data.Entity;
using IdentityModel.Client;
using System.Xml;
using TicoPay.Users;
using Abp.Application.Services.Dto;
using System.Transactions;
using Abp.Runtime.Validation;
using System.Text.RegularExpressions;
using TicoPay.Invoices.XSD;
using BCR;

namespace TicoPay.Vouchers
{
    public class VoucherAppService : ApplicationService, IVoucherAppService
    {
        private readonly VoucherManager _VoucherManager;
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITenantAppService _tenantAppService;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IUserAppService _userAppService;
        RateOfDay _rateOfDay = new RateOfDay();
        SendMailTP mail = new SendMailTP(); // clase para envio de correo

        public const string subject = "Confirmación de Factura Electrónica";
        public const string emailbody = "<p>Adjunto al correo encontrará el comprobante de confirmación de su factura electrónica en formato  XML. Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su factura.</p>";
        public const string emailfooter = "<p>Para cualquier ayuda contáctenos a, soporte@ticopays.com</p> <p>Gracias</p> <p><b>Equipo TicoPay</b></p>";
        public const string emailbodySignature = @"<p>El comprobante de confirmación Nº {0} ha sido generado de forma exitosa, por favor complete los siguientes pasos para realizar 
                                                    su firma digital y el envío a hacienda: </p> <br/>
                                                    {1}
                                                    <p>Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su factura.</p>";
        public const string emailSteps = @"<p><h3>Paso 1:</h3>Descargue e instale la aplicación Firma TicoPay. 
                                            </p>
                                           <p><h3>Paso 2:</h3>Firme digitalmente sus comprobantes siguiendo las instrucciones del manual provisto.</p>
                                           <p><h3>Paso 3:</h3>Al realizar la firma, se enviará por correo de forma automática el comprobante electrónica a su proveedor en formato  XML, asi como su envío a hacienda. </p>
                                           <br/>";
        public VoucherAppService(VoucherManager VoucherManager, TenantManager tenantManager, IUnitOfWorkManager unitOfWorkManager, IInvoiceManager invoiceManager,
            ITenantAppService tenantAppService, IUserAppService userAppService)
        {
            _VoucherManager = VoucherManager;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantAppService = tenantAppService;
            _invoiceManager = invoiceManager;
            _userAppService = userAppService;
        }

        public string CleanInvalidCharacterFromExternalProvider(string xml)
        {
            if (xml!=null)
            {
                string pattern = @"[\u0000-\u001F]+";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(xml, replacement);
                xml = result;
            }
            return xml;
        }

        public VoucherDto downloadXML(HttpPostedFileBase file)
        {
            VoucherDto voucherDto = new VoucherDto();
            string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName).ToLower();

            //file.SaveAs(Server.MapPath("~/Uploads/" + archivo));

            //XmlSerializer serializer = new XmlSerializer(typeof(MiObjeto));
            //using (TextReader reader = new StringReader(elXML))
            //{
            //    MiObjeto result = (MiObjeto)serializer.Deserialize(reader);
            //}

            return voucherDto;
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

        public Uri SaveAzureStorageFromText(string documentname, string ruta, Stream content, string _container)
        {
            //para que se pasa la ruta

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

        private void loadXMLReceived(string documentname, string path, string content)
        {
            try
            {

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //var base64EncodedBytes = System.Convert.FromBase64String(content);
                //var contentfile = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                // Create the file.
                using (FileStream fs = File.Create(path))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(content);
                    fs.Write(info, 0, info.Length);
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Createvoucher(VoucherDto input)
        {
            if (input.Drawer == null)
            {
                throw new UserFriendlyException("Debe abrir una caja para la confirmación de XML.");
            }
            int TenantId;
            if (AbpSession.TenantId != null)
            {
                TenantId = AbpSession.TenantId.Value;
                var tenant = _tenantManager.Get(TenantId);
                var allRegisters = _invoiceManager.GetRegistersbyDrawer(input.Drawer.Id);

                Certificate certified = null;
                //colocar en el tenant  public long LastVoucherNumber { get; set; }

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    Voucher voucher = Voucher.Create(TenantId, input.IdentificationSender, input.NameSender, input.Email, input.NameReceiver, input.IdentificationReceiver,
                                                      input.ConsecutiveNumberInvoice, input.DateInvoice, input.Coin, input.Totalinvoice,
                                                      input.TotalTax, input.Message, input.DetailsMessage, false, input.TipoFirma, input.TypeVoucher, input.MessageSupplier, input.MessageTaxAdministrationSupplier);

                    if (input.IdentificationReceiver != tenant.IdentificationNumber)
                    {
                        throw new UserFriendlyException("La identificación del receptor no coincide con la identificación de la factura");
                    }

                    voucher.VoucherKeyRef = input.VoucherKeyRef;
                    int typeResponse = (int)voucher.Message + 5;
                    voucher.SetVoucherConsecutivo(allRegisters, tenant, typeResponse.ToString().PadLeft(2, '0'), false, input.Drawer);
                    voucher.SetVoucherNumberKey(voucher.CreationTime, voucher.ConsecutiveNumber, tenant, (int)VoucherSituation.Normal);

                    if (input.Drawer != null)
                    {
                        voucher.DrawerId = input.Drawer.Id;
                    }
                    SetFirmTypeRecurrente(input, tenant, voucher);



                    //Guarda el xml a confirmar en azure
                    #region SaveXML
                    string archivo = ("Received_" + tenant.Id.ToString() + "_" + input.VoucherKeyRef + ".xml").ToLower();
                    var ruta = Path.Combine(WorkPaths.GetXmlReceivedPath(), archivo);

                    loadXMLReceived(archivo, ruta, input.XLM);
                    Uri XMLReceived = SaveAzureStorage(archivo, Path.Combine(WorkPaths.GetXmlSignedPath(), archivo), "xmlvoucher");
                    #endregion

                    //Genera el xml (si el tipo de firma es llave)
                    certified = BeginCreateAndUploadXMl(tenant, certified, voucher);

                    _VoucherManager.Create(voucher);

                    _unitOfWorkManager.Current.SaveChanges();

                    //envía el xml a hacienda 
                    voucher = BeginValidateWithHacienda(tenant, certified, voucher);

                    //actualiza el consecutivo
                    voucher.SetInvoiceConsecutivo(allRegisters); // verificar esto

                    unitOfWork.Complete();

                    BeginSendMailAndTicoTalk(tenant, voucher, voucher.VoucherKey);
                }


            }

        }



        private Voucher BeginValidateWithHacienda(Tenant tenant, Certificate certified, Voucher voucher)
        {
            if (tenant.ValidateHacienda && voucher.TipoFirma == FirmType.Llave)
            {
                ValidateTribunet validateHacienda = new ValidateTribunet();
                //enviar factura a hacienda
                string validate = validateHacienda.SendResponseTribunet("POST", null, tenant, null, voucher.VoucherKey, null, voucher);

                if (validate == "-1")
                {
                    SetVoucherWithInternet(ref voucher);

                    Uri XML;
                    GenerateAndUploadXML(tenant, certified, voucher, out XML);

                    voucher.SetVoucherXML(XML);

                }
                else
                {
                    voucher.StatusVoucher = VoucherSituation.Normal;
                    voucher.SendVoucher = true;
                }
            }

            return voucher;
        }

        private void SetVoucherWithInternet(ref Voucher voucher)
        {
            voucher.StatusVoucher = VoucherSituation.withoutInternet;
            int _vouchersituation = (int)VoucherSituation.withoutInternet;
            voucher.VoucherKey = voucher.VoucherKey.Substring(0, 41) + _vouchersituation.ToString() + voucher.VoucherKey.Substring(42, 8);
            voucher.SendVoucher = false;

            var mensajereceptor = Voucher.CreateVoucherToSerialize(voucher, voucher.Tenant);
            Stream xmlStream = Voucher.GetXML(mensajereceptor);
            Uri XML = SaveAzureStorageFromText(voucher.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), "_voucher" + voucher.VoucherKey + ".xml"), xmlStream, "xmlvoucher");
        }

        private void GenerateAndUploadXML(Tenant tenant, Certificate certified, Voucher voucher, out Uri XML)
        {
            voucher.CreateXML(voucher, tenant, certified);

            XML = SaveAzureStorage(voucher.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), "voucher_" + voucher.VoucherKey + ".xml"), "xmlvoucher");

        }

        public Voucher UploadSignedXML(Voucher voucher, string xmlContent)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
            Uri XML = SaveAzureStorageFromText(voucher.VoucherKey + ".xml", Path.Combine(WorkPaths.GetXmlSignedPath(), voucher.VoucherKey + ".xml"), stream, "xmlvoucher"); //signed 

            voucher.StatusFirmaDigital = StatusFirmaDigital.Firmada;

            ValidateTribunet validateHacienda = new ValidateTribunet();
            //enviar factura a hacienda
            string validate = validateHacienda.SendResponseTribunet("POST", null, voucher.Tenant, null, voucher.VoucherKey, null, voucher);

            if (validate == "-1")
            {
                SetVoucherWithInternet(ref voucher);
                voucher.SetVoucherXML(XML);
            }
            else
            {
                voucher.StatusVoucher = VoucherSituation.Normal;
                voucher.SendVoucher = true;
            }

            _VoucherManager.Update(voucher);

            return voucher;
        }

        private static void SetFirmTypeRecurrente(VoucherDto input, Tenant tenant, Voucher voucher)
        {
            if (tenant.ValidateHacienda)
            {
                if (input.TipoFirma == FirmType.Firma)
                    voucher.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
            }
        }

        private Certificate BeginCreateAndUploadXMl(Tenant tenant, Certificate certified, Voucher voucher)
        {
            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (voucher.TipoFirma != FirmType.Firma)))
            {
                //obtiene el certificado si se valida con hacienda
                if (tenant.ValidateHacienda)
                    certified = _tenantAppService.GetCertified(tenant.Id);

                Uri XML;
                GenerateAndUploadXML(tenant, certified, voucher, out XML);

                voucher.SetVoucherXML(XML);
            }

            return certified;
        }

        private void BeginSendMailAndTicoTalk(Tenant tenant, Voucher voucher, string voucherk)
        {
            if (!(tenant.ValidateHacienda) || ((tenant.ValidateHacienda) && (voucher.TipoFirma != FirmType.Firma)))
            {

                if (voucher.Email != null)
                {
                    // Enviar Correo Electronico 
                    mail.SendMailTicoPay(voucher.Email, subject, emailbody, emailfooter, "",
                        Path.Combine(WorkPaths.GetXmlSignedPath(), "voucher_" + voucherk + ".xml"), tenant.AlternativeEmail, (tenant.ComercialName != null ? tenant.ComercialName : tenant.Name), string.Empty); //signed
                }

                TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient(tenant.Id, ConfigurationManager.AppSettings["TicoTalk:UserNameOrEmailAddress"], ConfigurationManager.AppSettings["TicoTalk:Password"]);


            }
            else
            {
                // Enviar Correo Electronico 
                mail.SendMailTicoPay(tenant.Email, subject, String.Format(emailbodySignature, voucher.ConsecutiveNumber, emailSteps), emailfooter, "", tenant.AlternativeEmail, (tenant.ComercialName != null ? tenant.ComercialName : tenant.Name), "");

            }
        }

        public List<VoucherDto> SearchVouchers(SearchVoucher searchInput)
        {
            //tolist
            //ordene consecutivo
            //int defaultPageSize = 10;
            //int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;
            var TenantId = AbpSession.TenantId.Value;
            var Tenant = _tenantAppService.GetById(TenantId);
            var CodigoMonedaTenant = Tenant.CodigoMoneda;

            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();

            if (drawers != null)
            {
                var predicate = PredicateBuilder.New<Voucher>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));

      

                predicate = predicate.And(a => a.TenantId == TenantId);

                if (searchInput.StartDueDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) >= searchInput.StartDueDate);

                if (searchInput.EndDueDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) <= searchInput.EndDueDate);

                if (searchInput.Status != null)
                    predicate = predicate.And(a => a.Message == searchInput.Status.Value);

                if (searchInput.StatusTribunet != null)
                {
                    predicate = predicate.And(a => a.StatusTribunet == searchInput.StatusTribunet);
                }
                if (searchInput.ConsecutiveNumber != null)
                {
                    var number = searchInput.ConsecutiveNumber.ToString().PadLeft(10, '0');
                    predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);
                }
                if (searchInput.Name != null)
                    predicate = predicate.And(a => a.NameSender == searchInput.Name);

                if (searchInput.StartDueDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) >= searchInput.StartDueDate);

                if (searchInput.EndDueDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) <= searchInput.EndDueDate);

                if (searchInput.BranchOfficeId != null)
                {
                    predicate = predicate.And(a => a.Drawer.BranchOfficeId == searchInput.BranchOfficeId);
                }

                if (searchInput.DrawerId != null)
                {
                    predicate = predicate.And(a => a.DrawerId == searchInput.DrawerId);
                }

                if (searchInput.Id != null)
                    predicate = predicate.And(a => a.Id == searchInput.Id);

                var MinimumAmount = 0;
                var MaxAmount = 0;

                if (searchInput.MinimumAmount == null)
                {
                    MinimumAmount = 0;
                }
                else
                {
                    MinimumAmount = (int?)searchInput.MinimumAmount ?? 0;
                }


                if (searchInput.MaxAmount == null)
                {
                    MaxAmount = 0;
                }
                else
                {
                    MaxAmount = (int?)searchInput.MaxAmount ?? 0; 
                }
               
              

                if (MinimumAmount > 0 || MaxAmount > 0)
                {

                    // obterner la tasa 
                    var fechaInicio = DateTimeZone.Now();
                    var fechaFin = DateTimeZone.Now();
                    ResultRateDto rate = _rateOfDay.GetDayRate(fechaInicio, fechaFin).FirstOrDefault();

                    var entities1 = _VoucherManager.GetAll().Where(predicate);

                    int i = 0;
                    if (CodigoMonedaTenant == FacturaElectronicaResumenFacturaCodigoMoneda.CRC)
                    {
                        var SumTotalVoucherCRC = entities1.Where(a => a.Coin == FacturaElectronicaResumenFacturaCodigoMoneda.CRC).Sum(a => (decimal?)a.Totalinvoice) ?? 0;
                        var SumTotalVoucherUSD = entities1.Where(a => a.Coin == FacturaElectronicaResumenFacturaCodigoMoneda.USD).Sum(a => (decimal?)a.Totalinvoice / rate.rate) ?? 0;


                        var Total = (SumTotalVoucherCRC + SumTotalVoucherUSD);

                        if (Total >= MinimumAmount || Total <= MaxAmount)
                        {
                            if (searchInput.StartDueDate != null)
                                predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) >= searchInput.StartDueDate);

                            if (searchInput.EndDueDate != null)
                                predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) <= searchInput.EndDueDate);
                        }
                        else
                        {
                            return null;
                        }


                    }
                    else if (CodigoMonedaTenant == FacturaElectronicaResumenFacturaCodigoMoneda.USD)
                    {

                        // obterner la tasa 
                        var SumTotalVoucherCRC = entities1.Where(a => a.Coin == FacturaElectronicaResumenFacturaCodigoMoneda.CRC).Sum(a => (decimal?)a.Totalinvoice) ?? 0;
                        var SumTotalVoucherUSD = entities1.Where(a => a.Coin == FacturaElectronicaResumenFacturaCodigoMoneda.USD).Sum(a => (decimal?)a.Totalinvoice * rate.rate) ?? 0;


                        var Total = (SumTotalVoucherCRC + SumTotalVoucherUSD);

                        if (Total >= MinimumAmount || Total <= MaxAmount)
                        {
                            if (searchInput.StartDueDate != null)
                                predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) >= searchInput.StartDueDate);

                            if (searchInput.EndDueDate != null)
                                predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) <= searchInput.EndDueDate);
                        }
                        else
                        {
                            return null;
                        }

                    }
                }

                var entities = _VoucherManager.GetAll().Where(predicate);

                var lista = (from v in entities
                             select new VoucherDto
                             {
                                 Id = v.Id,
                                 Coin = v.Coin,
                                 ConsecutiveNumber = v.ConsecutiveNumber,
                                 ConsecutiveNumberInvoice = v.ConsecutiveNumberInvoice,
                                 CreationTime = v.CreationTime,
                                 DateInvoice = v.DateInvoice,
                                 DetailsMessage = v.DetailsMessage,
                                 ElectronicBill = v.ElectronicBill,
                                 Email = v.Email,
                                 IdentificationReceiver = v.IdentificationReceiver,
                                 IdentificationSender = v.IdentificationSender,
                                 Message = v.Message,
                                 NameReceiver = v.NameReceiver,
                                 NameSender = v.NameSender,
                                 SendVoucher = v.SendVoucher,
                                 StatusFirmaDigital = v.StatusFirmaDigital,
                                 StatusTribunet = v.StatusTribunet,
                                 TipoFirma = v.TipoFirma,
                                 Totalinvoice = v.Totalinvoice,
                                 TotalTax = v.TotalTax,
                                 TypeVoucher = v.TypeVoucher,
                                 MessageTaxAdministration = v.MessageTaxAdministration,
                                 MessageSupplier = v.MessageSupplier,
                                 MessageTaxAdministrationSupplier = v.MessageTaxAdministrationSupplier
                             }).OrderBy(x => x.ConsecutiveNumber).ToList();

                if (lista == null)
                {
                    throw new UserFriendlyException("Could not found the voucher, maybe it's deleted.");
                }
                return lista.ToList();
            }
            else
            {
                return null;
            }




        }

        public List<VoucherDtoApi> SearchVouchersApi(SearchVoucherApi searchInput)
        {
            var drawers = (from c in _userAppService.getUserDrawers(null) select c.DrawerId).ToList();

            if (drawers != null)
            {
                var predicate = PredicateBuilder.New<Voucher>(true).And(x => drawers.Any(y => y.Value == x.DrawerId));


                if (searchInput.StatusFirmaDigital != null)
                    predicate = predicate.And(a => a.StatusFirmaDigital == searchInput.StatusFirmaDigital);

                if (searchInput.TipoFirma != null)
                    predicate = predicate.And(a => a.TipoFirma == searchInput.TipoFirma);

                if (searchInput.StatusTribunet != null)
                {
                    predicate = predicate.And(a => a.StatusTribunet == searchInput.StatusTribunet);
                }
                if (searchInput.ConsecutiveNumber != null)
                {
                    var number = searchInput.ConsecutiveNumber.ToString().PadLeft(10, '0');
                    predicate = predicate.And(a => a.ConsecutiveNumber.Substring(10) == number);
                }
                if (searchInput.Name != null)
                    predicate = predicate.And(a => a.NameSender == searchInput.Name);

                if (searchInput.StartDueDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) >= searchInput.StartDueDate);

                if (searchInput.EndDueDate != null)
                    predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreationTime) <= searchInput.EndDueDate);

                var entities = _VoucherManager.GetAll().Where(predicate);

                var lista = (from v in entities
                             select new VoucherDtoApi
                             {
                                 Id = v.Id,
                                 Coin = v.Coin,
                                 ConsecutiveNumber = v.ConsecutiveNumber,
                                 ConsecutiveNumberInvoice = v.ConsecutiveNumberInvoice,
                                 CreationTime = v.CreationTime,
                                 DateInvoice = v.DateInvoice,
                                 DetailsMessage = v.DetailsMessage,
                                 ElectronicBill = v.ElectronicBill,
                                 Email = v.Email,
                                 IdentificationReceiver = v.IdentificationReceiver,
                                 IdentificationSender = v.IdentificationSender,
                                 Message = v.Message,
                                 NameReceiver = v.NameReceiver,
                                 NameSender = v.NameSender,
                                 VoucherKey = v.VoucherKey,
                                 VoucherKeyRef = v.VoucherKeyRef,
                                 SendVoucher = v.SendVoucher,
                                 StatusFirmaDigital = v.StatusFirmaDigital,
                                 StatusTribunet = v.StatusTribunet,
                                 TipoFirma = v.TipoFirma,
                                 Totalinvoice = v.Totalinvoice,
                                 TotalTax = v.TotalTax
                             }).OrderBy(x => x.ConsecutiveNumber).ToList();

                if (lista == null)
                {
                    throw new UserFriendlyException("Could not found the voucher, maybe it's deleted.");
                }
                return lista;
            }
            else
            {
                throw new UserFriendlyException("Could not found the voucher, maybe it's deleted.");
            }

        }

        [UnitOfWork]
        [DisableValidation]
        public void SyncsInvoicesWithTaxAdministration(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            var pendingVouchers = _VoucherManager
                .GetAll()
                .Where(i => i.SendVoucher /*&& !i.IsDeleted */&& i.TenantId == tenant.Id && (i.StatusTribunet == 0 || i.StatusTribunet == StatusTaxAdministration.Recibido || i.StatusTribunet == StatusTaxAdministration.Procesando) && ((i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Firmada) || (i.TipoFirma == FirmType.Llave && i.StatusFirmaDigital == null)))
                .Include(i => i.Tenant)                
                .ToList();

            if (pendingVouchers != null && pendingVouchers.Count > 0)
            {
                ValidateTribunet validateTribunet = new ValidateTribunet();
                TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet,pss).Result;
                if (tokenResponse != null && !tokenResponse.IsError)
                {
                    foreach (var voucher in pendingVouchers)
                    {
                        var currentVoucher = _VoucherManager.Get(voucher.Id);
                        ElectronicBillResponse response = validateTribunet.GetComprobanteStatusFromTaxAdministration(currentVoucher, tokenResponse.AccessToken);
                        if (response != null)
                        {
                            if (response.IndEstado.ToLower() == "aceptado")
                                currentVoucher.StatusTribunet = StatusTaxAdministration.Aceptado;
                            else if (response.IndEstado.ToLower() == "rechazado")
                                currentVoucher.StatusTribunet = StatusTaxAdministration.Rechazado;
                            else if (response.IndEstado.ToLower() == "error")
                                currentVoucher.StatusTribunet = StatusTaxAdministration.Error;

                            currentVoucher.MessageTaxAdministration = response.RespuestaXml;

                            var result = _VoucherManager.Update(currentVoucher);

                            string consecutiveNumber = voucher.ConsecutiveNumber;
                            if (voucher.Tenant != null && !voucher.Tenant.ValidateHacienda)
                                consecutiveNumber = Convert.ToInt64(voucher.ConsecutiveNumber.Substring(10, 10)).ToString();

                            if (currentVoucher.StatusTribunet == StatusTaxAdministration.Aceptado)
                                SendTaxAdministrationAcceptedVoucherEmail(voucher.NameSender, voucher.Tenant.Name , voucher.VoucherKey, voucher.VoucherKeyRef, new string[] { voucher.Email, voucher.Tenant.Email, GetTenantUserAdminEmail(tenant.Id) }, response.RespuestaXml);
                            else if ((currentVoucher.StatusTribunet == StatusTaxAdministration.Rechazado)|| (currentVoucher.StatusTribunet == StatusTaxAdministration.Error))
                                SendTaxAdministrationVoucherEmail(voucher.Tenant.Name, voucher.VoucherKey, consecutiveNumber,currentVoucher.StatusTribunet, new string[] { voucher.Tenant.Email, GetTenantUserAdminEmail(tenant.Id) }, response.RespuestaXml);
                            
                        }
                    }

                    _unitOfWorkManager.Current.SaveChanges();
                }
            }
        }

        [DisableValidation]
        public void VouchersWithTaxAdministration(VoucherDto voucher)
        {
            Tenant tenant = _tenantManager.Get(AbpSession.TenantId.Value);
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            
            ValidateTribunet validateTribunet = new ValidateTribunet();
            TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet, pss).Result;
            if (tokenResponse != null && !tokenResponse.IsError)
            {
                Voucher currentVoucher = new Voucher {
                    ConsecutiveNumber = voucher.ConsecutiveNumberInvoice,
                    VoucherKey=voucher.VoucherKeyRef
                };
                ElectronicBillResponse response = validateTribunet.GetComprobanteStatusFromTaxAdministration(currentVoucher, tokenResponse.AccessToken);
                if (response != null)
                {
                    if (response.IndEstado.ToLower() == "aceptado")
                    { 
                        voucher.StatusTribunet = StatusTaxAdministration.Aceptado;
                        voucher.MessageSupplier = MessageSupplier.Aceptado;
                        voucher.MessageTaxAdministrationSupplier = response.RespuestaXml;
                    }
                    else if (response.IndEstado.ToLower() == "rechazado")
                    {
                        voucher.StatusTribunet = StatusTaxAdministration.Rechazado;
                        voucher.MessageSupplier = MessageSupplier.Rechazado;
                        voucher.MessageTaxAdministrationSupplier = response.RespuestaXml;
                    }
                    else if (response.IndEstado.ToLower() == "error")
                    voucher.StatusTribunet = StatusTaxAdministration.Error;
                    else if(response.IndEstado.ToLower() == "procesando")
                    voucher.StatusTribunet = StatusTaxAdministration.Procesando;

                    if (voucher.StatusTribunet == StatusTaxAdministration.Procesando)
                        voucher.isFile = false;
                    else
                        voucher.isFile = true;
                }
                else
                {
                    voucher.StatusTribunet = StatusTaxAdministration.NoEnviada;
                    voucher.isFile = false;
                }
            }
        }

        [UnitOfWork(isTransactional: false)]
        public void ResendFailedVouchers(Tenant tenant)
        {
            var pss = CryptoHelper.Desencriptar(tenant.PasswordTribunet);
            var notSendedVouchers = _VoucherManager
                .GetAll()
                .Where(i => !i.SendVoucher /*&& !i.IsDeleted*/ && i.ElectronicBill != null && i.TenantId == tenant.Id && ((i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Firmada) || (i.TipoFirma == FirmType.Llave && i.StatusFirmaDigital == null)))
                .Include(i => i.Tenant)
                .ToList();
            using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress,
                IsTransactional = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            }))
            {
                if (notSendedVouchers != null && notSendedVouchers.Count > 0)
                {
                    ValidateTribunet validateTribunet = new ValidateTribunet();
                    TokenResponse tokenResponse = validateTribunet.LoginAsync(tenant.UserTribunet, pss).Result;
                    if (tokenResponse != null && !tokenResponse.IsError)
                    {
                        foreach (var voucher in notSendedVouchers)
                        {
                            bool success = validateTribunet.ResendInvoice(voucher, null, voucher.CreationTime, voucher.Tenant, tokenResponse.AccessToken);
                            if (success)
                            {
                                voucher.SendVoucher = true;
                                _unitOfWorkManager.Current.SaveChanges();
                                //_VoucherManager.Update(voucher);
                            }
                        }
                       
                    }
                }
                unitOfWork.Complete();
            }
        }

        private string GetTenantUserAdminEmail(int tenantId)
        {
            string adminEmail = string.Empty;
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var admin = _userAppService.GetUserByRole("Admin", tenantId);
                if (admin != null)
                {
                    adminEmail = admin.EmailAddress;
                }
            }
            return adminEmail;
        }

        private void SendTaxAdministrationVoucherEmail(string tenantName, string voucherKey, string VoucherNumber, StatusTaxAdministration status, string[] to, string respuestaXml)
        {
            var statusresponse = status == StatusTaxAdministration.Error ? "reportado con error" : status.ToString(); 
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("<p>");
                body.AppendFormat("<p>Estimado {0}!</p>", tenantName);
                body.AppendFormat("<p>Le informamos que la confirmación de XML número {0} ha sido {1} por parte de la administración tributaria, por favor verifique.</p>", VoucherNumber, statusresponse);
                body.Append("<p>Se adjunta el documento de respuesta enviado por la administración tributaria.</p>");
                body.Append("<p>Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su factura.</p>");
                body.Append("<p>Para cualquier ayuda contáctenos a, soporte@ticopays.com</p> <p>Gracias</p> <p><b>Equipo TicoPay</b></p>");
                body.Append("</p>");

                var encodedTextBytes = Convert.FromBase64String(respuestaXml);
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);

                string xmlPath = SaveXmlToFile("Respuesta_" + voucherKey + ".xml", plainText);
                SendMailTP sendMail = new SendMailTP();
                sendMail.SendNoReplyMail(to, string.Format("Confirmación de XML - {0}",status.ToString()), body.ToString(), new string[] { xmlPath });
            }
            catch (Exception)
            {
            }
        }            

        private void SendTaxAdministrationAcceptedVoucherEmail(string clientName, string tenantName, string voucherKey, string invoiceNumber, string[] to, string respuestaXml)
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("<p>");
                body.AppendFormat("<p>Estimado {0}!</p>", clientName);
                body.AppendFormat("<p>Le informamos que la confirmación de XML clave {0}, emitido por {1}, ha sido aceptada por la administración tributaria.</p>", invoiceNumber, tenantName);
                body.Append("<p>Se adjunta el documento de respuesta enviado por la administración tributaria.</p>");
                body.Append("<p>Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a su factura.</p>");
                body.Append("<p>Para cualquier ayuda contáctenos a, soporte@ticopays.com</p> <p>Gracias</p> <p><b>Equipo TicoPay</b></p>");
                body.Append("</p>");

                var encodedTextBytes = Convert.FromBase64String(respuestaXml);
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);

                string xmlPath = SaveXmlToFile("Respuesta_" + voucherKey + ".xml", plainText);
                SendMailTP sendMail = new SendMailTP();
                sendMail.SendNoReplyMail(to, "Confirmación XML Aceptada", body.ToString(), new string[] { xmlPath });
            }
            catch (Exception)
            {
            }
        }

        private string SaveXmlToFile(string fileName, string xml)
        {
            string path = Path.Combine(WorkPaths.GetXmlSignedPath(), fileName);
            XmlDocument document = new XmlDocument() { PreserveWhitespace = true };
            document.LoadXml(xml);
            document.Save(path);
            return path;
        }

        public bool isDigitalPendingVoucher(int tenatId)
        {
            bool isPending = false;

            var invoice = _VoucherManager.GetAll().Where(i => (i.TipoFirma == FirmType.Firma && i.StatusFirmaDigital == StatusFirmaDigital.Pendiente) && i.TenantId == tenatId).FirstOrDefault();
            if (invoice != null)
                isPending = true;
            return isPending;
        }

        public bool isExistsVoucher(string keyref, string IdentificationSender, int tenatId)
        {
            bool isExists = false;

            var Voucher = _VoucherManager.GetAll().Where(i => i.VoucherKeyRef == keyref && i.IdentificationSender== IdentificationSender && i.TenantId == tenatId && (i.StatusTribunet!= StatusTaxAdministration.Rechazado && i.StatusTribunet != StatusTaxAdministration.Error)).FirstOrDefault();
            if (Voucher != null)
                isExists = true;
            return isExists;
        }

        public Voucher Get(Guid id)
        {
            var @voucher = _VoucherManager.Get(id);            
                        
            if (@voucher == null)
            {
                throw new UserFriendlyException("Could not found the voucher, maybe it's deleted!");
            }
            return @voucher;
        }
    }
}
