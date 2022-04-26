using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.WebApi.Controllers;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TicoPay.Api.Common;
using TicoPay.Api.Models;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices.XSD;
using TicoPay.ReportInvoicesSentToTribunet;
using TicoPay.ReportInvoicesSentToTribunet.Dto;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la consulta de Reportes / Method that manages reports
    /// </summary>
    /// <seealso cref="TicoPay.Api.Common.TicoPayApiController" />
    [AbpAuthorize]
    [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
    public class ReportsController : TicoPayApiController
    {
        private readonly IReportInvoicesSentToTribunetAppService _reportInvoicesSentToTribunetAppService;
        private readonly IClientAppService _clientAppClient;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <exclude />
        public ReportsController(IReportInvoicesSentToTribunetAppService reportInvoicesSentToTribunetAppService, IClientAppService clientAppClient, IUnitOfWorkManager unitOfWorkManager)
        {
            _reportInvoicesSentToTribunetAppService = reportInvoicesSentToTribunetAppService;
            _clientAppClient = clientAppClient;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Reporte de Facturas enviadas a Hacienda / Status Report of the invoices sent to Hacienda.
        /// </summary>
        /// <param name="input">Parámetros de Búsqueda / Search parameters.</param>
        /// <param name="type">Envié "pdf" para recibir el documento en pdf , si es enviado con otro valor retorna el json con las facturas / 
        /// Send "pdf" to obtain a pdf file , any other value will return a json with the data. 
        /// </param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Facturas Encontradas / Returns the Invoice List -> (Invoices)", Type = typeof(TicoPayResponseAPI<InvoiceSentToTribunetModel>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota", Type = typeof(TicoPayResponseErrorAPI))]
        // [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen facturas que cumplan con los parámetros", Type = typeof(TicoPayResponseErrorAPI))]
        // [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("InvoicesSentToTribunet")]
        [HttpGet]
        public HttpResponseMessage InvoicesSentToTribunet([FromUri] ReportInvoicesSentToTribunetSearchInput input, string type = null)
        {
            ReportInvoicesSentToTribunetOutput result = _reportInvoicesSentToTribunetAppService.Search(input);
            GenericPDFGenerator PdfInvoie = new GenericPDFGenerator();

            var list = result.Invoices.ConvertAll(i => new InvoiceSentToTribunetModel(i));

            if (!string.IsNullOrWhiteSpace(type) && "pdf".Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                Stream pdfStream = PdfInvoie.CreateGenericReportList("FACTURAS ENVIADAS A TRIBUNET", list);
                return StreamToFile(pdfStream, "Listado de facturas enviadas a tribunet.pdf", MediaTypeNames.Application.Pdf);
            }

            var jsonObj = new
            {
                ReportName = "FACTURAS ENVIADAS A TRIBUNET",
                PrintDate = $"Fecha de emisión: {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}",
                Invoices = list
            };            
            return JsonResponse(JsonConvert.SerializeObject(jsonObj));
        }

        /// <summary>
        /// Reporte de Facturas enviadas a Hacienda / Status Report of the invoices sent to Hacienda.
        /// </summary>
        /// <param name="input">Parámetros de Búsqueda / Search parameters.</param>
        /// <param name="type">Envié "pdf" para recibir el documento en pdf , si es enviado con otro valor retorna el json con las facturas.
        /// / Send "pdf" to obtain a pdf file , any other value will return a json with the data. 
        /// </param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Facturas Encontradas en -> (Invoices)", Type = typeof(TicoPayResponseAPI<InvoiceSentToTribunetModel>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de nota", Type = typeof(TicoPayResponseErrorAPI))]
        // [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen facturas que cumplan con los parámetros", Type = typeof(TicoPayResponseErrorAPI))]
        // [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("InvoicesSentToTribunetPost")]
        [HttpPost]
        public HttpResponseMessage InvoicesSentToTribunetPost(ReportInvoicesSentToTribunetSearchInput input, string type = null)
        {
            ReportInvoicesSentToTribunetOutput result = _reportInvoicesSentToTribunetAppService.Search(input);
            GenericPDFGenerator PdfInvoie = new GenericPDFGenerator();

            var list = result.Invoices.ConvertAll(i => new InvoiceSentToTribunetModel(i));

            if (!string.IsNullOrWhiteSpace(type) && "pdf".Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                Stream pdfStream = PdfInvoie.CreateGenericReportList("FACTURAS ENVIADAS A TRIBUNET", list);
                return StreamToFile(pdfStream, "Listado de facturas enviadas a tribunet.pdf", MediaTypeNames.Application.Pdf);
            }

            var jsonObj = new
            {
                ReportName = "FACTURAS ENVIADAS A TRIBUNET",
                PrintDate = $"Fecha de emisión: {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}",
                Invoices = list
            };
            return JsonResponse(JsonConvert.SerializeObject(jsonObj));
        }
    }
}
