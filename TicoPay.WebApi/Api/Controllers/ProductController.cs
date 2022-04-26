using Abp.WebApi.Controllers;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TicoPay.Application.Helpers;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.GroupConcept;
using TicoPay.GroupConcept.Dto;
using TicoPay.Groups;
using TicoPay.Inventory;
using TicoPay.Inventory.Dto;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la Consulta, Creación, Actualización y Eliminación de Productos / Methods that manages Products
    /// </summary>
    [Abp.Runtime.Validation.DisableValidation]
    public class ProductController : AbpApiController
    {
        private readonly IInventoryAppServices _productAppService;


        /// <exclude />
        public ProductController(IInventoryAppServices productAppService)
        {
            _productAppService = productAppService;

        }

        /// <summary>
        /// Obtiene Todos los Productos / Gets all Products.
        /// </summary>
        /// <remarks>
        /// Obtiene Todos los Productos del Tenant / Gets all Products.
        /// </remarks>
        /// <param name="detallado">Si es <c>true</c> Trae todos los Datos del Producto, sino solo trae los Indispensables 
        /// / If <c>true</c> Gets all product information , If not , Gets only the main Fields.
        /// </param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Productos / Returns a Product List -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<ProductDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Productos creados "+
            "/ Returns this message when there are no Products Created", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll(bool detallado)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var productos = _productAppService.GetProductos();

                if (productos == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<ProductDto>(HttpStatusCode.OK, null, productos));

            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene un Producto Especifico / Gets a Specific Product.
        /// </summary>
        /// <remarks>
        /// Obtiene los datos un Producto Especifico / Gets a Specific Product.
        /// </remarks>
        /// <param name="detallado">Si es <c>true</c> Trae todos los Datos del Producto, sino solo trae los Indispensables /
        /// If <c>true</c> Gets all Product information, otherwise gets the Main Fields.
        /// </param>
        /// <param name="Id">Id del Producto / Product Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Producto / Returns the Product -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ProductDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Producto especificado "+
            "/ Returns this message if the Product doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(bool detallado, Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var producto = _productAppService.Get(Id);

                if (producto == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<ProductDto>(HttpStatusCode.OK, producto));

            }
            catch (Exception ex)
            {
                var exceptionDetais = ex.GetBaseException().ToString();
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualiza un Producto especifico / Updates a Product.
        /// </summary>
        /// <remarks>
        /// Actualiza los datos de un Producto especifico / Updates a product.
        /// </remarks>
        /// <param name="input">Datos del Producto a Actualizar / Product information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si el Producto fue Actualizado / Returns null if the product was successfully updated -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ProductDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los Datos del Servicio a Actualizar o si el mismo no existe "+
            "/ Returns this message if the Product to update doesn't exist or some of the fields have errors", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Servicio especificado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Put")]
        [HttpPost]
        public IHttpActionResult Put(ProductDto input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _productAppService.Update(input);

                return Ok(new TicoPayResponseAPI<ProductDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Crea un Producto / Creates a Product.
        /// </summary>
        /// <remarks>
        /// Crea un Producto / Creates a Product.
        /// </remarks>
        /// <param name="input">Datos del Producto a Crear / Product information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Producto Creado / Returns the newly created Product -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ProductDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los Datos del Servicio a Crear "+
            "/ Returns this message if some information fields have errors", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Servicio especificado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Post")]
        [HttpPost]
        public IHttpActionResult Post(ProductDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                var producto = _productAppService.Create(input);

                return Ok(new TicoPayResponseAPI<ProductDto>(HttpStatusCode.OK, producto));
            }
            catch (DbUpdateConcurrencyException ex)
            {

                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Elimina un Producto especifico / Delete a Specific Product.
        /// </summary>
        /// <remarks>
        /// Elimina un Producto especifico / Delete a Specific Product.
        /// </remarks>
        /// <param name="id">Id del Producto / Product Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si el Producto fue Eliminado / Returns null if the Product was successfully Deleted -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UpdateClientInput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el producto se encuentra asociado a clientes "+
            "/ Returns this message if the Product is already used.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el product especificado "+
            "/ Returns this message if the Product doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Delete")]
        [HttpPost]
        public IHttpActionResult Delete(Guid id)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            try
            {
                var producto = _productAppService.Get(id);

                if (producto == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                //if (!_productAppService.isAllowedDelete(id)) //metodo no implementado
                //    //Verificar Condicion para elimiar producto
                //   return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El Producto se encuentra asociado"));
                _productAppService.Delete(id);

                return Ok(new TicoPayResponseAPI<ProductDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

    }
}
