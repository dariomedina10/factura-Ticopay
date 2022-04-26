using System;
using System.Web.Mvc;
using System.Web.WebPages;
using Abp.UI;
using TicoPay.Services;
using TicoPay.Services.Dto;
using TicoPay.Web.Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Entity.Validation;
using Abp.Runtime.Validation;
using TicoPay.Common;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class ServicesController : TicoPayControllerBase
    {
        public readonly IServiceAppService _serviceAppService;
        public const int unitmeasurID = 1;
        public ServicesController(IServiceAppService serviceAppService)
        {
            _serviceAppService = serviceAppService;
        }

        [HttpPost]
        public ViewResultBase Search(SearchServicesInput model)
        {
            try
            {
                var entities = _serviceAppService.SearchServices(model);
                model.Taxes = _serviceAppService.GetAllTaxes();
                model.Recurrents = SelectRecurrents();
                model.ClientServices = _serviceAppService.GetAllClientServices();
                model.InvoiceLine = _serviceAppService.invoiceLineRepository();
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Servicios";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string name, decimal? priceSince, decimal? priceUntil, Guid? taxId, int? recurrentId)
        {
            SearchServicesInput model = new SearchServicesInput();
            try
            {
                model.NameFilter = name;
                model.PriceSinceFilter = priceSince;
                model.PriceUntilFilter = priceUntil;
                model.TaxId = taxId;
                model.RecurrentId = recurrentId;
                model.Taxes = _serviceAppService.GetAllTaxes();
                model.Recurrents = SelectRecurrents();
                model.Entities = _serviceAppService.SearchServices(model);
                model.ClientServices = _serviceAppService.GetAllClientServices();
                model.Control = "Services";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Servicios";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchServicesInput model = new SearchServicesInput();
            try
            {
                var Entities = _serviceAppService.SearchServices(model);
                model.Taxes = _serviceAppService.GetAllTaxes();
                model.Recurrents = SelectRecurrents();
                model.ClientServices = _serviceAppService.GetAllClientServices();
                model.InvoiceLine = _serviceAppService.invoiceLineRepository();
                model.Entities = Entities;
                model.Control = "Services";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Servicios";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string name, decimal? priceSince, decimal? priceUntil, Guid? taxId, int? recurrentId, int? page)
        {
            SearchServicesInput model = new SearchServicesInput();
            model.Page = page;
            if (name == null)
                name = "";

            model.NameFilter = name;
            model.PriceSinceFilter = priceSince;
            model.PriceUntilFilter = priceUntil;
            model.TaxId = taxId;
            model.RecurrentId = recurrentId;

            try
            {
                var Entities = _serviceAppService.SearchServices(model);
                model.Taxes = _serviceAppService.GetAllTaxes();
                model.Recurrents = SelectRecurrents();
                model.Entities = Entities;
                model.ClientServices = _serviceAppService.GetAllClientServices();
                model.InvoiceLine = _serviceAppService.invoiceLineRepository();
                model.InvoiceLine = _serviceAppService.invoiceLineRepository();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Servicios";
            }
            return PartialView("_listPartial", model);
        }


        public ActionResult Details(Guid id)
        {
            var entity = _serviceAppService.GetDetail(id);
            entity.Users = _serviceAppService.GetAllUser();
            
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateServiceInput viewModel = new CreateServiceInput();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.Taxes = _serviceAppService.GetAllTaxes();
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
        public ActionResult Create(CreateServiceInput viewModel)
        {
            try
            {
                ModelState.Clear();
                
                if (ModelState.IsValid)
                {
                    if (!viewModel.IsRecurrent)
                        viewModel.CronExpression = null;

                    var newVm = new CreateServiceInput();

                    newVm.result= _serviceAppService.Create(viewModel);

                    ModelState.Clear();

                   
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.Taxes = _serviceAppService.GetAllTaxes();
                    newVm.ErrorDescription = "¡Servicio guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrors().ToErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrors().ToErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            viewModel.Taxes = _serviceAppService.GetAllTaxes();
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult CreateAjax(CreateServiceInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!viewModel.IsRecurrent)
                        viewModel.CronExpression = null;

                  
                    var result = _serviceAppService.Create(viewModel);

                    ModelState.Clear();               
                    return Json(result);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos: " + ModelState.ToErrorMessage();
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
            }
            viewModel.Taxes = _serviceAppService.GetAllTaxes();
            return PartialView("_createPartial", viewModel);
        }

        public ActionResult Edit(Guid id)
        {
            UpdateServiceInput viewModel = new UpdateServiceInput();
            try
            {
                viewModel = _serviceAppService.GetEdit(id);

                if (viewModel.CronExpression.IsEmpty())
                    viewModel.CronExpression = "0 0 0 1 * ?";

                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            viewModel.Taxes = _serviceAppService.GetAllTaxes();
            return PartialView("_editPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(UpdateServiceInput viewModel)
        {
            try
            {
                ModelState.Clear();
               
                if (ModelState.IsValid)
                {
                    _serviceAppService.Update(viewModel);
                    viewModel.Taxes = _serviceAppService.GetAllTaxes();

                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Servicio guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }                
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrors().ToErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrors().ToErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            viewModel.Taxes = _serviceAppService.GetAllTaxes();
            return PartialView("_editPartial", viewModel);
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _serviceAppService.Delete(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        public IList<SelectListItem> SelectRecurrents()
        {
            IList<SelectListItem> listSelectRecurrents = new List<SelectListItem>();

            listSelectRecurrents.Add(new SelectListItem { Value = ((int)SelectRecurrent.Recurrent).ToString(CultureInfo.InvariantCulture), Text = "Recurrente" });
            listSelectRecurrents.Add(new SelectListItem { Value = ((int)SelectRecurrent.NonRecurring).ToString(CultureInfo.InvariantCulture), Text = "No Recurrente" });

            return listSelectRecurrents;
        }

    }
}