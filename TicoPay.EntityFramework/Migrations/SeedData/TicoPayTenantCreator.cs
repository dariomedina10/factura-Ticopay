using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.BranchOffices;
using TicoPay.Drawers;
using TicoPay.Editions;
using TicoPay.EntityFramework;
using TicoPay.General;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.Taxes;
using TicoPay.Users;

namespace TicoPay.Migrations.SeedData
{
    public class TicoPayTenantCreator
    {
        public const string TicoPayTenantName = "ticopay";
        public const string TicoPayTenantComercialName = "HNG CARMENTA GLOBALGROUP SOCIEDAD ANONIMA";
        public const string TicoPayTenantEmail = "info@ticopays.com";
        public const string TicoPayAdminEmail = "info@ticopays.com";
        public const string TicoPayAdminPassword = "P@ssw0rd";
        public const string TaxAdministrationUserName = "mhcostarica";
        public const string TaxAdministrationUserPassword = "gjC8m6EV";

        private readonly TicoPayDbContext _context;

        public TicoPayTenantCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateTenant();
        }

        private void CreateTenant()
        {
            var ticoPayTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == TicoPayTenantName);
            if (ticoPayTenant == null)
            {
                Country countries = new Country();
                countries.CountryCode = "506";
                countries.CountryName = "Costa Rica";
                countries.ResolutionDate = Convert.ToDateTime("20-02-2017");
                countries.ResolutionNumber = "DGT-R-13-2017";

                _context.Countries.Add(countries);
                _context.SaveChanges();

                var pais = _context.Countries.FirstOrDefault(p => p.CountryCode == "506");
                var barrio = _context.Barrios.FirstOrDefault(b => b.NombreBarrio == "Pocosol");
                var edicionEmpresarial = _context.Editions.FirstOrDefault(b => b.Name == EditionManager.BusinessEditionName);


                Tenant ticoPay = new Tenant();
                ticoPay.BussinesName = TicoPayTenantComercialName;
                ticoPay.ComercialName = TicoPayTenantComercialName;
                ticoPay.Name = TicoPayTenantComercialName;
                ticoPay.TenancyName = TicoPayTenantName.ToLower();
                ticoPay.local = "001";
                ticoPay.IdentificationNumber = "3101741788";
                ticoPay.PhoneNumber = "50687046985";
                ticoPay.Fax = "";
                ticoPay.Email = TicoPayTenantEmail;
                ticoPay.CodigoMoneda = Invoices.XSD.FacturaElectronicaResumenFacturaCodigoMoneda.CRC;
                ticoPay.ConditionSaleType = Invoices.XSD.FacturaElectronicaCondicionVenta.Contado;
                ticoPay.IdentificationType = Invoices.XSD.IdentificacionTypeTipo.Cedula_Juridica;
                ticoPay.GeneratesAutomaticClientCodeSetting = true;
                ticoPay.IsActive = true;
                ticoPay.ConnectionString = null;
                if (pais != null)
                {
                    ticoPay.CountryID = pais.Id;
                }
                if (barrio != null)
                {
                    ticoPay.BarrioId = barrio.Id;
                }
                ticoPay.EditionId = edicionEmpresarial.Id;
                ticoPay.Address = "125 METROS OESTE DE LA IGLESIA CATOLICA DE SANTA ROSA POCOSOL";

                _context.Tenants.Add(ticoPay);
                _context.SaveChanges();
            }

            ticoPayTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == TicoPayTenantName);
            if (ticoPayTenant != null)
            {
                AgreementConectivity agreement = new AgreementConectivity();
                agreement.AgreementNumbers = 555;
                agreement.KeyType = TipoLLaveAcceso.Codigo_Cliente;
                agreement.Port = 5103;
                agreement.TenantID = ticoPayTenant.Id;


                _context.AgreementsConectivities.Add(agreement);
                _context.SaveChanges();

                CreateRolesAndUsers(ticoPayTenant.Id);
                CreateRoleAndUserForTaxAdministration(ticoPayTenant.Id);
                CreateTaxes(ticoPayTenant.Id);
                CreateServices(ticoPayTenant.Id);
                CreateDefaultRegister(ticoPayTenant.Id);
            }
        }

        private void CreateRolesAndUsers(int tenantId)
        {
            var adminRole = _context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true });
                _context.SaveChanges();

                var permissions = PermissionFinder
                    .GetAllPermissions(new TicoPayAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant))
                    .ToList();

                foreach (var permission in permissions)
                {
                    _context.Permissions.Add(new RolePermissionSetting { TenantId = tenantId, Name = permission.Name, IsGranted = true, RoleId = adminRole.Id });
                }
                _context.SaveChanges();
            }


            var adminUser = _context.Users.FirstOrDefault(u => u.TenantId == tenantId && u.UserName == User.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(tenantId, TicoPayAdminEmail, TicoPayAdminPassword);
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                _context.UserRoles.Add(new UserRole(tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }
        }

        private void CreateTaxes(int tenantId)
        {
            Tax tex = _context.Taxes.FirstOrDefault(r => r.TenantId == tenantId);
            if (tex == null)
            {
                _context.Taxes.Add(Tax.Create(tenantId, "IVA", 15, Invoices.XSD.ImpuestoTypeCodigo.Impuesto_General_Sobre_Ventas));
                _context.SaveChanges();
            }
        }

        private void CreateServices(int tenantId)
        {
            string monthly = "0 0 0 1 * ?";

            var tax = _context.Taxes.FirstOrDefault(t => t.TenantId == tenantId && t.Name == "IVA");

            var free = _context.Services.FirstOrDefault(s => s.TenantId == tenantId && s.Name == EditionManager.FreeEditionName);
            if (free == null)
            {
                var service = Service.Create(tenantId, EditionManager.FreeEditionName, 0, monthly, Invoices.XSD.UnidadMedidaType.Uno, "", false,1,0);
                service.ChangeTax(tax);
                _context.Services.Add(service);
            }
            var professional = _context.Services.FirstOrDefault(s => s.TenantId == tenantId && s.Name == EditionManager.ProfesionalEditionName);
            if (professional == null)
            {
                var service = Service.Create(tenantId, EditionManager.ProfesionalEditionName, 7, monthly, Invoices.XSD.UnidadMedidaType.Uno, "", false, 1, 0);
                service.ChangeTax(tax);
                _context.Services.Add(service);
            }
            var pyme1 = _context.Services.FirstOrDefault(s => s.TenantId == tenantId && s.Name == EditionManager.Pyme1EditionName);
            if (pyme1 == null)
            {
                var service = Service.Create(tenantId, EditionManager.Pyme1EditionName, 25, monthly, Invoices.XSD.UnidadMedidaType.Uno, "", false, 1, 0);
                service.ChangeTax(tax);
                _context.Services.Add(service);
            }
            var pyme2 = _context.Services.FirstOrDefault(s => s.TenantId == tenantId && s.Name == EditionManager.Pyme2EditionName);
            if (pyme2 == null)
            {
                var service = Service.Create(tenantId, EditionManager.Pyme2EditionName, 15, monthly, Invoices.XSD.UnidadMedidaType.Uno, "", false, 1, 0);
                service.ChangeTax(tax);
                _context.Services.Add(service);
            }
            var business = _context.Services.FirstOrDefault(s => s.TenantId == tenantId && s.Name == EditionManager.BusinessEditionName);
            if (business == null)
            {
                var service = Service.Create(tenantId, EditionManager.BusinessEditionName, 40, monthly, Invoices.XSD.UnidadMedidaType.Uno, "", false, 1, 0);
                service.ChangeTax(tax);
                _context.Services.Add(service);
            }
        }

        private void CreateDefaultRegister(int tenantId)
        {
            Register register = _context.Registers.FirstOrDefault(r => r.TenantId == tenantId);
            if (register == null)
            {
                BranchOffice branchOffice = BranchOffice.Create(tenantId, "Principal", "001", string.Empty);
                _context.BranchOffices.Add(branchOffice);

                var drawer = Drawer.Create(tenantId, "00001", "Caja 001", branchOffice.Id);
                _context.Drawers.Add(drawer);              

                _context.Registers.Add(Register.Create(tenantId, "Caja 001", "00001", 1, 0, 1, 0, 1, 0, 1, 0, "", "", true, false, false, true, true, false, drawer.Id));
                _context.SaveChanges();
            }
        }
        
        private void CreateRoleAndUserForTaxAdministration(int tenantId)
        {
            var haciendaRole = _context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == StaticRoleNames.Tenants.TaxAdministration);
            if (haciendaRole == null)
            {
                haciendaRole = _context.Roles.Add(new Role(tenantId, StaticRoleNames.Tenants.TaxAdministration, StaticRoleNames.Tenants.TaxAdministration) { IsStatic = true });
                _context.SaveChanges();

                _context.Permissions.Add(new RolePermissionSetting { TenantId = tenantId, Name = PermissionNames.TaxAdministration, IsGranted = true, RoleId = haciendaRole.Id });
                _context.Permissions.Add(new RolePermissionSetting { TenantId = tenantId, Name = PermissionNames.TaxAdministration_ViewInvoices, IsGranted = true, RoleId = haciendaRole.Id });
                _context.SaveChanges();
            }

            var haciendaUser = _context.Users.FirstOrDefault(u => u.TenantId == tenantId && u.UserName == TaxAdministrationUserName);
            if (haciendaUser == null)
            {
                haciendaUser = User.CreateTenantUser(tenantId, TaxAdministrationUserName, StaticRoleNames.Tenants.TaxAdministration, StaticRoleNames.Tenants.TaxAdministration, TicoPayTenantEmail, TaxAdministrationUserPassword);
                haciendaUser.IsEmailConfirmed = true;
                haciendaUser.IsActive = true;

                _context.Users.Add(haciendaUser);
                _context.SaveChanges();

                _context.UserRoles.Add(new UserRole(tenantId, haciendaUser.Id, haciendaRole.Id));
                _context.SaveChanges();
            }
        }
    }
}
