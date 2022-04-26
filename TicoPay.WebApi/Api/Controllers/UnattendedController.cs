using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TicoPay.Api.Common;
using TicoPay.MultiTenancy;
using TicoPay.Unattended;
using TicoPay.Unattended.Dto;
using TicoPay.Users;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la consulta y Firmado de XML
    /// </summary>
    [AbpAuthorize]
    [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
    public class UnattendedController : TicoPayApiController
    {
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly UnattendedAppService _unattendedAppService;

        /// <exclude />
        public UnattendedController(UserManager userManager, TenantManager tenantManager, UnattendedAppService unattendedAppService)
        {
            _userManager = userManager;
            _tenantManager = tenantManager;
            _unattendedAppService = unattendedAppService;
        }

        /// <summary>
        /// Recibe el XML del Cliente y lo Firma.
        /// </summary>
        /// <remarks>Recibe el XML del Cliente y lo Firma</remarks>
        /// <param name="useConsecutive">true si se usa el consecutivo generado por TicoPay false si el cliente usa su propio consecutivo, el cliente debe generar su propia clave y enviar su nodo respectivo en el XML</param>
        /// <param name="invoiceDate">La fecha que realiza la facturación.</param>
        /// <param name="xml">El XML en bruto</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el XML Firmado-> (objectResponse)", Type = typeof(TicoPayResponseAPI<UnattendedApiDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos del XML", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Países en la lista", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("ReciveAndSendXml")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> ReciveAndSendXml(bool useConsecutive, DateTime invoiceDate, [FromBody] string xml)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            try
            {
                UnattendedNotification unattendedNotification;
                var tenant = _tenantManager.Get((int)AbpSession.TenantId);
                try
                {
                    unattendedNotification = _unattendedAppService.SendXmlTribunet(useConsecutive, tenant, xml, invoiceDate);
                    if (unattendedNotification.UnattendedApiDto != null)
                    {
                        var unattended = unattendedNotification.UnattendedApiDto;
                        return Ok(new TicoPayResponseAPI<UnattendedApiDto>(HttpStatusCode.OK, unattended));
                    }
                    else
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, unattendedNotification.MensajeRespuesta));
                    }
                }
                catch (UserFriendlyException ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.Code.ToString(), ex.Message));
                }                
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtener respuesta de hacienda.
        /// </summary>
        /// <remarks>Obteniene la repuesta de Hacienda del XML firmado</remarks>
        /// <param name="voucherKey">La clave del documento electrónico</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna XML y Respuesta de Hacienda -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UnattendedStatusToTribunet>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con el VoucherKey", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Países en la lista", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("StatusFromComprobante")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> StatusFromComprobante(string voucherKey)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            try
            {
                var tenant = _tenantManager.Get((int)AbpSession.TenantId);
                try
                {
                    var unattended = _unattendedAppService.UnattendedWithTaxAdministration(voucherKey,tenant);
                    return Ok(new TicoPayResponseAPI<UnattendedStatusToTribunet>(HttpStatusCode.OK, unattended));
                }
                catch (UserFriendlyException ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.Code.ToString(), ex.Message));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtener respuesta de hacienda.
        /// </summary>
        /// <remarks>Crea una nueva factura con los datos proporcionados</remarks>
        /// <param name="voucherKey">La clave del documento electrónico</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna factura creada en -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UnattendedStatusToTribunet>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de la factura", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Países en la lista", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("GetUnattendedPDF/{voucherKey}")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUnattendedPDF(string voucherKey)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            try
            {
                var tenant = _tenantManager.Get((int)AbpSession.TenantId);
                try
                {
                    var unattended = _unattendedAppService.ObtenerPDF(voucherKey, tenant);
                    return Ok(new TicoPayResponseAPI<UnattendedStatusToTribunet>(HttpStatusCode.OK, unattended));
                }
                catch (UserFriendlyException ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.Code.ToString(), ex.Message));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }
    }
}
