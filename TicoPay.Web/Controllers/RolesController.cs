using Abp.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Authorization.Roles;
using TicoPay.Common;
using TicoPay.Roles;
using TicoPay.Roles.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class RolesController : TicoPayControllerBase
    {
        public readonly IRoleAppService _rolAppService;
        private readonly RoleManager _roleManager;

        public RolesController(IRoleAppService rolAppService, RoleManager roleManager)
        {
            _rolAppService = rolAppService;
            _roleManager = roleManager;
        }

        [HttpPost]
        public ViewResultBase Search(SearchRolesInput model)
        {
            try
            {
                var entities = _rolAppService.SearchRoles(model);
                model.Entities = entities;
                model.UserRoles = _rolAppService.GetAllListUserRoles();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Roles";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_rolesList", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string query)
        {
            SearchRolesInput model = new SearchRolesInput();
            try
            {
                model.Query = query;
                model.Entities = _rolAppService.SearchRoles(model);
                model.UserRoles = _rolAppService.GetAllListUserRoles();
                model.Control = "Roles";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Roles";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_rolesList", model);
            }
            return View("Index", model);
        }

        public ActionResult Index(int? page)
        {
            SearchRolesInput model = new SearchRolesInput();
            try
            {
                model.Query = "";
                model.Entities = _rolAppService.SearchRoles(model);
                model.UserRoles = _rolAppService.GetAllListUserRoles();
                model.Control = "Roles";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los roles";
            }
            return View(model);
        }

        public ActionResult AjaxPage(string query, int? page)
        {
            SearchRolesInput model = new SearchRolesInput();
            model.Page = page;
            if (query == null)
                query = "";

            model.Query = query;

            try
            {
                model.Entities = _rolAppService.SearchRoles(model);
                model.UserRoles = _rolAppService.GetAllListUserRoles();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los roles";
            }
            return PartialView("_rolesList", model);
        }

        public ActionResult Details(int id)
        {
            var entity = _rolAppService.GetDetail(id);
            entity.Users = _rolAppService.GetAllUser();

            return PartialView("_detailsPartial", entity);
        }

        private static List<RecursiveObject> FillRecursive(List<FlatObject> flatObjects, string parentId)
        {
            List<RecursiveObject> recursiveObjects = new List<RecursiveObject>();

            foreach (var item in flatObjects.Where(x => x.ParentId.Equals(parentId)))
            {
                recursiveObjects.Add(new RecursiveObject
                {
                    Data = item.Data,
                    Id = item.Id,
                    Children = FillRecursive(flatObjects, item.Id)
                });
            }

            return recursiveObjects;
        }

        public async Task<List<RecursiveObject>> FillRecursiveEdit(List<FlatObject> flatObjects, string parentId, int roleId)
        {
            List<RecursiveObject> recursiveObjects = new List<RecursiveObject>();
            bool selected;

            foreach (var item in flatObjects.Where(x => x.ParentId.Equals(parentId)))
            {
                var cc = await _roleManager.GetGrantedPermissionsAsync(roleId);

                var per = cc.Where(p => p.Name == item.Id).ToList();

                if (per.Count > 0)
                    selected = true;
                else
                    selected = false;

                recursiveObjects.Add(new RecursiveObject
                {
                    Data = item.Data,
                    Id = item.Id,
                    Children = await FillRecursiveEdit(flatObjects, item.Id, roleId),
                    Attr = new FlatTreeAttribute { selected = selected }
                });
            }

            return recursiveObjects;
        }

        public async Task<List<RecursiveObject>> GetPermissionsEdit(int RoleID)
        {
            var permissions = PermissionManager.GetAllPermissions(multiTenancySides: Abp.MultiTenancy.MultiTenancySides.Tenant);
            List<FlatObject> flatObject = new List<FlatObject>();
            foreach (var item in permissions)
            {
                if (item.Parent != null)
                {
                    flatObject.Add(new FlatObject(L(item.Name, new CultureInfo("ES")), item.Name, item.Parent.Name));
                }
                else
                {
                    flatObject.Add(new FlatObject(L(item.Name, new CultureInfo("ES")), item.Name, ""));
                }
            }

            return await FillRecursiveEdit(flatObject, "", RoleID);
        }

        public List<RecursiveObject> GetPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions(false);

            List<FlatObject> flatObject = new List<FlatObject>();

            foreach (var item in permissions)
            {
                if (item.Parent != null)
                {
                    flatObject.Add(new FlatObject(L(item.Name, new CultureInfo("ES")), item.Name, item.Parent.Name));
                }
                else
                {
                    flatObject.Add(new FlatObject(L(item.Name, new CultureInfo("ES")), item.Name, ""));
                }
            }

            return FillRecursive(flatObject, "");
        }

        [HttpGet]
        public ActionResult Create()
        {
            CreateRoleInput model = new CreateRoleInput();
            try
            {
                model.PermissionNames = GetPermissions();
                model.ErrorCode = ErrorCodeHelper.None;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.PermissionNames = GetPermissions();
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los roles";
            }
            return PartialView("_createPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRoleInput model)
        {
            try
            {
                if (model.permisos[0].ToString() == "")
                {
                    throw new UserFriendlyException("Debe seleccionar al menos un permiso");
                }
                else
                {
                    var role = await _roleManager.FindByNameAsync(model.Name);

                    if (role != null)
                    {
                        throw new UserFriendlyException("Existe un rol con el mismo nombre, por favor ingrese otro");
                    }
                    else
                    {
                        var result = await _rolAppService.Create(model);
                        if (result)
                        {
                            model.ErrorCode = ErrorCodeHelper.Ok;
                            model.ErrorDescription = "Rol creado exitosamente";
                            model.permisos = null;
                        }
                        else
                        {
                            model.ErrorCode = ErrorCodeHelper.Error;
                            model.ErrorDescription = "Error al crear el rol";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = e.Message;
            }
            finally
            {
                model.PermissionNames = GetPermissions();
                model.permisos = null;
            }
            return PartialView("_createPartial", model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            UpdateRoleInput viewModel = new UpdateRoleInput();

            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                viewModel.Name = role.Name;
                viewModel.Id = role.Id;
                viewModel.PermissionNames = await GetPermissionsEdit(id);// pintar todos los permisos, y chequear los que tiene asignados
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
        public async Task<ActionResult> Edit(UpdateRoleInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _rolAppService.Update(viewModel);
                    
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "¡Rol guardado exitosamente!";
                    var role = await _roleManager.FindByIdAsync(viewModel.Id);
                    viewModel.Name = role.Name;
                    viewModel.Id = role.Id;
                    viewModel.PermissionNames = await GetPermissionsEdit(viewModel.Id);
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error en los datos.";
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
            }
            viewModel.PermissionNames = await GetPermissionsEdit(viewModel.Id);
            return PartialView("_editPartial", viewModel);
        }
    }
}