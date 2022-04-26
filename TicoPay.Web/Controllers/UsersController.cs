using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Mvc.Authorization;
using Microsoft.AspNet.Identity;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.Users;
using TicoPay.Web.Infrastructure;
using TicoPay.Users.Dto;
using TicoPay.Roles;
using System.IO;
using TicoPay.MultiTenancy;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using Abp.Configuration;
using TicoPay.Common;
using TicoPay.Drawers;
using System.Linq;
using TicoPay.Drawers.Dto;

namespace TicoPay.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Users)]
    [Authorize]
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    public class UsersController : TicoPayControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IRoleAppService _roleAppService;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITenantAppService _tenantAppService;
        private readonly ISettingManager _settingManager;
        private readonly IDrawersAppService _drawersAppService;

        public UsersController(IUserAppService userAppService, UserManager userManager, RoleManager roleManager,
            IUnitOfWorkManager unitOfWorkManager, IRoleAppService roleAppService, ITenantAppService tenantAppService,ISettingManager settingManager, IDrawersAppService drawersAppService)

        {
            _userAppService = userAppService;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWorkManager = unitOfWorkManager;
            _roleAppService = roleAppService;
            _tenantAppService = tenantAppService;
            _settingManager = settingManager;
            _drawersAppService = drawersAppService;
        }

        public ActionResult Index(int? page)
        {
            SearchUsersInput model = new SearchUsersInput();
            try
            {
                model.Query = "";
                model.Entities = _userAppService.SearchUsers(model);
                model.Roles = _roleManager.Get();
                model.Control = "Users";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Usuarios";
            }
            return View(model);
        }

        [HttpPost]
        public ViewResultBase Search(SearchUsersInput model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entities = _userAppService.SearchUsers(model);
                    model.Roles = _roleManager.Get();
                    model.Entities = entities;
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                    model.Control = "Users";
                    model.Action = "Search";
                }
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Clientes";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public ViewResultBase SearchDebounce(string name, string surName, string emailAddress, int? rolId)
        {
            SearchUsersInput model = new SearchUsersInput();
            try
            {
                if (name.Length <= 50 && surName.Length <= 50 && emailAddress.Length <= 100)
                {
                    model.NameFilter = (name.Length > 50) ? null : name; ;
                    model.SurnameFilter = (surName.Length > 50) ? null : surName; ;
                    model.EmailAddressFilter = (emailAddress.Length > 100) ? null : emailAddress; ;
                    model.IdRolFilter = rolId;
                    model.Entities = _userAppService.SearchUsers(model);
                    model.Roles = _roleManager.Get();
                    model.Control = "Users";
                    model.Action = "Search";
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                }
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Usuarios";
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View("Index", model);
        }

        public ActionResult AjaxPage(string name, string surName, string emailAddress, int? rolId, int? page)
        {
            SearchUsersInput model = new SearchUsersInput();
            model.Page = page;
            model.NameFilter = name;
            model.SurnameFilter = surName;
            model.EmailAddressFilter = emailAddress;
            model.IdRolFilter = rolId;
            model.Roles = _roleManager.Get();

            try
            {
                if (ModelState.IsValid)
                {
                    model.Entities = _userAppService.SearchUsers(model);
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                }
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Usuarios";
            }
            return PartialView("_listPartial", model);
        }


        public ActionResult Create()
        {
            CreateUserInput viewModel = new CreateUserInput();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
                viewModel.Roles = _roleManager.Get();
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.Roles = _roleManager.Get();
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_createPartial", viewModel);
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> Create(CreateUserInput model)
        {
            try
            {
                CheckModelState();

                if (_userAppService.UserNameExist(model.UserName))
                    throw new UserFriendlyException("Existe un Usuario con el mismo Nombre de Usuario.");

                if (_userAppService.EmailExistCreate(model.EmailAddress))
                    throw new UserFriendlyException("Existe un Usuario con el mismo Correo.");

                var user = new User
                {
                    TenantId = AbpSession.GetTenantId(),
                    Name = model.Name,
                    Surname = model.Surname,
                    EmailAddress = model.EmailAddress,
                    IsEmailConfirmed = true,
                    IsActive = true
                };

                user.UserName = model.UserName;
                user.Password = new PasswordHasher().HashPassword(model.Password);
               
                //Switch to the tenant
                _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant); 
                _unitOfWorkManager.Current.SetTenantId(AbpSession.GetTenantId());

                //Add default roles
                user.Roles = new List<UserRole>();
                user.Roles.Add(new UserRole { RoleId = model.IdRol, TenantId = user.TenantId });

                //Save user
                CheckErrors(await _userManager.CreateAsync(user));
                await _unitOfWorkManager.Current.SaveChangesAsync();

                _settingManager.ChangeSettingForUser(user.ToUserIdentifier(), Abp.Localization.LocalizationSettingNames.DefaultLanguage, "en");

                var newVm = new CreateUserInput();
                newVm.ErrorCode = ErrorCodeHelper.Ok;
                newVm.ErrorDescription = "¡Usuario guardado exitosamente!";

                newVm.Roles = _roleManager.Get();

                return PartialView("_createPartial", newVm);
            }
            catch (Exception ex)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = ex.Message;
                model.Roles = _roleManager.Get();
                return PartialView("_createPartial", model);
            }
        }

        public async Task<ActionResult> Edit(long id)
        {
            UpdateUserInput viewModel = new UpdateUserInput();
            UserRole userRole = new UserRole();
            try
            {
                var user = await _userManager.GetUserByIdAsync(id);
                viewModel.Roles = _roleManager.Get();
                viewModel.Name = user.Name;
                viewModel.Surname = user.Surname;
                viewModel.UserName = user.UserName;
                viewModel.Id = user.Id;
                viewModel.IdRol = _roleAppService.GetRoleByUser(id); ;
                viewModel.Password = "";
                viewModel.EmailAddress = user.EmailAddress;
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.Roles = _roleManager.Get();
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_editPartial", viewModel);
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> Edit(UpdateUserInput viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userAppService.Update(viewModel);
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.Roles = _roleManager.Get();
                    viewModel.ErrorDescription = "¡Usuario guardado exitosamente!";
                    return PartialView("_editPartial", viewModel);
                }
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error en los datos.";
                viewModel.Roles = _roleManager.Get();
                return PartialView("_editPartial", viewModel);
            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.Roles = _roleManager.Get();
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
                return PartialView("_editPartial", viewModel);
            }
        }

        [HttpPost]
        public ActionResult ChangeStatus(long id)
        {
            try
            {
                _userAppService.ChangeStatus(id);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResendConfirmation(long id)
        {
            try
            {
                var provider = new DpapiDataProtectionProvider("TicoPay.Web");
                _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ConfirmEmail")) { TokenLifespan = TimeSpan.FromHours(24) } as IUserTokenProvider<User, long>;
                _userManager.EmailService = new IdentityMessageService();

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.Current.SetTenantId(AbpSession.GetTenantId());
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(id);
                    var user = _userManager.FindById(id);
                    var tenant = _tenantAppService.GetEdit(AbpSession.GetTenantId());
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { tenantId = AbpSession.GetTenantId(), userId = id, code = code }, protocol: Request.Url.Scheme);
                    await _userManager.SendEmailAsync(id, "Confirmar Cuenta", GetAccountConfirmationEmailBody(user.Name, tenant.TenancyName, user.UserName, callbackUrl));
                    await unitOfWork.CompleteAsync();
                }
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetAccountConfirmationEmailBody(string nameAndSurname, string tenantName, string userName, string activationLink)
        {
            string body = string.Empty;
            string templatePath = HttpContext.Server.MapPath("~/Content/_AccountConfirmationTemplate.html");
            if (System.IO.File.Exists(templatePath))
            {
                using (StreamReader vReader = new StreamReader(templatePath))
                {
                    body = vReader.ReadToEnd();
                }
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var tenantUrl = string.Format("{0}://{1}{2}{3}",
                        Request.Url.Scheme,
                        Request.Url.Host,
                        Request.Url.Port == 80 ? string.Empty : ":" + Request.Url.Port,
                        Request.ApplicationPath);

                    body = body.Replace("{PART_NameAndSurname}", nameAndSurname);
                    body = body.Replace("{PART_ActivationLink}", activationLink);
                    body = body.Replace("{PART_TenantUrl}", tenantUrl);
                    body = body.Replace("{PART_UserName}", userName);
                }
            }
            return body;
        }

        public async Task<ActionResult> UserDrawers(long id) {

            UpdateUserDrawers viewModel = new UpdateUserDrawers();
          
            try
            {
                var user = await _userManager.GetUserByIdAsync(id);
                viewModel.BranchOffice = _drawersAppService.GetBranchOffices().ToList();               
                viewModel.Name = user.Name +" "+ user.Surname;            
                viewModel.Id = user.Id;               
                viewModel.ListDrawers = (from c in  user.DrawerUsers where c.Drawer!=null select new listDrawer { Id= c.DrawerId.Value, IdDrawerUser=c.Id, Code=c.Drawer.Code, Description=c.Drawer.Description, Name=c.Drawer.BranchOffice.Name }).ToList();
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.BranchOffice = _drawersAppService.GetBranchOffices().ToList();
                viewModel.ErrorDescription = "Error al obtener datos.";
            }
            return PartialView("_drawersPartial", viewModel);
        }

        public async Task<ActionResult> updateDrawers (UpdateUserDrawers viewModel)
        {
            User user = null;
            try
            {
                await _userAppService.UpdateDrawesUser(viewModel);
                viewModel.ErrorCode = ErrorCodeHelper.Ok;
                viewModel.ErrorDescription = "¡Usuario guardado exitosamente!";
            }
            catch (Exception ex)
            {

                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = ex.Message;
            }
            try
            {             
                user = await _userManager.GetUserByIdAsync(viewModel.Id);
                viewModel.BranchOffice = _drawersAppService.GetBranchOffices().ToList();
                viewModel.Name = user.Name + " " + user.Surname;
                viewModel.Id = user.Id;
                viewModel.DrawerUsers = user.DrawerUsers;

            }
            catch (Exception e)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;                
                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    viewModel.ErrorDescription = "¡Por favor verifique los datos!";
                else
                    viewModel.ErrorDescription = e.Message;
                
            }           
            return PartialView("_drawersPartial", viewModel);
        }

        [ActionName("GetDrawer")]
        public ActionResult GetDrawer(Guid? idBranch)
        {
            IList<SelectListItem> resp= new List<SelectListItem>();
            if (idBranch != null)
            {
                var drawer = _drawersAppService.SearchDrawersbyBranch((Guid)idBranch);

                 resp = drawer.Select(x => new SelectListItem()
                {
                    Value = x.Id.ToString()+"_"+x.BranchOffice.Name+"_"+x.Code + "_"+x.Description,
                    Text = x.Code
                }).ToList();
               
                
            }
            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Caja" });
            

            return Json(resp, JsonRequestBehavior.AllowGet);

        }
    }
}