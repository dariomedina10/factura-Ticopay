using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.Editions;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using TicoPay.Users.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Authorize]
    public class AboutController : TicoPayControllerBase
    {
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IUserAppService _userAppService;
        private readonly IInvoiceAppService _invoiceAppService;

        public AboutController(UserManager userManager, TenantManager tenantManager, IUserAppService userAppService, IInvoiceAppService invoiceAppService)
        {
            _userManager = userManager;
            _tenantManager = tenantManager;
            _userAppService = userAppService;
            _invoiceAppService = invoiceAppService;
        }

        public virtual async Task<ActionResult> Index(string id)
        {
            long IdUser = long.Parse(id);

            UpdateProfileInput viewModel = new UpdateProfileInput();

            try
            {
                var user = await _userManager.GetUserByIdAsync(IdUser);
                viewModel.FullName = user.FullName;
                viewModel.UserName = user.UserName;
                viewModel.EmailAddress = user.EmailAddress;
                viewModel.Id = user.Id;
                viewModel.LastLogin = user.LastLoginTime;
                viewModel.TenantID = user.TenantId;
                viewModel.Password = "";
                viewModel.ConfirmedPassword = "";

                var tenant = await _tenantManager.GetByIdAsync(viewModel.TenantID.GetValueOrDefault());
                viewModel.TenancyName = tenant.TenancyName;
                viewModel.EditionName = tenant.Edition.Name;
                viewModel.EditionUsersLimit = await _tenantManager.EditionManager.GetFeatureValueOrNullAsync(tenant.EditionId.Value, "UsersLimit");
                viewModel.EditionInvoicesMonthlyLimit = await _tenantManager.EditionManager.GetFeatureValueOrNullAsync(tenant.EditionId.Value, "InvoicesMonthlyLimit");

                var users = await _userAppService.GetUsers();
                viewModel.UsersCount = users.Items.Count;
                viewModel.InvoicesInMonth = await _invoiceAppService.GetTotalInvoicesInMonthAsync(tenant.Id);

                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Index(UpdateProfileInput viewModel)
        {
            IdentityResult result;

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(viewModel.Id);

                    result = await _userManager.ChangePasswordAsync(user, viewModel.Password);

                    if (result.Succeeded)
                    {
                        result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            viewModel.ErrorCode = ErrorCodeHelper.Ok;
                            viewModel.ErrorDescription = "La contraseña ha sido restablecida exitosamente!";
                            return View(viewModel);
                        }
                    }

                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "Error al cambiar la contraseña!";
                }
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener los datos!";
            }

            return View(viewModel);
        }
    }
}