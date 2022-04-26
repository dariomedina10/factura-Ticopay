using Abp.Web.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Authorization;
using TicoPay.Common;
using TicoPay.GroupConcept;
using TicoPay.GroupConcept.Dto;
using TicoPay.Services;
using TicoPay.Web.Infrastructure;


namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [AbpMvcAuthorize(PermissionNames.Pages_GroupConcepts)]
    [Authorize]
    public class GroupConceptsController : TicoPayControllerBase
    {
        public readonly IGroupConceptsAppService _groupConceptsAppService;
        public readonly IServiceAppService _serviceAppService;

        public GroupConceptsController(IGroupConceptsAppService groupConceptsAppService, IServiceAppService serviceAppService)
        {
            _groupConceptsAppService = groupConceptsAppService;
            _serviceAppService = serviceAppService;
        }

        [HttpPost]
        public ViewResultBase Search(SearchGroupConceptsInput model)
        {
            try
            {
                var entities = _groupConceptsAppService.SearchGroupConcepts(model);
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupo de Servicios.";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchGroupConceptsInput model = new SearchGroupConceptsInput();
            try
            {
                model.Query = query;
                model.Entities = _groupConceptsAppService.SearchGroupConcepts(model);
                model.Control = "GroupConcepts";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupo de Servicios.";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchGroupConceptsInput model = new SearchGroupConceptsInput();
            try
            {
                model.Query = "";
                model.Entities = _groupConceptsAppService.SearchGroupConcepts(model);
                model.Control = "GroupConcepts";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupo de Servicios.";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchGroupConceptsInput model = new SearchGroupConceptsInput();
            model.Page = page;
            if (query == null)
                query = "";

            model.Query = query;
            try
            {
                model.Entities = _groupConceptsAppService.SearchGroupConcepts(model);
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupo de Servicios.";
            }
            return PartialView("_listPartial", model);
        }

        public ActionResult Details(Guid id)
        {
            var entity = _groupConceptsAppService.GetDetail(id);
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateGroupConceptsInput viewModel = new CreateGroupConceptsInput();
            try
            {
                viewModel.AvailableServicios = _serviceAppService.GetServices().Items.Select(i => i).Where(i => i.IsRecurrent).ToList();
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al crear el Grupo de Servicios.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateGroupConceptsInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _groupConceptsAppService.Create(viewModel);
                    ModelState.Clear();

                    var newVm = new CreateGroupConceptsInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Grupo de conseptos guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.AvailableServicios = _serviceAppService.GetServices().Items.Select(i => i).Where(i => i.IsRecurrent).ToList();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
                return PartialView("_createPartial", viewModel);
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_createPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_createPartial", viewModel);
            }
        }

        public ActionResult Edit(Guid id)
        {
            UpdateGroupConceptsInput viewModel = new UpdateGroupConceptsInput();
            try
            {
                viewModel = _groupConceptsAppService.GetEdit(id);
                viewModel.AvailableServicios = _serviceAppService.GetServices().Items.Select(i => i).Where(i => i.IsRecurrent).ToList();
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
        public ActionResult Edit(UpdateGroupConceptsInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _groupConceptsAppService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Grupo guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.AvailableServicios = _serviceAppService.GetServices().Items.Select(i => i).Where(i => i.IsRecurrent).ToList();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ModelState.ToErrorMessage();
                return PartialView("_editPartial", viewModel);
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                viewModel.ErrorDescription = ex.GetModelErrorMessage();
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_createPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorDescription = e.Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_editPartial", viewModel);
            }
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _groupConceptsAppService.Delete(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}