using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TicoPay.Api.Common;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.MultiTenancy;
using TicoPay.ReportsSettings;
using TicoPay.Services.Dto;
using TicoPay.Users;
using System.Linq;
using Abp.UI;
using TicoPay.Common;
using static TicoPay.MultiTenancy.Tenant;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http.Description;
using System.IO;
using TicoPay.Invoices.XSD;
using System.Net.Mime;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using AutoMapper;
using TicoPay.Printers;

namespace TicoPay.Api.Controllers
{

    /// <summary>
    /// Conjunto de Métodos que manejan la consulta y creación de Facturas , notas y otros documentos / Methods that manage the Invoices an Notes
    /// </summary>
    [AbpAuthorize]
    [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
    public class InvoiceController : TicoPayApiController
    {
        private readonly IInvoiceAppService _invoiceAppService;
        public readonly IClientAppService _clientAppClient;
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly IIocResolver _iocResolver;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IReportSettingsAppService _reportSettingsAppService;
        private ReportSettings _facturaReportSettings;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Note, Guid> _noteRepository;
        private readonly ITenantAppService _tenantAppService;
        SendMail.SendMailTP mail = new SendMail.SendMailTP(); // clase para envió de correo

        /// <exclude />
        public InvoiceController(IInvoiceAppService invoiceAppService, IClientAppService clientAppClient, TenantManager tenantManager, UserManager userManager, IIocResolver iocResolver, IInvoiceManager invoiceManager, IReportSettingsAppService reportSettingsAppService, IRepository<Invoice, Guid> invoiceRepository, IRepository<Note, Guid> noteRepository, ITenantAppService tenantAppService)
        {
            _invoiceAppService = invoiceAppService;
            _clientAppClient = clientAppClient;
            _tenantManager = tenantManager;
            _userManager = userManager;
            _iocResolver = iocResolver;
            _invoiceManager = invoiceManager;
            _reportSettingsAppService = reportSettingsAppService;
            _invoiceRepository = invoiceRepository;
            _noteRepository = noteRepository;
            _tenantAppService = tenantAppService;
        }

        /// <summary>
        /// Crea una nueva Factura / Creates a New Invoice.
        /// </summary>
        /// <remarks>Crea una nueva factura con los datos proporcionados / Creates a new invoice with the information provided</remarks>
        /// <param name="input">
        /// Datos de la Factura :
        /// Para Crear una factura a Crédito debe enviar el campo InvoicePaymentTypes con 0 elementos (null) y debe enviar el campo CreditTerm con valor mayor a 0
        /// Invoice Information : To create a Credit Invoice you must send the Fields InvoicePaymentTypes in null (0 Elements) and must send the field CreditTerm with a value greater than 0
        /// </param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna factura creada / Returns the newly created invoice -> (objectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de la factura "+
            "/ Returns this message if there are errors in some of the invoice fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Países en la lista", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("PostAsync")]
        [HttpPost]
        public async Task<IHttpActionResult> PostAsync(CreateInvoiceDTO input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            ModelState.AddValidationErrors(input, _iocResolver);

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage() , ModelState.ToErrorMessage()));

            try
            {
                ModelState.Clear();
                //User user = null;
                //Tenant output = null;

                //user = await _userManager.FindByNameOrEmailAsync(User.Identity.GetUserName());

                //output = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());

                //if (!user.IsEmailConfirmed)
                //    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.EMAILCONFIRM));

                //if (!(output.BarrioId != null && output.CountryID != null && output.Address != null))
                //    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.ADRESSINCOMPLETE));

                //CreateInvoiceInput invoiceClss = new CreateInvoiceInput();
                //invoiceClss.ClientId = input.ClientId;
                //invoiceClss.InvoiceLines = input.InvoiceLines;

                try
                {
                    await _invoiceAppService.TenantCantDoInvoices(User.Identity.GetUserName(), input.TipoFirma);
                }
                catch (UserFriendlyException ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.Code.ToString(), ex.Message));
                }

                var invoice = _invoiceAppService.Create(input);
                return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, invoice));
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }



        /// <summary>
        /// Crea un nuevo Tiquete / Creates a new Ticket.
        /// </summary>
        /// <remarks>Crea un nuevo tiquete con los datos proporcionados / Creates a new Ticket with the information provided</remarks>
        /// <param name="input">
        /// Datos del Tiquete :
        /// Para Crear un tiquete a Crédito debe enviar el campo InvoicePaymentTypes con 0 elementos (null) y debe enviar el campo CreditTerm con valor mayor a 0
        /// Invoice Information : To create a Credit Invoice you must send the Fields InvoicePaymentTypes in null (0 Elements) and must send the field CreditTerm with a value greater than 0
        /// </param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna tiquete creado / Returns the newly created ticket -> (objectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos del tiquete "+
            "/ Returns this message when there are errors in some of the ticket fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Países en la lista", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("PostTicketAsync")]
        [HttpPost]
        public async Task<IHttpActionResult> PostTicketAsync(CreateInvoiceDTO input)
        {
            
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            input.TypeDocument = TypeDocumentInvoice.Ticket;
            ModelState.Clear();
            ModelState.AddValidationErrors(input, _iocResolver);

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                ModelState.Clear();               

                try
                {
                    await _invoiceAppService.TenantCantDoInvoices(User.Identity.GetUserName(), input.TipoFirma);
                }
                catch (UserFriendlyException ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.Code.ToString(), ex.Message));
                }

                var invoice = _invoiceAppService.Create(input);
                return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, invoice));
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Paga una Factura o Tiquete a crédito Pendiente / Pays a Credit Invoice or ticket that is Pending.
        /// </summary>
        /// <remarks>Paga una factura de Crédito o Tiquete a Crédito, debe realizar el pago completo / Pays a Credit Invoice or Ticket , must be paid in full</remarks>
        /// <param name="IdInvoiceOrTicket">Id de la factura o tiquete / Invoice or Ticket Id.</param>
        /// <param name="Pagos">Lista con el o los pagos realizados / Payment methods list.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Factura o Tiquete Pagado / Returns the Paid Invoice or Ticket -> (objectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de pago o la factura o tiquete ya fue pagada "+
            "/ Returns this message when the Payment Data has errors or the Invoice or ticket has already been paid", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la factura o tiquete a pagar "+
            "/ Returns this message if the Invoice or Ticket doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays / Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("PayInvoiceOrTicket")]
        [HttpPost]
        public IHttpActionResult PayInvoiceOrTicket(Guid IdInvoiceOrTicket, List<PaymentInvoceDto> Pagos)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            
            ModelState.Clear();
            var invoice = _invoiceAppService.Get(IdInvoiceOrTicket);            

            if (invoice == null)
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "La Factura o Tiquete no puede ser encontrada"));
            }

            if (invoice.Balance <= 0)
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "La factura o tiquete ya fue pagada"));
            }

            try
            {               
                if(Pagos != null)
                {
                    if (Pagos.Count == 0)
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Debe enviar la lista de pagos"));
                    }
                    try
                    {
                        _invoiceAppService.PayInvoiceList(Pagos, IdInvoiceOrTicket);       
                        var PaidInvoice = _invoiceAppService.GetPaidInvoice(IdInvoiceOrTicket);
                        return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, PaidInvoice));
                    }
                    catch
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El total de pagos no corresponde con el balance de la factura o tiquete"));
                    }                    
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Debe enviar la lista de pagos"));
                }
                
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Obtiene la ESC/POS data de impresión de una factura o tiquete / Gets the Print ESC/POS Data or an Invoice or Ticket.
        /// </summary>
        /// <remarks>
        /// Obtiene la data de impresión en lenguaje ESC/POS para una factura o Tiquete / Gets the Print ESC/POS Data or an Invoice or Ticket.
        /// </remarks>
        /// <param name="IdInvoiceOrTicket">Id del la Factura o Tiquete a obtener la data de impresión / Invoice or ticket to Print Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Data en lenguaje ESC/POS para la impresión de la Factura o Tiquete "+
            "/ Returns the ESC/POS Data of the Invoice or ticket-> (objectResponse)", Type = typeof(TicoPayResponseAPI<string>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de pago o la factura o tiquete ya fue pagada", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la factura o tiquete a imprimir "+
            "/ Returns this message when the Invoice or Ticket doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("PrintDataForInvoiceOrTicket")]
        [HttpPost]
        public IHttpActionResult PrintDataForInvoiceOrTicket(Guid IdInvoiceOrTicket)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            var invoice = _invoiceAppService.GetInvoice(IdInvoiceOrTicket);

            if (invoice == null)
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "La Factura o Tiquete no puede ser encontrada"));
            }            
            try
            {
                var tenant = _tenantAppService.GetById((int) AbpSession.TenantId);
                DocumentPrint documento = new DocumentPrint(invoice);
                string dataImpresion;
                if(tenant.IsPos == true)
                {
                    switch (tenant.PrinterType)
                    {
                        case PrinterTypes.Epson:
                            Epson impresoraEpson = new Epson();
                            dataImpresion = impresoraEpson.print(documento);
                            break;
                        case PrinterTypes.Epson_TMT20II:
                            Epson_TMT20II impresoraTMT20II = new Epson_TMT20II();
                            dataImpresion = impresoraTMT20II.print(documento);
                            break;
                        case PrinterTypes.MatrizPunto:
                            Matriz impresoraMatriz = new Matriz();
                            dataImpresion = impresoraMatriz.print(documento);
                            break;
                        case PrinterTypes.Bematech_LR200:
                            Bematech_LR200 impresoraBematechLR200 = new Bematech_LR200();
                            dataImpresion = impresoraBematechLR200.print(documento);
                            break;
                        case PrinterTypes.BTP_R880NP:
                            BTP_R880NP impresoraBtpR880NP = new BTP_R880NP();
                            dataImpresion = impresoraBtpR880NP.print(documento);
                            break;
                        case PrinterTypes.XP_58iih:
                            XP_58iih impresoraXP58iih = new XP_58iih();
                            dataImpresion = impresoraXP58iih.print(documento);
                            break;
                        case PrinterTypes.Zebra_iMZ320:
                            Zebra_iMZ320 impresoraZebraimz320 = new Zebra_iMZ320();
                            dataImpresion = impresoraZebraimz320.print(documento);
                            break;
                        case PrinterTypes.Zebra_ZQ320:
                            Zebra_ZQ320 impresoraZebrazq320 = new Zebra_ZQ320();
                            dataImpresion = impresoraZebrazq320.print(documento);
                            break;
                        case PrinterTypes.POS_5890c:
                            XP_58iih impresoraPOS_5890c = new XP_58iih();
                            dataImpresion = impresoraPOS_5890c.print(documento);
                            break;
                        case PrinterTypes.N3Star_PPT300BT:
                            Bematech_LR200 impresoraN3Star_PPT300BT = new Bematech_LR200();
                            dataImpresion = impresoraN3Star_PPT300BT.print(documento);
                            break;
                        case PrinterTypes.POS_5805DD:
                            POS_5805DD impresorPOS_5805DD = new POS_5805DD();
                            dataImpresion = impresorPOS_5805DD.print(documento);
                            break;
                        default:
                            Epson impresoraEpsonDefecto = new Epson();
                            dataImpresion = impresoraEpsonDefecto.print(documento);
                            break;
                    }
                    dataImpresion = dataImpresion.Replace("\r\n", "$intro$");
                    return Ok(new TicoPayResponseAPI<string>(HttpStatusCode.OK, dataImpresion));
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "No tiene configurada una impresora de punto de venta"));
                }
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }


        /// <summary>
        /// Búsqueda de Facturas o Tiquetes / Invoice or ticket Search.
        /// </summary>
        /// <remarks>
        /// Busca facturas or tiquetes del Tenant de acuerdo a los parámetros proporcionados / Search for the Invoices or Tickets of the Tenant
        /// </remarks>
        /// <param name="input">Conjunto de Parámetros para la búsqueda. (En caso de no poner Fecha en Desde , solo se buscaran en los últimos dos meses)
        /// Search Parameters (In case you don't fill the Start Date , the search will only returns the documents of the last 2 months)
        /// </param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de facturas que cumplen los criterios / Returns the Invoice or ticket list -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de búsqueda "+
            "/ Returns this message when there are errors in the search parameters", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen facturas que cumplan con los parámetros "+
            "/ Returns this message when no Invoice or Tickets fulfill the Search Parameters", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("Post")]
        [HttpPost]
        public IHttpActionResult GetInvoices(SearchInvoicesApi input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));


            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
            try
            {
                var invoices = _invoiceAppService.SearchInvoicesApi(input);
                if (invoices == null)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
                }
                else
                {
                    return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, null, invoices));
                }
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Aplicar una Nota de Crédito o Débito a un documento / Applies a Credit or Debit Memo to an Invoice or ticket.
        /// </summary>
        /// <remarks>
        /// Aplica un Nota de Crédito o Débito a un documento, El tipo de nota se define en el Campo NoteType del input / 
        /// Applies a Credit or Debit Memo to an Invoice or ticket , The Memo Type is defined in the NoteType Field in the input
        /// </remarks>
        /// <param name="input">Datos de la nota a generar / Memo Information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la nota creada / Returns the Memo newly Created -> (objectResponse)", Type = typeof(TicoPayResponseAPI<NoteDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota "+
            "/ Returns this message when there are errors in some of the Note Fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen facturas que cumplan con los parámetros", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("ApplyNote")]
        [HttpPost]
        public IHttpActionResult ApplyNote([FromBody] NoteDto input,bool dontModifyBalance = false)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, message: ModelState.ToErrorMessage()));
            }
            try
            {
                var note = _invoiceAppService.CreateNote(input,dontModifyBalance);
                return Ok(new TicoPayResponseAPI<NoteDto>(HttpStatusCode.OK, note));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtiene las Notas de Crédito o Débito / Credit or Debit Memo Search.
        /// </summary>
        /// <remarks>
        /// Obtiene las Notas de Crédito o Débito de acuerdo a los parámetros solicitados / Gets the Debit or Credit Memos according to the Search Parameters
        /// </remarks>
        /// <param name="input">Parámetros de las notas a buscar / Memo Search Parameters.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de notas encontradas / Returns the Memo List -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<NoteApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda "+
            "/ Returns this message when there are errors with the Search Parameters", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen notas que cumplan con los parámetros "+
            "/ Returns this message when there are no Memos that match the search parameters", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("Post")]
        [HttpPost]
        public IHttpActionResult GetNotes(SearchNotesApi input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));


            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
            try
            {
                var notes = _invoiceAppService.SearchNotesApi(input);
                if (notes == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<NoteApiDto>(HttpStatusCode.OK, null, notes));
                }
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Aplica notas en Batch.
        /// </summary>
        /// <remarks>
        /// Aplica una lista de notas de Crédito o Débito
        /// </remarks>
        /// <param name="list">Lista de notas a Aplicar.</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la nota creada en -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ApplyNoteInput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de las notas", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen facturas que cumplan con los parámetros", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays / Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("ApplyNoteInBatch")]
        [HttpPost]
        public IHttpActionResult ApplyNoteInBatch([FromBody] List<NoteDto> list)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, message: ModelState.ToErrorMessage()));
            }
            try
            {               
                foreach (var input in list)
                {
                    _invoiceAppService.CreateNote(input);
                }
                return Ok(new TicoPayResponseAPI<ApplyNoteInput>(HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Aplica el Reverso de la Factura (Nota de Crédito de la factura completa) / Applies a Credit Memo to an Invoice or Ticket to Void it.
        /// </summary>
        /// <param name="idInvoiceOrTicket">ID de la factura o tiquete a Reversar / Invoice or Ticket Id.</param>
        /// <param name="NumReferenciaExterna">Numero de referencia externa de la Nota de Crédito / Credit Memo External Reference number.</param>
        /// <returns></returns>
        /// <remarks>
        /// Aplica el Reverso a una factura dejando su estado de Ticopays en Reversada (Nota de Crédito de la factura completa) /
        /// Applies a Credit Memo to an Invoice or Ticket for the Entire amount to Void it.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la nota creada / Returns the newly created Memo -> (objectResponse)", Type = typeof(TicoPayResponseAPI<NoteDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si la factura o tiquete posee notas (No se puede reversar) "+
            "/ Returns this message when the Invoice or Ticket can't be Voided", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la factura a Reversar "+
            "/ Returns this message when the Invoice or Ticket to Void doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("ApplyReverse")]
        [HttpPost]
        public IHttpActionResult ApplyReverse(Guid idInvoiceOrTicket, string NumReferenciaExterna = "N/A")
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            var invoice = _invoiceAppService.Get(idInvoiceOrTicket);

            if (invoice == null)
            {
                return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "La Factura o Tiquete a reversar no puede ser encontrada"));
            }
            SearchNotesApi buscarNotas = new SearchNotesApi();
            buscarNotas.InvoiceId = idInvoiceOrTicket;
            var notes = _invoiceAppService.SearchNotesApi(buscarNotas);
            if (notes.Items.Count == 0)
            {
                NoteDto notaAReversar;
                bool afectarBalance = false;
                if(invoice.Balance <= 0)
                {
                    afectarBalance = true;
                }
                try
                {
                    notaAReversar = _invoiceAppService.CreateReverseNoteFromInvoice(invoice,NumReferenciaExterna);
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Imposible armar una nota de Reverso en base a la factura o tiquete"));
                }
                try
                {
                    var note = _invoiceAppService.CreateNote(notaAReversar,afectarBalance);
                    return Ok(new TicoPayResponseAPI<NoteDto>(HttpStatusCode.OK, note));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
                }
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "La Factura o Tiquete a reversar tiene ya notas aplicadas, no se puede reversar"));
            }
        }

        /// <summary>
        /// Reenvía los correos de las facturas enviadas por parámetro / Resend the Email of the selected invoices.
        /// </summary>
        /// <remarks>
        /// Reenvía los correos de la lista de facturas enviadas por parámetro / Resend the Email of the selected invoices.
        /// </remarks>
        /// <param name="invoices">Lista de IDs de Facturas a Reenviar / List of invoices Id to send.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si envió los correos / Returns null if all email were sent -> (objectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que alguna de las facturas enviadas no existen " +
            "/ Returns this message if some of the invoices don't exist", Type = typeof(TicoPayResponseErrorAPI))]
        // [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("Resend")]
        [HttpPost]
        public IHttpActionResult Resend(List<Guid> invoices)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            try
            {
                foreach (var item in invoices)
                {
                    _invoiceAppService.ResendInvoice(item);
                }

                return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Obtiene el XML de una Factura / Gets the Invoice XML.
        /// </summary>
        /// <remarks>
        /// Obtiene el XML de una factura / Gets the Invoice XML
        /// </remarks>
        /// <param name="id">Id de Factura / Invoice Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el XML de la factura / Returns the Invoice Unsigned XML", Type = typeof(string))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la factura solicitada no existe " +
            "/ Returns this message when the requested Invoice doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        // [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [Route("GetXML/{id}")]
        public System.Net.Http.HttpResponseMessage GetXML(Guid id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED)));
                }

                var invoice = _invoiceManager.Get(id);

                if (invoice != null && invoice.TipoFirma != null && invoice.TipoFirma == Tenant.FirmType.Firma && (invoice.StatusFirmaDigital == StatusFirmaDigital.Pendiente || invoice.StatusFirmaDigital == StatusFirmaDigital.Error))
                {
                    invoice.DueDate = DateTimeZone.Now();
                    var facturaElectronica = Invoice.CreateInvoiceToSerialize(invoice, invoice.Client, invoice.Tenant, null);
                    var xmlStream = Invoice.GetXML(facturaElectronica);
                    return StreamToFile(xmlStream, invoice.VoucherKey + ".xml", System.Net.Mime.MediaTypeNames.Text.Xml);
                }
                else
                {
                    return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND)));
                }
            }
            catch (Exception ex)
            {
                return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message)));
            }
        }

        /// <summary>
        /// Actualiza el XML de una Factura de Firma Digital / Updates the Invoice Digital Signature.
        /// </summary>
        /// <remarks>
        /// Actualiza el XML de una Factura de Firma Digital una vez fue firmado / Updates the Invoice Digital Signature after signing it.
        /// </remarks>
        /// <param name="id">ID de la Factura a Actualizar / Invoice Id.</param>
        /// <param name="xmlContent">XML a Cargar / Signed XML to Load.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si actualizo el XML / Returns null if the XML was updated-> (objectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si la factura a Actualizar no es de FirmType Firma Digital o si el Estatus de la Firma no es Pendiente o Error " +
            "/ Return this message if the Invoice to Update Signature Type is not Digital Signature or if the Signature Status is different than Pending or Error", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la factura a actualizar no existe " +
            "/ Returns this message when the invoice to update doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("UpdateXML/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateXML(Guid id, [FromBody] string xmlContent)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            Invoice invoice = _invoiceManager.Get(id);
            if(invoice == null)
            {
                return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
            }
            try
            {
                if (invoice != null && invoice.TipoFirma != null && invoice.TipoFirma == Tenant.FirmType.Firma && (invoice.StatusFirmaDigital == StatusFirmaDigital.Pendiente || invoice.StatusFirmaDigital == StatusFirmaDigital.Error))
                {
                    await _invoiceAppService.SaveXMLFirmaDigital(invoice, xmlContent);
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
                }

                return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                invoice.StatusFirmaDigital = StatusFirmaDigital.Error;

                await _invoiceRepository.UpdateAsync(invoice);
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtiene el XML de una Nota / Gets the XML of a Memo.
        /// </summary>
        /// <remarks>
        /// Obtiene el XML de una Nota de Crédito o Débito / Gets the XML of a Credit or Debit Memo
        /// </remarks>
        /// <param name="id">ID de la Nota / Memo Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el XML de la nota / Returns the Memo Unsigned XML", Type = typeof(string))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la nota solicitada no existe " +
            "/ Returns this message when the requested Memo doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [Route("GetNoteXML/{id}")]
        public System.Net.Http.HttpResponseMessage GetNoteXML(Guid id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED)));
                }

                var note = _noteRepository.Get(id);

                if (note != null && note.TipoFirma != null && note.TipoFirma == Tenant.FirmType.Firma && (note.StatusFirmaDigital == StatusFirmaDigital.Pendiente || note.StatusFirmaDigital == StatusFirmaDigital.Error))
                {
                    note.CreationTime = DateTimeZone.Now();
                    if (note.NoteType == NoteType.Debito)
                    {
                        var notaDebito = Note.CreateNoteDebitoToSerialize(note.Invoice, note.Invoice.Client, note.Invoice.Tenant, note);
                        return StreamToFile(Note.GetXML(notaDebito), note.VoucherKey + ".xml", System.Net.Mime.MediaTypeNames.Text.Xml);
                    }
                    else
                    {
                        var notaCredito = Note.CreateNoteCreditoToSerialize(note.Invoice, note.Invoice.Client, note.Invoice.Tenant, note);
                        return StreamToFile(Note.GetXML(notaCredito), note.VoucherKey + ".xml", System.Net.Mime.MediaTypeNames.Text.Xml);
                    }
                }
                else
                {
                    return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND)));
                }
            }
            catch (Exception ex)
            {
                return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message)));
            }
        }

        /// <summary>
        /// Actualiza el XML de una Nota de Firma Digital / Updates the Memo XML with the Digital Signature.
        /// </summary>
        /// <remarks>
        /// Actualiza el XML de una Nota de Firma Digital una vez fue firmado / Updates the Memo XML after it has been signed with a Digital Signature
        /// </remarks>
        /// <param name="id">ID de la Nota / Memo Id.</param>
        /// <param name="xmlContent">XML de la Nota Firmado / Signed XML to load.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si actualizo el XML / Returns null if the XML was successfully updated -> (objectResponse)", Type = typeof(TicoPayResponseAPI<InvoiceApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si la nota a Actualizar no es de FirmType Firma Digital o si el Estatus de la Firma no es Pendiente o Error " +
            "/ Returns this message when the Memo to update signature Type is not Digital Signature or if the Signature Status is different than Pending or Error ", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la nota a actualizar no existe " +
            "/ Returns this message if the Memo to update doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("UpdateNoteXML/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateNoteXML(Guid id, [FromBody] string xmlContent)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            Note note = _noteRepository.Get(id);
            try
            {
                if (note != null && note.TipoFirma != null && note.TipoFirma == Tenant.FirmType.Firma && (note.StatusFirmaDigital == StatusFirmaDigital.Pendiente || note.StatusFirmaDigital == StatusFirmaDigital.Error))
                {
                    await _invoiceAppService.SaveXMLFirmaDigital(note, xmlContent);
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
                }

                return Ok(new TicoPayResponseAPI<InvoiceApiDto>(HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                note.StatusFirmaDigital = StatusFirmaDigital.Error;

                await _noteRepository.UpdateAsync(note);
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtiene el Pdf the una factura o tiquete / Gets the Invoice or Ticket PDF.
        /// </summary>
        /// <param name="id">Id de la factura o tiquete / Invoice or Ticket Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la información del PDF de la factura o tiquete / Returns the Invoice or Ticket PDF Information -> (objectResponse)", Type = typeof(TicoPayResponseAPI<TicopayPDFDto>))]        
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la factura o tiquete no existe " +
            "/ Returns this message if the Invoice or Ticket doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [Route("GetInvoicePDF/{id}")]
        public IHttpActionResult GetInvoicePDF(Guid id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }

                var invoice = _invoiceManager.Get(id);
                Client client;
                if(invoice.ClientId != null)
                {
                    client = _invoiceAppService.GetClientPdf(invoice.Client.Id);
                }
                else
                {
                    client = null;
                }
                
                var tenant = _tenantManager.Get(invoice.TenantId);

                if (invoice != null && tenant != null && invoice.ElectronicBillPDF != null)
                {
                    _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);
                    GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
                    Stream pdfStream = generatePDF.CreatePDFAsStream(invoice, client, tenant, null, null);

                    TicopayPDFDto ticopayPDFDto = new TicopayPDFDto { FileName = invoice.VoucherKey, Data = pdfStream.ToByteArray() };
                    return Ok(new TicoPayResponseAPI<TicopayPDFDto>(HttpStatusCode.OK, ticopayPDFDto, null));
                }
                else if (invoice != null && invoice.ElectronicBillPDF == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.BUILDINGPDF));
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtiene el Archivo Pdf the una factura o tiquete / Gets the Invoice or Ticket PDF.
        /// </summary>
        /// <param name="id">Id de la factura o tiquete / Invoice or Ticket Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Archivo PDF de la factura o tiquete / Returns the Invoice or Ticket PDF File-> (objectResponse)", Type = typeof(TicoPayResponseAPI<TicopayPDFDto>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la factura o tiquete no existe " +
            "/ Returns this message if the Invoice or Ticket doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [Route("GetInvoicePDFFile/{id}")]
        public HttpResponseMessage GetInvoicePDFFile(Guid id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                var invoice = _invoiceManager.Get(id);
                Client client;
                if (invoice.ClientId != null)
                {
                    client = _invoiceAppService.GetClientPdf(invoice.Client.Id);
                }
                else
                {
                    client = null;
                }

                var tenant = _tenantManager.Get(invoice.TenantId);

                if (invoice != null && tenant != null)
                {
                    _facturaReportSettings = _reportSettingsAppService.GetReportSettingsByReportName(tenant.Id, TicoPayReports.Factura, tenant.ComercialName);
                    GeneratePDF generatePDF = new GeneratePDF(_facturaReportSettings);
                    Stream pdfStream = generatePDF.CreatePDFAsStream(invoice, client, tenant, null, null);
                    TicopayPDFDto ticopayPDFDto = new TicopayPDFDto { FileName = invoice.VoucherKey, Data = pdfStream.ToByteArray() };                   
                    return StreamToFile(pdfStream, invoice.VoucherKey + ".pdf", MediaTypeNames.Application.Pdf);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        /// <summary>
        /// Aplica el Reverso de la Nota / Applies a Memo to a Memo to Void it.
        /// </summary>
        /// <remarks>
        /// Aplica el Reverso a una nota / Applies a Memo to a Memo to Void it
        /// </remarks>
        /// <param name="input">Datos de la Nota / Memo Information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la nota creada / Returns the newly created Memo -> (objectResponse)", Type = typeof(TicoPayResponseAPI<NoteDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota " +
            "/ Returns this message when there are errors with the Memo Information", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro " +
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen facturas que cumplan con los parámetros", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("ApplyReverse")]
        [HttpPost]
        public IHttpActionResult ApplyReverseNote([FromBody] NoteDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, message: ModelState.ToErrorMessage()));
            }
            try
            {
                var note = Mapper.Map<NoteDto>(_invoiceAppService.ReverseNote(input));
                return Ok(new TicoPayResponseAPI<NoteDto>(HttpStatusCode.OK, note));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }
    }
}
