using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TicoPay.Api.Common;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.UI;
using TicoPay.Vouchers;
using TicoPay.Vouchers.Dto;
using static TicoPay.MultiTenancy.Tenant;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using TicoPay.MultiTenancy;
using TicoPay.Invoices;
using System.IO;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan los Comprobantes electrónicos / Methods that manage Electronic Vouchers
    /// </summary>
    [AbpAuthorize]
    [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
    public class VoucherController : TicoPayApiController
    {
        private readonly VoucherAppService _voucherAppService;
        private readonly IRepository<Voucher, Guid> _voucherRepository;

        /// <exclude />
        public VoucherController(VoucherAppService voucherAppService, IRepository<Voucher, Guid> voucherRepository)
        {
            _voucherAppService = voucherAppService;
            _voucherRepository = voucherRepository;
        }

        /// <summary>
        /// Obtener comprobantes electrónicos / Gets Electronic Vouchers.
        /// </summary>
        /// <param name="input">Estructura de parámetros opcionales para la personalización de la búsqueda de comprobantes. En caso de no querer utilizar un parámetro de la estructura colocar Null en su valor
        /// Search Parameters used to refine the Voucher Search , In case a Field filter is not needed place the value null in it.
        /// </param>
        /// <remarks>Obtiene los comprobantes electrónicos registrados por el Tenant / Gets the Electronic Voucher of the Tenant</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Comprobantes encontrados / Returns a List of Vouchers Found-> (listObjectResponse)", Type= typeof(TicoPayResponseAPI<VoucherDtoApi>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados "+
            "/ Returns this message if there are errors in the Search Parameters",Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Comprobantes que cumplas con esos parámetros "+
            "/ Returns this message when there are no Vouchers that fulfill the requirements", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("Post")]        
        [HttpPost]
        public IHttpActionResult GetVouchers(SearchVoucherApi input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));


            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
            try
            {
                var vouchers = _voucherAppService.SearchVouchersApi(input);
                if (vouchers == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {                    
                    return Ok(new TicoPayResponseAPI<VoucherDtoApi>(HttpStatusCode.OK, null, new ListResultDto<VoucherDtoApi>(vouchers.MapTo<List<VoucherDtoApi>>())));
                }
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Obtener el XML de un comprobante electrónico / Gets the Electronic Voucher XML.
        /// </summary>
        /// <param name="id">Id del comprobante electrónico / Electronic Voucher Id.</param>
        /// <remarks>Obtiene el XML de un comprobante electrónico especifico que halla sido cargado con Firma Electrónica 
        /// / Gets a specific Electronic Voucher XML that was loaded with Electronic Signature
        /// </remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el XML de un Comprobante Electrónico / Returns the Electronic Voucher XML -> (objectResponse)", Type = typeof(TicoPayResponseAPI<string>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el comprobante solicitado o si su tipo de firma no es Firma Electrónica "+
            "/ Returns this message when the Voucher requested doesn't exist or the Signature Type is not Electronic Signature", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [Route("GetVoucherXML/{id}")]
        public System.Net.Http.HttpResponseMessage GetVoucherXML(Guid id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return JsonResponse(JsonConvert.SerializeObject(new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED)));
                }

                var voucher = _voucherAppService.Get(id);

                if (voucher != null && voucher.TipoFirma != null && voucher.TipoFirma == Tenant.FirmType.Firma && (voucher.StatusFirmaDigital == StatusFirmaDigital.Pendiente || voucher.StatusFirmaDigital == StatusFirmaDigital.Error))
                {                                         
                    var xmlTextStream =  voucher.GenerateXML(voucher, voucher.Tenant);
                    return StreamToFile(xmlTextStream, voucher.VoucherKey + ".xml", System.Net.Mime.MediaTypeNames.Text.Xml);
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
        /// Actualiza el XML Firmado de un comprobante electrónico / Updates the XML of an Electronic Voucher.
        /// </summary>
        /// <param name="id">Id del comprobante electrónico / Electronic Voucher Id.</param>
        /// <param name="xmlContent">XML Firmado / Signed XML.</param>
        /// <remarks>Actualiza el XML Firmado de un comprobante electrónico que haya sido cargado con Firma Digital 
        /// / Updates the XML of an Electronic Voucher that was uploaded with Electronic Signature</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si el XML Firmado del comprobante fue cargado "+
            "/ Returns true if the Electronic Voucher was successfully Updated -> (objectResponse)", Type = typeof(TicoPayResponseAPI<VoucherDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el comprobante a actualizar no fue encontrado o si el mismo no utiliza Firma Digital "+
            "/ Returns this message when the Electronic Voucher doesn't exist or its Digital Signature type is not Electronic Signature", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]        
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Route("UpdateVoucherXML/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateVoucherXML(Guid id, [FromBody] string xmlContent)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            Voucher voucher = _voucherAppService.Get(id);
            try
            {
                if (voucher != null && voucher.TipoFirma != null && voucher.TipoFirma == Tenant.FirmType.Firma && (voucher.StatusFirmaDigital == StatusFirmaDigital.Pendiente || voucher.StatusFirmaDigital == StatusFirmaDigital.Error))
                {
                    _voucherAppService.UploadSignedXML(voucher, xmlContent);
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST));
                }

                return Ok(new TicoPayResponseAPI<VoucherDto>(HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                voucher.StatusFirmaDigital = StatusFirmaDigital.Error;

                await _voucherRepository.UpdateAsync(voucher);
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }
    }
}
