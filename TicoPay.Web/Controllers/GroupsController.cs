using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.Groups;
using TicoPay.Groups.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class GroupsController : TicoPayControllerBase
    {
        public readonly IGroupAppService _groupAppService;
        public GroupsController(IGroupAppService groupAppService)
        {
            _groupAppService = groupAppService;
        }

        [HttpPost]
        public ViewResultBase Search(SearchGroupsInput model)
        {
            try
            {
                var entities = _groupAppService.SearchGroups(model);
                model.ClientGroups = _groupAppService.GetAllClientGroups();
                model.Entities = entities;
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupos";
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
            SearchGroupsInput model = new SearchGroupsInput();
            try
            {
                model.Query = query;
                model.Entities = _groupAppService.SearchGroups(model);
                model.ClientGroups = _groupAppService.GetAllClientGroups();
                model.Control = "Groups";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupos";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchGroupsInput model = new SearchGroupsInput();
            try
            {
                model.Query = "";
                model.Entities = _groupAppService.SearchGroups(model);
                model.ClientGroups = _groupAppService.GetAllClientGroups();
                model.Control = "Groups";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupos";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchGroupsInput model = new SearchGroupsInput();
            model.Page = page;
            if (query == null)
                query = "";

            model.Query = query;
            try
            {
                model.Entities = _groupAppService.SearchGroups(model);
                model.ClientGroups = _groupAppService.GetAllClientGroups();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Grupos";
            }
            return PartialView("_listPartial", model);
        }


        public ActionResult Details(Guid id)
        {
            var entity = _groupAppService.GetDetail(id);
            entity.Users = _groupAppService.GetAllUser();
            
            return PartialView("_detailsPartial", entity);
        }

        public ActionResult Create()
        {
            CreateGroupInput viewModel = new CreateGroupInput();
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
        public ActionResult Create(CreateGroupInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _groupAppService.Create(viewModel);
                    ModelState.Clear();

                    var newVm = new CreateGroupInput();
                    newVm.ErrorCode = ErrorCodeHelper.Ok;
                    newVm.ErrorDescription = "¡Grupo guardado exitosamente!";
                    return PartialView("_createPartial", newVm);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al intentar guardar los datos.";
                return PartialView("_createPartial", viewModel);
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_createPartial", viewModel);
            }
        }

        public ActionResult Edit(Guid id)
        {
            UpdateGroupInput viewModel = new UpdateGroupInput();
            try
            {
                viewModel = _groupAppService.GetEdit(id);
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
        public ActionResult Edit(UpdateGroupInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _groupAppService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Grupo guardado exitosamente!";
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
                _groupAppService.Delete(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
    }
}