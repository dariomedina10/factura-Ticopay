using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Common;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Abp.ObjectMapping;
using System.Data.Entity.Validation;
using Abp.Domain.Repositories;
using TicoPay.BranchOffices;
using TicoPay.BranchOffices.Dto;
using TicoPay.MultiTenancy;

namespace TicoPay.Web.Controllers
{
    public class BranchOfficesController : TicoPayControllerBase
    {
        private readonly IBranchOfficesAppService _branchOfficesAppService;
        private readonly ITenantAppService _tenantAppService;

        public BranchOfficesController(IBranchOfficesAppService branchOfficesAppService, ITenantAppService tenantAppService)
        {
            _branchOfficesAppService = branchOfficesAppService;
            _tenantAppService = tenantAppService;
        }

        public ActionResult Index()
        {
            SearchBranchOfficesInput model = new SearchBranchOfficesInput();
            try
            {
                    model.MaxResultCount = 5;
                    model.Entities = _branchOfficesAppService.SearchServices(model);
                    model.Control = "BranchOffices";
                    model.Action = "Search";
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar Sucursal";
            }
            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _branchOfficesAppService.GetDetail(id);
            entity.Users = _branchOfficesAppService.GetAllUser();
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateBranchOfficesInput viewModel = new CreateBranchOfficesInput();
            viewModel.isEnabled = _tenantAppService.isDrawerEnabled(AbpSession.TenantId.GetValueOrDefault(), true);
            try
            {
                if (viewModel.isEnabled)
                {
                    viewModel.ErrorCode = ErrorCodeHelper.None;
                    viewModel.ErrorDescription = "";
                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "Esta opción no se encuentra disponible para su Plan. Para acceder a ella debe actualizar su Plan a Pyme 1.";
                }
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateBranchOfficesInput viewModel)
        {
            try
            {
                viewModel.isEnabled = _tenantAppService.isDrawerEnabled(AbpSession.TenantId.GetValueOrDefault(), true);
                if (ModelState.IsValid)
                {
                    var newVm = new CreateBranchOfficesInput();
                    newVm.isEnabled = viewModel.isEnabled;
                    newVm.result = _branchOfficesAppService.Create(viewModel);

                    ModelState.Clear();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;

                    newVm.ErrorDescription = "¡Sucursal guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }

            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }

            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult CreateAjax(CreateBranchOfficesInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _branchOfficesAppService.Create(viewModel);
                    ModelState.Clear();
                    return Json(result);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
            }

            return PartialView("_createPartial", viewModel);
        }

        public ActionResult Edit(Guid id)
        {
            UpdateBranchOfficesInput viewModel = new UpdateBranchOfficesInput();
            try
            {
                viewModel = _branchOfficesAppService.GetEdit(id);

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
        public ActionResult Edit(UpdateBranchOfficesInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _branchOfficesAppService.Update(viewModel);

                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Sucursal guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.GetBaseException().Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
            }

            return PartialView("_editPartial", viewModel);
        }


        public ActionResult AjaxPage(int? page, string name, string code)
        {
            SearchBranchOfficesInput model = new SearchBranchOfficesInput();
            model.MaxResultCount = 5;
            model.Page = page;
            model.CodeFilter = code;
            model.NameFilter = name;
            try
            {
                var Entities = _branchOfficesAppService.SearchServices(model);
                model.Entities = Entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar Sucursal";
            }
            return PartialView("_listPartial", model);
        }
        [HttpPost]
        public ViewResultBase Search(SearchBranchOfficesInput model)
        {
            try
            {
                model.MaxResultCount = 5;

                var entities = _branchOfficesAppService.SearchServices(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar Sucursal";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _branchOfficesAppService.Delete(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

    }
}