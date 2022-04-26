using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Vouchers.Dto;
using PagedList;
using System.Xml.Serialization;
using System.IO;
using System.Xml.XPath;
using TicoPay.Invoices.XSD;
using System.Xml;
using TicoPay.Vouchers;
using TicoPay.Common;
using System.Text;
using System.Threading.Tasks;
using TicoPay.MultiTenancy;

using static TicoPay.MultiTenancy.Tenant;
using TicoPay.Application.Helpers;
using TicoPay.Drawers;
using Abp.UI;
using TicoPay.Invoices;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Abp.Web.Mvc.Models;
using Abp.Web.Models;

//using Abp.Domain.Uow;
//using Abp.Dependency;
//using Abp.Web.Mvc.Models;
//using Abp.Web.Models;
//using Abp.UI;


namespace TicoPay.Web.Controllers
{
    public class VoucherController :  TicoPayControllerBase
    {
        public readonly IVoucherAppService _voucherAppClient;
        private readonly TenantManager _tenantManager;
        public readonly IDrawersAppService _drawersAppService;
        private readonly VoucherManager _VoucherManager;


        public VoucherController(IVoucherAppService voucherAppClient, TenantManager tenantManager, IDrawersAppService drawersAppService, VoucherManager voucherManager)
        {
            _voucherAppClient = voucherAppClient;
            _tenantManager = tenantManager;
            _drawersAppService = drawersAppService;
            _VoucherManager = voucherManager;

        }

        // GET: Voucher
        public ActionResult Index()
        {

            SearchVoucher model = new SearchVoucher();
            try
            {
                var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
                model.isKey = tenant.ValidateHacienda;
                model.StartDueDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                model.EndDueDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                model.Entities = _voucherAppClient.SearchVouchers(model);
                model.Drawer = _drawersAppService.getUserDrawersOpen();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                ViewBag.isOpenDrawer = (model.Drawer != null);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(-1, ex.Message) });
            }
            //model.Entities = getVouchers().ToPagedList(1, 10);
            return View(model);
        }

        public ActionResult Search(SearchVoucher model)
        {
            if (ModelState.IsValid)
            {
                //var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
                model.Page = 1;
                //model.isKey = tenant.ValidateHacienda;
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.Entities = _voucherAppClient.SearchVouchers(model);
                model.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (model.Drawer != null);
            }
            return View("Index", model);
        }

        public ActionResult Create()
        {
            
         
            VoucherDto model = initializeVoucher();
            try
            {
                if (model.ListType == null)
                {
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = "La compañía no tiene asiganda el tipo de firma, por favor verifique en el módulo de configuración de compañía.";
                }
            }
            catch (Exception ex)
            {

                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = ex.GetBaseException().Message;
            }
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(VoucherDto model)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    if (_voucherAppClient.isExistsVoucher(model.VoucherKeyRef, model.IdentificationSender, AbpSession.TenantId.GetValueOrDefault())) 
                    {
                        model.ErrorCode = ErrorCodeHelper.Error;
                        model.ErrorDescription = "Este comprobante ha sido confirmado previamente, por favor verifique.";
                    }
                    else
                    {
                        model.XLM = Base64Decode(model.XLM);
                        if ((model.TipoFirma == FirmType.Llave) && (_voucherAppClient.isDigitalPendingVoucher(AbpSession.TenantId.GetValueOrDefault())))
                        {
                            model.ErrorCode = ErrorCodeHelper.Error;
                            model.ErrorDescription = "Existen comprobantes pendientes por firma Digital";
                        }
                        else
                        {
                            model.Drawer = _drawersAppService.getUserDrawersOpen();
                            _voucherAppClient.Createvoucher(model);
                            VoucherDto NewModel = initializeVoucher();
                            NewModel.ErrorCode = ErrorCodeHelper.Ok;
                            NewModel.ErrorDescription = "Comprobantes de confirmación generado de forma exitosa.";
                            return View("Create",NewModel);
                        }
                    }
                    
                  
                }
                else{
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = "Datos no válido: "+ ModelState.ToErrorMessage();
                   
                }
            }
            catch (Exception ex)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = ex.Message;
               
            }
            finally
            {
                if (model.Drawer == null)
                {
                    model.Drawer = _drawersAppService.getUserDrawersOpen(); 
                }
                ViewBag.isOpenDrawer = (model.Drawer != null);
            }
            model = getTypeFirm(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Subir(VoucherDto model)
        {
            //  if (file == null) return;
            //VoucherDto model = new VoucherDto();
            //string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName).ToLower();
            //var ruta = Server.MapPath("~/Uploads/" + archivo);
            //file.SaveAs(ruta);
            model.isFile = false;
         
            try
            {
                XmlTextReader reader = null;
                byte[] byteArray = null;
                MemoryStream stream = null;
                StreamReader dirtyReader = new StreamReader(model.File.InputStream);
                string dirtyXML = dirtyReader.ReadToEnd();
                dirtyReader.Close();

                XmlDocument doc = new XmlDocument();
                XmlElement nav = null;

                byteArray = Encoding.UTF8.GetBytes(dirtyXML);
                stream = new MemoryStream(byteArray);
                reader = new XmlTextReader(stream);
                try
                {
                    doc.Load(reader);
                    reader.Close();
                    nav = doc.DocumentElement;
                }
                catch (System.Xml.XmlException ex)
                {
                    if (ex.Message.Contains("hexadecimal") && ex.Message.Contains("is an invalid character"))
                    {
                        string cleanXML = _voucherAppClient.CleanInvalidCharacterFromExternalProvider(dirtyXML);
                        byteArray = Encoding.UTF8.GetBytes(cleanXML);
                        stream = new MemoryStream(byteArray);
                        reader = new XmlTextReader(stream);
                        try
                        {
                            doc.Load(reader);
                            reader.Close();
                            nav = doc.DocumentElement;
                        }
                        catch (System.Xml.XmlException ex2)
                        {
                            if (ex2.Message.Contains("hexadecimal") && ex2.Message.Contains("is an invalid character"))
                            {
                                throw new UserFriendlyException("El documento XML cargado tiene uno o mas caracteres invalidos:" + ex2.Message + ". Contacte a su emisor para mas detalles.");
                            }
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException("Error en el formato del documento electrónico, contacte a su emisor para mas detalles.");
                    }
                }
                catch (Exception)
                {
                    throw new UserFriendlyException("Error cargando el archivo XML, por favor verifique que el formato del archivo sea correcto.");
                }

                model.Type = getTypeVouche(nav.Name);
                model = getTypeFirm(model);
                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("msbld", nav.NamespaceURI);
                //model.TipoFirma = Tenant.FirmType.Llave;

                if (nav.SelectSingleNode("//msbld:MensajeHacienda", ns) != null)
                {
                    throw new UserFriendlyException("Hemos detectado un XML de respuesta de hacienda, los documentos electrónicos que puede confirmar son: Factura, Tiquete, Notas de Crédito y Débito.");
                }
                model.IsTypeDocument = true;
                model.TypeVoucher = TypeVoucher.Purchases;
                model.VoucherKeyRef = nav.SelectSingleNode("//msbld:Clave", ns).InnerText;
                model.NameSender = nav.SelectSingleNode("//msbld:Emisor/ msbld:Nombre", ns).InnerText;
                model.Email = nav.SelectSingleNode("//msbld:Emisor/ msbld:CorreoElectronico", ns) != null ? nav.SelectSingleNode("//msbld:Emisor/ msbld:CorreoElectronico", ns).InnerText : string.Empty;
                model.IdentificationSender = nav.SelectSingleNode("//msbld:Emisor/ msbld:Identificacion/ msbld:Numero", ns).InnerText;
                if (nav.SelectSingleNode("//msbld:Receptor/ msbld:Nombre", ns) != null)
                {
                    model.NameReceiver = nav.SelectSingleNode("//msbld:Receptor/ msbld:Nombre", ns).InnerText;
                    var identificationext = nav.SelectSingleNode("//msbld:Receptor/ msbld:IdentificacionExtranjero", ns) != null ? nav.SelectSingleNode("//msbld:Receptor/ msbld:IdentificacionExtranjero", ns).InnerText : string.Empty;
                    model.IdentificationReceiver = nav.SelectSingleNode("//msbld:Receptor/ msbld:Identificacion/ msbld:Numero", ns) != null ? nav.SelectSingleNode("//msbld:Receptor/ msbld:Identificacion/ msbld:Numero", ns).InnerText : identificationext;
                }
                else
                {
                    model.NameReceiver = string.Empty;
                    model.IdentificationReceiver = string.Empty;
                }           
                model.ConsecutiveNumberInvoice = nav.SelectSingleNode("//msbld:NumeroConsecutivo", ns).InnerText;
                model.DateInvoice = Convert.ToDateTime(nav.SelectSingleNode("//msbld:FechaEmision", ns).InnerText);
                if (nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:TotalImpuesto", ns)!=null)
                    model.TotalTax = Convert.ToDecimal(nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:TotalImpuesto", ns).InnerText);
                model.Totalinvoice = Convert.ToDecimal(nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:TotalComprobante", ns).InnerText);
                if (nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:CodigoMoneda", ns) != null)
                    model.Coin = (FacturaElectronicaResumenFacturaCodigoMoneda)TicoPay.Application.Helpers.EnumHelper.Parse(typeof(FacturaElectronicaResumenFacturaCodigoMoneda), nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:CodigoMoneda", ns).InnerText);
                //else
                //    model.Coin =  FacturaElectronicaResumenFacturaCodigoMoneda.CRC;
                model.XLM = Base64Encode(doc.InnerXml);
                _voucherAppClient.VouchersWithTaxAdministration(model);
                if (model.StatusTribunet.Equals(StatusTaxAdministration.NoEnviada))
                {
                    ViewBag.showStatus = false;
                    model.ErrorCode = ErrorCodeHelper.None;
                    model.ErrorDescription = "El documento electrónico que se cargo no ha sido recibido por Hacienda.";
                }else if (model.StatusTribunet.Equals(StatusTaxAdministration.Procesando))
                {
                    ViewBag.showStatus = false;
                    model.ErrorCode = ErrorCodeHelper.None;
                    model.ErrorDescription = "El documento electrónico que cargo fue recibido en Hacienda, pero no tiene respuesta de aceptación o rechazo.";
                }
                else
                {
                    ViewBag.showStatus = true;
                }
            }
            catch (UserFriendlyException ex)
            {
                model.ErrorCode = ErrorCodeHelper.Error; 
                model.ErrorDescription = ex.Message;
                return View("Create", model);
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = " Error en formato del XML. Por favor verifique.";
                return View("Create", model);
            }
            finally
            {
                model.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (model.Drawer != null);
            }
            
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            var file = new StreamReader(this.HttpContext.Request.InputStream, Encoding.UTF8).BaseStream;
           // var file =this.HttpContext.Request;

            VoucherDto model = new VoucherDto();
            //string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + file.FileName).ToLower();
            //var ruta = Server.MapPath("~/Uploads/" + archivo);
            //file.SaveAs(ruta);


            try
            {
                XmlTextReader reader = new XmlTextReader(file);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();
                XmlElement nav = doc.DocumentElement;

                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("msbld", nav.NamespaceURI);

                model.VoucherKeyRef = nav.SelectSingleNode("//msbld:Clave", ns).InnerText;
                model.NameSender = nav.SelectSingleNode("//msbld:Emisor/ msbld:Nombre", ns).InnerText;
                model.Email = nav.SelectSingleNode("//msbld:Emisor/ msbld:CorreoElectronico", ns) != null ? nav.SelectSingleNode("//msbld:Emisor/ msbld:CorreoElectronico", ns).InnerText : string.Empty;
                model.IdentificationSender = nav.SelectSingleNode("//msbld:Emisor/ msbld:Identificacion/ msbld:Numero", ns).InnerText;
                if (nav.SelectSingleNode("//msbld:Receptor/ msbld:Nombre", ns) != null)
                {
                    model.NameReceiver = nav.SelectSingleNode("//msbld:Receptor/ msbld:Nombre", ns).InnerText;
                    var identificationext = nav.SelectSingleNode("//msbld:Receptor/ msbld:IdentificacionExtranjero", ns) != null ? nav.SelectSingleNode("//msbld:Receptor/ msbld:IdentificacionExtranjero", ns).InnerText : string.Empty;
                    model.IdentificationReceiver = nav.SelectSingleNode("//msbld:Receptor/ msbld:Identificacion/ msbld:Numero", ns) != null ? nav.SelectSingleNode("//msbld:Receptor/ msbld:Identificacion/ msbld:Numero", ns).InnerText : identificationext;
                }
                else
                {
                    model.NameReceiver = string.Empty;
                    model.IdentificationReceiver = string.Empty;
                }
                model.ConsecutiveNumberInvoice = nav.SelectSingleNode("//msbld:NumeroConsecutivo", ns).InnerText;
                model.DateInvoice = Convert.ToDateTime(nav.SelectSingleNode("//msbld:FechaEmision", ns).InnerText);
                if (nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:TotalImpuesto", ns) != null)
                    model.TotalTax = Convert.ToDecimal(nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:TotalImpuesto", ns).InnerText);
                model.Totalinvoice = Convert.ToDecimal(nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:TotalComprobante", ns).InnerText);
                if (nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:CodigoMoneda", ns) != null)
                    model.Coin = (FacturaElectronicaResumenFacturaCodigoMoneda)TicoPay.Application.Helpers.EnumHelper.Parse(typeof(FacturaElectronicaResumenFacturaCodigoMoneda), nav.SelectSingleNode("//msbld:ResumenFactura/ msbld:CodigoMoneda", ns).InnerText);
                model.XLM = Base64Encode(doc.InnerXml);
                model = getTypeFirm(model);
                ViewBag.showStatus = true;

            }
            catch (Exception ex)
            {
                model = initializeVoucher();
                model.ErrorCode = ErrorCodeHelper.Error; ;
                model.ErrorDescription = " Error en formato del XML. " + ex.Message;
             
            }
          
            return PartialView("_formPartial", model);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public ActionResult AjaxPage(int page, MessageVoucher? status, Invoices.StatusTaxAdministration? statusTribunet, string proveedor, string comprobante, DateTime? startDueDate, DateTime? endDueDate, bool isKey)
        {
            //var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
            SearchVoucher model = new SearchVoucher();
            model.Page = page;
            model.Name = proveedor;
            model.ConsecutiveNumber = comprobante;
            model.EndDueDate = endDueDate;
            model.StartDueDate = startDueDate;
            model.Status=  status;
            model.StatusTribunet = statusTribunet;
            model.isKey = isKey;

            model.Entities = _voucherAppClient.SearchVouchers(model);
            model.BranchOffice = _drawersAppService.getUserbranch().ToList();
            var drawerUser = _drawersAppService.getUserDrawers(null);
            model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();

            return PartialView("_listPartial", model);
        }

        private VoucherDto initializeVoucher()
        {
               
            VoucherDto model = new VoucherDto();
            model.Type = "[Documento]";
            model.NameSender = "[NOMBRE EMISOR]";
            model.NameReceiver = "[NOMBRE RECEPTOR]";
            model.IdentificationReceiver = "[############]";
            model.IdentificationSender = "[############]";
            model.Email = "[Correo]";
            model.ConsecutiveNumberInvoice = "[############]";
            model.DateInvoice = new DateTime(1900, 1, 1);
            model.Totalinvoice = 0;
            model.TotalTax = 0;
            model.isFile = false;
            model.VoucherKeyRef = string.Empty;
            model.DetailsMessage = string.Empty;
            model.Message = MessageVoucher.Aceptado;
            model = getTypeFirm( model);
            model.Drawer = _drawersAppService.getUserDrawersOpen();
            model.IsTypeDocument = true;
            model.TypeVoucher = TypeVoucher.Purchases;
            ViewBag.isOpenDrawer = (model.Drawer != null);
            return model;
        }

        private VoucherDto getTypeFirm(VoucherDto model)
        {
            var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
            model.Coin = tenant.CodigoMoneda;
            if (tenant.TipoFirma != null)
            {
                if (tenant.TipoFirma == FirmType.Todos)
                    model.ListType = EnumHelper.GetSelectListValues(typeof(FirmType)).Take(2);
                else
                    model.ListType = EnumHelper.GetSelectList(tenant.TipoFirma);
                model.TipoFirma = model.TipoFirma == null ? tenant.FirmaRecurrente : model.TipoFirma;
            }
           
            
            return model;
        }

        private string getTypeVouche(string root)
        {
            string type = string.Empty;
            switch (root)
            {
                case "NotaCreditoElectronica":
                    type = "Nota Crédito Electrónica";
                    break;
                case "NotaDebitoElectronica":
                    type = "Nota Débito Electrónica";
                    break;
                case "FacturaElectronica":
                    type = "Factura Electrónica";
                    break;
                default:
                    type = "Ticket Electrónico";
                    break;
            }
            return type;
        }


        public ActionResult DownloadVoucher(Guid id)
        {
            var voucher = _VoucherManager.Get(id);
            Guid idVoucher = new Guid(Convert.ToString(voucher.Id));
            string containerName = "xmlvoucher";
            string fileName = voucher.VoucherKey + ".xml";

            var xml = LoadFileFromAzureStorage(containerName, voucher.ElectronicBill);

            StreamReader reader = new StreamReader(xml);
            string text = reader.ReadToEnd();

            MemoryStream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var fileStreamResult = new FileStreamResult(xmlStream, "text/xml")
            {
                FileDownloadName = fileName
            };

            return fileStreamResult;
        }

        public ActionResult DescargarRespuestaXml(Guid id)
        {
            SearchVoucher model = new SearchVoucher
            {
                Id = id
            }; 

            var entities = _voucherAppClient.SearchVouchers(model);
            string messageTaxAdministration = entities.FirstOrDefault().MessageTaxAdministration.ToString();
            string fileName = entities.FirstOrDefault().ConsecutiveNumberInvoice + ".xml";
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

        public ActionResult DescargarRespuestaDocumentoXml(Guid id)
        {
            SearchVoucher model = new SearchVoucher
            {
                Id = id
            };

            var entities = _voucherAppClient.SearchVouchers(model);
            string MessageTaxAdministrationSupplier = entities.FirstOrDefault().MessageTaxAdministrationSupplier.ToString();
            string fileName = entities.FirstOrDefault().ConsecutiveNumberInvoice + ".xml";
            var encodedTextBytes = Convert.FromBase64String(MessageTaxAdministrationSupplier);
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