using Abp.UI;
using Abp.Web.Models;
using System;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.Taxes;
using TicoPay.Taxes.Dto;
using TicoPay.Web.Infrastructure;


namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class TaxesController : TicoPayControllerBase
    {
        public readonly ITaxAppService _taxAppService;
        public TaxesController(ITaxAppService taxAppService)
        {
            _taxAppService = taxAppService;
        }

        [HttpPost]
        public ViewResultBase Search(SearchTaxesInput model)
        {
            try
            {
                var entities = _taxAppService.SearchTaxes(model);
                model.Entities = entities;
                model.Services = _taxAppService.GetAllListServices();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
                
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Impuestos";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_taxesListPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchTaxesInput model = new SearchTaxesInput();
            try
            {
                model.Query = query;
                model.Entities = _taxAppService.SearchTaxes(model);
                model.Services = _taxAppService.GetAllListServices();
                model.Control = "Taxes";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Impuestos";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_taxesListPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchTaxesInput model = new SearchTaxesInput();
            try
            {
                model.Query = "";
                model.Entities = _taxAppService.SearchTaxes(model);
                model.Services = _taxAppService.GetAllListServices();
                model.Control = "Taxes";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Impuestos";
            }
            return View(model);
        }

        
        public ActionResult AjaxPage(string query, int? page)
        {
            SearchTaxesInput model = new SearchTaxesInput();
            model.Page = page;
            if (query == null)
                query = "";

            model.Query = query;

            try
            {
                model.Entities = _taxAppService.SearchTaxes(model);
                model.Services = _taxAppService.GetAllListServices();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Impuestos";
            }
            return PartialView("_taxesListPartial", model);
        }


        public ActionResult Details(Guid id)
        {
            var entity = _taxAppService.GetDetail(id);
            entity.Users = _taxAppService.GetAllUser();

            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateTaxInput viewModel = new CreateTaxInput();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateTaxInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                   var tax= _taxAppService.Create(viewModel);
                    ModelState.Clear();
                    var newVm = new CreateTaxInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Impuesto guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                return PartialView("_createPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
                return PartialView("_createPartial", viewModel);
            }
        }

        public ActionResult Edit(Guid id)
        {
            UpdateTaxInput viewModel = new UpdateTaxInput();
            try
            {
                viewModel = _taxAppService.GetEdit(id);
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_editPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(UpdateTaxInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _taxAppService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Impuesto guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error en los datos.";
                return PartialView("_editPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
                return PartialView("_editPartial", viewModel);
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _taxAppService.Delete(id);
                return Json(new AjaxResponse { Success = true });
            }
            catch(UserFriendlyException ex)
            {
                return Json(new AjaxResponse { Success= false, Error =new ErrorInfo { Message= ex.Message, Details= ex.Details}});
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = ex.Message } });
            }
        }
    }
}