using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using TicoPay.Authorization.Roles;
using TicoPay.Editions;
using TicoPay.MultiTenancy.Dto;
using TicoPay.Users;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Application.Editions;
using Abp.Domain.Repositories;
using Abp.Dependency;
using System;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Services;
using TicoPay.Address;
using TicoPay.General;
using TicoPay.Invoices;
using Abp.Notifications;
using Abp;
using AutoMapper;
using SendMail;
using System.Text;
using System.IO;
using TicoPay.Common;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TicoPay.Services.Dto;
using TicoPay.ReportsSettings;
using TicoPay.Authorization;
using Quartz;
using Microsoft.AspNet.Identity;
using Abp.Application.Features;
using TicoPay.Common;
using TicoPay.Invoices.Dto;
using IdentityModel.Client;
using TicoPay.Drawers;
using TicoPay.BranchOffices;
using LinqKit;
using Abp.Application.Features;
using static TicoPay.MultiTenancy.Tenant;
namespace TicoPay.MultiTenancy
{
    //[AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantAppService : TicoPayAppServiceBase, ITenantAppService, ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly RoleManager _roleManager;
        private readonly EditionManager _editionManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IUserAppService _userAppService;
        private readonly UserManager _userManager;
        private readonly IClientAppService _clientAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly IAddressService _addressService;
        private readonly IRepository<Register, Guid> _registerRepository;
        private readonly IRepository<Certificate, int> _certificateRepository;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IClientManager _clientManager;
        private readonly IReportSettingsAppService _reportSettingsAppService;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        private readonly IRepository<BranchOffice, Guid> _branchOfficesRepository;
        private readonly IRepository<EditionFeatureSetting, long> _editionFeatureRepository;

        // definicion de constantes para envio de correo
        public const string emailCuentaTicopay = @"<br/><p><strong>Estimado cliente se le recuerda que dispone de tres (03) días para pagar esta factura, de lo contrario su cuenta será desactivada hasta recibir el pago correspondiente.</strong></p>
                                                    <p>Efectuar pagos en Colones (&#8353;) o en Dólares ($),según tipo de cambio de venta del día del Banco Nacional de Costa Rica.</p>
                                                    
                                                    <p><ins><strong>CANALES DEL BANCO NACIONAL:</strong></ins></p>
                                                    <p>Para su comodidad y seguridad puede Pagar sus facturas a través de todos los canales del Banco Nacional: BN Internet Banking, BN Servicios, BN Corresponsales, PAR o Cajas.</p>
	                                                <p>Desde la opción <strong>“Pagos”</strong> selecciona la opción <strong>“Facturación”</strong>, luego <strong>“Ticopay”</strong>. Para consultar sus facturas pendientes digite el <strong>Código del Cliente</strong>, que aparece en su Factura Electrónica, en el espacio “Valor a consultar”.</p>
                                                    
                                                    <p><ins><strong>DEPÓSITOS O TRANSFERENCIAS</ins></strong>
                                                    <p><strong>RAZON SOCIAL : </strong> HNG CARMENTA GLOBAL GROUP SOCIEDAD ANONIMA</p>
	                                                <p><strong>Cédula Jurídica : </strong> 3-101-741788</p>
	                                                
	                                                <table>
                                                        <tr>
                                                            <td style='width:350px'><strong>Cuenta en Colones (₡) – Banco Nacional</strong></td>
		                                                </tr>
                                                        <tr>
			                                                <td style='width:350px'>Cuenta Banco Nacional</td>
                                                            <td style='width:250px'><strong>200-01-111-015692-4</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Nacional</td>
                                                            <td style='width:250px'><strong>15111120010156920</strong></td>
		                                                </tr>	
                                                        <tr>
                                                            <td style='width:350px'><strong>Cuenta en Colones (₡) – Banco Costa Rica</strong></td>
		                                                </tr>
                                                        <tr>
			                                                <td style='width:350px'>Cuenta Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>001-0463071-8</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>15201001046307189</strong></td>
		                                                </tr>	
	                                                </table><br/>
	                                                
	                                                <table>
                                                        <tr>
                                                            <td style='width:350px'><strong>Cuenta en Dólares ($) – Banco Nacional</strong></td>
		                                                </tr>
                                                        <tr>
			                                                <td style='width:350px'>Cuenta Banco Nacional</td>
                                                            <td style='width:250px'><strong>200-02-111-007852-0</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Nacional</td>
                                                            <td style='width:250px'><strong>15111120020078523</strong></td>
		                                                </tr>
		                                                <tr>
                                                            <td style='width:350px'><strong>Cuenta en Dólares ($) – Banco Costa Rica</strong></td>
		                                                </tr>
                                                        <tr>
			                                                <td style='width:350px'>Cuenta Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>001-0463074-2</strong></td>
		                                                </tr>
		                                                <tr>
			                                                <td style='width:350px'>Cuenta Cliente Banco Costa Rica (Cuenta Corriente)</td>
                                                            <td style='width:250px'><strong>15201001046307427</strong></td>
		                                                </tr>
	                                                </table><br/>";

        public TenantAppService(
            TenantManager tenantManager,
            RoleManager roleManager,
            EditionManager editionManager,
            IAbpZeroDbMigrator abpZeroDbMigrator, IUnitOfWorkManager unitOfWorkManager, IRepository<Tenant, int> tenantRepository,
            IUserAppService userAppService, UserManager userManager, IRepository<Certificate, int> certificateRepository,
            IClientAppService clientAppService, IServiceAppService serviceAppService,
            IAddressService addressService, IRepository<Register, Guid> registerRepository, IRepository<BranchOffice, Guid> branchOfficesRepository, IRepository<Drawer, Guid> drawerRepository,
        INotificationSubscriptionManager notificationSubscriptionManager,
            IClientManager clientManager, IReportSettingsAppService reportSettingsAppService, IRepository<Invoice, Guid> invoiceRepository, IRepository<Client, Guid> clientRepository, IRepository<EditionFeatureSetting, long> editionFeatureRepository)
        {
            _tenantManager = tenantManager;
            _roleManager = roleManager;
            _editionManager = editionManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;
            _userAppService = userAppService;
            _userManager = userManager;
            _clientAppService = clientAppService;
            _serviceAppService = serviceAppService;
            _addressService = addressService;
            _registerRepository = registerRepository;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _certificateRepository = certificateRepository;
            _clientManager = clientManager;
            _reportSettingsAppService = reportSettingsAppService;
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _drawerRepository = drawerRepository;
            _branchOfficesRepository = branchOfficesRepository;
            _editionFeatureRepository = editionFeatureRepository;
        }

        public ListResultDto<TenantListDto> GetTenants()
        {
            return new ListResultDto<TenantListDto>(
                _tenantManager.Tenants
                    .OrderBy(t => t.TenancyName)
                    .ToList()
                    .MapTo<List<TenantListDto>>()
                );
        }

        public async Task Create(CreateTenantInput input)
        {

            Tenant @tenant;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                input.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                    ? null
                    : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
                input.local = input.local ?? "001";
                input.ConditionSaleType = Invoices.XSD.FacturaElectronicaCondicionVenta.Contado;

                //var defaultEdition =  _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
                //if (defaultEdition != null)
                //{
                //    input.EditionId = defaultEdition.Id;
                //}

                @tenant = Tenant.Create(input.TenancyName, input.Name, input.ConnectionString, input.CodigoMoneda, input.BarrioId, input.local,
                    input.ConditionSaleType, input.CountryID, input.BussinesName, input.IdentificationType, input.IdentificationNumber, input.ComercialName,
                   input.Address, input.PhoneNumber, input.Fax, input.Email, input.EditionId, false, false, false, false);

                if (input.Logo != null && input.Logo.InputStream != null)
                {
                    tenant.Logo = input.Logo.InputStream.ToByteArray();
                }
                unitOfWork.Complete();
            }

            using (CurrentUnitOfWork.SetTenantId(@tenant.Id))
            {
                CheckErrors(await _roleManager.CreateStaticRoles(@tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); //To get static role ids

                //grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                //Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress, User.DefaultPassword);
                CheckErrors(await UserManager.CreateAsync(adminUser));
                await CurrentUnitOfWork.SaveChangesAsync(); //To get admin user's id

                //Assign admin user to role!
                CheckErrors(await UserManager.AddToRoleAsync(adminUser.Id, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public bool validTenantClientIdentification(CreateTenantInput input)
        {
            Tenant ticoPayTenant = _tenantRepository.GetAll().Where(t => t.TenancyName == "ticopay").FirstOrDefault();
            try
            {
                if (_clientManager.ExistIdentification(ticoPayTenant.Id, input.IdentificationNumber) && _tenantRepository.Count(t => t.IdentificationNumber == input.IdentificationNumber) > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [UnitOfWork]
        public async Task<CreateTenantOutput> CreateTenant(CreateTenantInput input)
        {
            Tenant ticoPayTenant = _tenantRepository.GetAll().Where(t => t.TenancyName == "ticopay").FirstOrDefault();

            if (validTenantClientIdentification(input))
                throw new UserFriendlyException("Existe un cliente con el mismo número de cédula.");

            if (IsTenancyNameTaken(input.TenancyName))
                throw new UserFriendlyException("Nombre de sub dominio es inválido.");

            var tenant = input.MapTo<Tenant>();
            tenant.ConnectionString = input.ConnectionString ?? SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
            tenant.local = tenant.local ?? "001";
            tenant.IsTutorialClients = false;
            tenant.IsTutorialCompania = false;
            tenant.IsTutotialServices = false;
            tenant.IsTutorialProduct = false;
            tenant.ConditionSaleType = Invoices.XSD.FacturaElectronicaCondicionVenta.Contado;
            tenant.CostoSms = ticoPayTenant.CostoSms;
            tenant.NearInvoicesMonthlyLimitNotificationInterval = 7;
            tenant.InvoicesMonthlyLimitNotificationInterval = 7;
            if (input.Logo != null && input.Logo.InputStream != null)
            {
                tenant.Logo = input.Logo.InputStream.ToByteArray();
            }

            if (input.EditionId <= 0)
            {
                var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.FreeEditionName);
                if (defaultEdition != null)
                {
                    tenant.EditionId = defaultEdition.Id;
                }
            }
            var currentEdition = await _editionManager.FindByIdAsync(tenant.EditionId.Value);
            var service = _serviceAppService.GetServicesEntities().Where(s => s.Name == currentEdition.DisplayName).OrderByDescending(s => s.CreationTime).FirstOrDefault();

            //CheckErrors(await TenantManager.CreateAsync(tenant));
            await TenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new tenant's id.

            //Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            //We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                //Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); //To get static role ids

                //grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                //Create admin user for the tenant
                var adminUser = User.CreateTenantUser(tenant.Id, input.AdminUserName, input.AdminName, input.AdminSurname, input.AdminEmailAddress, input.AdminPassword);
                CheckErrors(await UserManager.CreateAsync(adminUser));
                await CurrentUnitOfWork.SaveChangesAsync(); //To get admin user's id

                //Assign admin user to role!
                CheckErrors(await UserManager.AddToRoleAsync(adminUser.Id, adminRole.Name));

                // se crea la sucursal principal
                BranchOffice branchOffice = BranchOffice.Create(tenant.Id, "Principal", "001", string.Empty);
                _branchOfficesRepository.Insert(branchOffice);
                await CurrentUnitOfWork.SaveChangesAsync();

                // se crea la caja principal
                var drawer = Drawer.Create(tenant.Id, "00001", "Caja 001", branchOffice.Id);
                _drawerRepository.Insert(drawer);
                await CurrentUnitOfWork.SaveChangesAsync();

                _registerRepository.Insert(Register.Create(tenant.Id, "Caja 001", "00001", 1, 0, 1, 0, 1, 0, 1, 0, "", "", true, false, false, true, true, false, drawer.Id));

                adminUser.DrawerUsers.Add(new DrawerUser { DrawerId = drawer.Id, Drawer = drawer, IsActive = true }); // permiso al usuario administrador a la caja principal

                await _notificationSubscriptionManager.SubscribeAsync(new UserIdentifier(tenant.Id, adminUser.Id), "InvoicesMonthlyLimit");
                await _notificationSubscriptionManager.SubscribeAsync(new UserIdentifier(tenant.Id, adminUser.Id), "NearInvoicesMonthlyLimit");

                _reportSettingsAppService.CreateDefaultConfigurationIfNone(tenant.Id);

                await CurrentUnitOfWork.SaveChangesAsync();

                using (UnitOfWorkManager.Current.SetTenantId(ticoPayTenant.Id))
                {
                    if (!_clientManager.ExistIdentification(ticoPayTenant.Id, input.IdentificationNumber))
                    {
                        UnitOfWorkManager.Current.DisableFilter("MustHaveTenant");
                        AddTenantToTicoPayClientsList(ticoPayTenant.Id, tenant, adminUser, service, input.ProvinciaID, input.DistritoID, input.CantonID);
                    }
                    else
                    {
                        Client client = _clientAppService.GetClintByIdentification(input.IdentificationNumber, ticoPayTenant.Id);
                        AddTenantToTicoPayClientsList(client, service);
                        _clientRepository.Update(client);
                    }
                }

                return new CreateTenantOutput { Tenant = tenant, AdminUser = adminUser };
            }
        }

        public UpdateTenantInput GetEdit(int id)
        {

            // var tenant = _tenantManager.Get(id);
            var query = from tenant in _tenantRepository.GetAll()
                        //join s in _registerRepository.GetAll() on tenant.Id equals s.TenantId
                        join certificate in _certificateRepository.GetAll() on tenant.Id equals (int?)certificate.TenantID ?? 0
                        into temp
                        from j in temp.DefaultIfEmpty()
                        where tenant.Id == id
                        select new UpdateTenantInput
                        {
                            Id = tenant.Id,
                            TenancyName = tenant.TenancyName,
                            Name = tenant.Name,
                            CodigoMoneda = tenant.CodigoMoneda,
                            BarrioId = tenant.BarrioId.Value,
                            local = tenant.local,
                            ConditionSaleType = tenant.ConditionSaleType,
                            CountryID = tenant.CountryID.Value,
                            BussinesName = tenant.BussinesName,
                            IdentificationType = tenant.IdentificationType,
                            IdentificationNumber = tenant.IdentificationNumber,
                            ComercialName = tenant.ComercialName,
                            PhoneNumber = tenant.PhoneNumber,
                            Address = tenant.Address,
                            Fax = tenant.Fax,
                            Email = tenant.Email,
                            AlternativeEmail = tenant.AlternativeEmail,
                            EditionId = tenant.EditionId.Value,
                            ValidateHacienda = tenant.ValidateHacienda,
                            UserTribunet=tenant.UserTribunet,
                            PasswordTribunet=tenant.PasswordTribunet,
                            //CertifiedRoute = j.CertifiedRoute,
                            //CertifiedPassword = j.Password,
                            CertifiedID = j.FileName != null ? (int?)j.Id : null,
                            //LastInvoiceNumber = s.LastInvoiceNumber,
                            //LastNoteCreditNumber = s.LastNoteCreditNumber,
                            //LastNoteDebitNumber = s.LastNoteDebitNumber,
                            //LastVoucherNumber = s.LastVoucherNumber,
                            //LastTicketNumber=s.LastTicketNumber,
                            //RegisterID = s.Id,
                            LogoData = tenant.Logo,
                            SmsNoficicarFacturaACobro = tenant.SmsNoficicarFacturaACobro,
                            CostoSms = tenant.CostoSms,
                            IsAddressShort = tenant.IsAddressShort,
                            AddressShort = tenant.AddressShort,
                            Sector = tenant.Sector,
                            TipoFirma = tenant.TipoFirma,
                            FirmaRecurrente = tenant.FirmaRecurrente,
                            UnitMeasurementDefault = tenant.UnitMeasurementDefault,
                            UnitMeasurementOthersDefault = tenant.UnitMeasurementOthersDefault,
                            IsConvertUSD=tenant.IsConvertUSD,                          
                            IsPos=tenant.IsPos,
                            PrinterType= tenant.PrinterType,
                            ShowServiceCodePdf = tenant.ShowServiceCodePdf
                        };

            return query.ToList().FirstOrDefault();

            //if (tenant == null)
            //{
            //    throw new UserFriendlyException("Could not found the tenant, may be it's deleted.");
            //}
            //return Mapper.Map<UpdateTenantInput>(tenant);
        }


        public UpdateTenantInput GetById(int id)
        {
            var query = from tenant in _tenantRepository.GetAll()
                        join s in _registerRepository.GetAll() on tenant.Id equals s.TenantId
                        where tenant.Id == id
                        select new UpdateTenantInput
                        {
                            Id = tenant.Id,
                            TenancyName = tenant.TenancyName,
                            Name = tenant.Name,
                            CodigoMoneda = tenant.CodigoMoneda,
                            currencyCode = tenant.CodigoMoneda.ToString(),
                            UnitMeasurementDefault = tenant.UnitMeasurementDefault,
                            UnitMeasurementOthersDefault = tenant.UnitMeasurementOthersDefault,
                            BarrioId = tenant.BarrioId.Value,
                            local = tenant.local,
                            ConditionSaleType = tenant.ConditionSaleType,
                            CountryID = tenant.CountryID.Value,
                            BussinesName = tenant.BussinesName,
                            IdentificationType = tenant.IdentificationType,
                            IdentificationNumber = tenant.IdentificationNumber,
                            ComercialName = tenant.ComercialName,
                            PhoneNumber = tenant.PhoneNumber,
                            Address = tenant.Address,
                            Fax = tenant.Fax,
                            Email = tenant.Email,
                            AlternativeEmail = tenant.AlternativeEmail,
                            EditionId = tenant.EditionId.Value,
                            ValidateHacienda = tenant.ValidateHacienda,
                            UserTribunet = tenant.UserTribunet,
                            PasswordTribunet = tenant.PasswordTribunet,
                            RegisterID = s.Id,
                            TipoFirma = tenant.TipoFirma,
                            FirmaRecurrente = tenant.FirmaRecurrente,
                            LastInvoiceNumber = s.LastInvoiceNumber,
                            LastNoteCreditNumber = s.LastNoteCreditNumber,
                            LastNoteDebitNumber = s.LastNoteDebitNumber,
                            LastVoucherNumber = s.LastVoucherNumber,
                            LastTicketNumber = s.LastTicketNumber,
                            IsPos = tenant.IsPos,
                            PrinterType = tenant.PrinterType
                        };

            return query.ToList().FirstOrDefault();


        }
        /// <summary>
        /// Obtiene el certificado de un Tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CertifiedTenantOutput GetCertifiedTenant(int id)
        {
            var @tenant = _certificateRepository.GetAll().Where(a => a.TenantID == id).FirstOrDefault();

            if (@tenant == null)
            {
                throw new UserFriendlyException("Could not found the certified, maybe it's deleted.");
            }

            return Mapper.Map<CertifiedTenantOutput>(@tenant);
        }


        /// <summary>
        /// Obtiene el certificado de un Tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Certificate GetCertified(int id)
        {
            var @tenant = _certificateRepository.GetAll().Where(a => a.TenantID == id).FirstOrDefault();

            if (@tenant == null)
            {
                //throw new UserFriendlyException("Could not found the certified, maybe it's deleted.");
            }

            return @tenant;
        }

        public List<TicoPayEdition> GetAllEditions()
        {
            return _editionManager.GetActiveEditions();
        }

        public List<TicoPayEdition> GetAllTicoPayEditions()
        {
            return _editionManager.GetActiveTicoPayEditions();
        }

        public TicoPayEdition GetTenantTicopayEdition(int editionId)
        {
            return _editionManager.GetEditionApi(editionId).FirstOrDefault();
        }

        [UnitOfWork]
        public async Task Update(UpdateTenantInput input)
        {
            var @tenant = _tenantRepository.Get(input.Id);

            if (input == null)
            {
                throw new UserFriendlyException("Could not found the tenant, maybe it's deleted.");
            }
            else
            {
                if (tenant.IdentificationNumber != input.IdentificationNumber || tenant.IdentificationType != input.IdentificationType)
                {
                    throw new UserFriendlyException("Para cambiar el número o tipo de identifiación escriba un correo a soporte@ticopays.com");
                }
            }
            if (input.ValidateHacienda)
            {
               
                ValidateTribunet validateTribunet = new ValidateTribunet();
                TokenResponse tokenResponse = await validateTribunet.LoginAsync(input.UserTribunet, input.PasswordTribunet);
                if (tokenResponse == null || tokenResponse.IsError)
                {
                    throw new UserFriendlyException("Error Validando credeciales contra hacienda. Por favor verifique e intente nuevamente.");
                }
            }
            @tenant.Name = input.Name;
            @tenant.Address = input.Address;
            @tenant.BarrioId = input.BarrioId;
            @tenant.BussinesName = input.ComercialName;
            @tenant.CodigoMoneda = input.CodigoMoneda;
            @tenant.ComercialName = input.ComercialName;
            @tenant.CountryID = input.CountryID;
            var LastEditionId = @tenant.EditionId;
            @tenant.EditionId = input.EditionId;
            if (input.EditionId != 1)
            {
                @tenant.ValidateHacienda = input.ValidateHacienda;
            }
            else
            {
                @tenant.ValidateHacienda = false;
            }
            @tenant.Email = input.Email;
            @tenant.Fax = input.Fax;
            @tenant.IdentificationNumber = input.IdentificationNumber;
            @tenant.IdentificationType = input.IdentificationType;
            @tenant.local = "001";
            @tenant.ConditionSaleType = Invoices.XSD.FacturaElectronicaCondicionVenta.Contado;
            @tenant.PhoneNumber = input.PhoneNumber;
            @tenant.TenancyName = input.TenancyName;
            @tenant.AlternativeEmail = input.AlternativeEmail;
            @tenant.IsTutorialCompania = true;
            @tenant.UnitMeasurementDefault = input.UnitMeasurementDefault;
            @tenant.UnitMeasurementOthersDefault = input.UnitMeasurementOthersDefault;
            if (input.LogoFile != null && input.LogoFile.InputStream != null)
            {
                tenant.Logo = input.LogoFile.InputStream.ToByteArray();
            }
            else if(input.DeleteLogo)
            {
                tenant.Logo = null;
            }
            decimal oldCostoSms = tenant.CostoSms;
            tenant.CostoSms = input.CostoSms;
            tenant.SmsNoficicarFacturaACobro = input.SmsNoficicarFacturaACobro;
            tenant.Sector = input.Sector;
            if (tenant.ValidateHacienda)
            {
                if ((!string.IsNullOrEmpty(input.UserTribunet)) && (!string.IsNullOrEmpty(input.PasswordTribunet)))
                {
                    tenant.UserTribunet = input.UserTribunet;
                    tenant.PasswordTribunet = CryptoHelper.Encriptar(input.PasswordTribunet);
               
                }
                tenant.TipoFirma = input.TipoFirma;
                if (tenant.TipoFirma.Equals(Tenant.FirmType.Llave) || tenant.TipoFirma.Equals(Tenant.FirmType.Firma))
                {
                    tenant.FirmaRecurrente = input.TipoFirma; ;
                }
                else
                {
                    tenant.FirmaRecurrente = input.FirmaRecurrente;
                }

                if (tenant.TipoFirma == Tenant.FirmType.Firma || tenant.TipoFirma == Tenant.FirmType.Todos)
                {
                    User user = await _userManager.GetUserByIdAsync((long)AbpSession.UserId);
                    foreach (var rol in user.Roles)
                    {
                        if (user.Permissions.Count(d => d.Name == PermissionNames.Download_Manual_digital_Signature_Installer) == 0)
                        {
                            user.Permissions.Add(new Abp.Authorization.Users.UserPermissionSetting
                            {
                                Name = PermissionNames.Download_Manual_digital_Signature_Installer,
                                IsGranted = true,
                                TenantId = rol.TenantId
                            });
                        }
                        if (user.Permissions.Count(d => d.Name == PermissionNames.Download_Manual_digital_Signature) == 0)
                        {
                            user.Permissions.Add(new Abp.Authorization.Users.UserPermissionSetting
                            {
                                Name = PermissionNames.Download_Manual_digital_Signature,
                                IsGranted = true,
                                TenantId = rol.TenantId
                            });
                        }
                        if (user.Permissions.Count(d => d.Name == PermissionNames.Download_Installer_digital_Signature) == 0)
                        {
                            user.Permissions.Add(new Abp.Authorization.Users.UserPermissionSetting
                            {
                                Name = PermissionNames.Download_Installer_digital_Signature,
                                IsGranted = true,
                                TenantId = rol.TenantId
                            });
                        }
                    }
                    await _userManager.UpdateAsync(user);
                }
            }
            else
            {
                tenant.TipoFirma = null;
                tenant.FirmaRecurrente = null;
                tenant.UserTribunet = null;
                tenant.PasswordTribunet = null;

            }

            tenant.ValidateHacienda = input.ValidateHacienda;

            if (oldCostoSms != tenant.CostoSms)
            {
                List<ClientDto> clients = _clientAppService.GetClientsByTenantId(tenant.Id);
                foreach (var ticoPayClient in clients)
                {
                    var ticoPayClientTenant = _tenantRepository.GetAll().Where(t => t.IdentificationNumber == ticoPayClient.Identification && t.SmsNoficicarFacturaACobro && t.CodigoMoneda == input.CodigoMoneda).FirstOrDefault();
                    if (ticoPayClientTenant != null)
                    {
                        ticoPayClientTenant.CostoSms = tenant.CostoSms;
                    }
                }
            }

            tenant.IsAddressShort = input.IsAddressShort;
            tenant.AddressShort = input.AddressShort;
            tenant.IsPos = input.IsPos;
            tenant.PrinterType = input.IsPos? input.PrinterType:null;
            tenant.ShowServiceCodePdf = input.ShowServiceCodePdf;
           
           _tenantRepository.Update(@tenant);

            Certificate cert = _certificateRepository.GetAll().Where(c => c.TenantID == input.Id).FirstOrDefault();
            // actualiza el certificado
            if (input.FileName != null)
            {
                if (cert == null)
                {
                    cert = new Certificate
                    {
                        TenantID = input.Id,
                        Installed = false,
                        CertifiedRoute = input.CertifiedRoute,
                        Password = input.CertifiedPassword,
                        FileName = input.FileName
                    };
                    _certificateRepository.Insert(cert);
                }
                else
                {
                    cert.FileName = input.FileName;
                    cert.CertifiedRoute = input.CertifiedRoute;
                    cert.Password = input.CertifiedPassword;
                    _certificateRepository.Update(cert);
                }
            }
            else
            {
                if (!input.ValidateHacienda && cert != null)
                {
                    cert.FileName = null;
                    cert.Password = "0000";
                    await _certificateRepository.UpdateAsync(cert);
                }

            }

            tenant.IsConvertUSD = input.IsConvertUSD;

            if (LastEditionId != tenant.EditionId)
            {
                Tenant ticoPayTenant = _tenantRepository.GetAll().Where(t => t.TenancyName == "ticopay").FirstOrDefault();

                using (UnitOfWorkManager.Current.SetTenantId(ticoPayTenant.Id))
                {
                    Client client = _clientAppService.GetClintByIdentification(input.IdentificationNumber);
                    if (client != null)
                    {
                        var clientService = _clientManager.GetAllListClientService(client.Id).FirstOrDefault();
                        var service = _serviceAppService.GetServicesEntities().Where(s => s.Name == @tenant.Edition.DisplayName).OrderByDescending(s => s.Id).FirstOrDefault();
                        if (service != null)
                        {
                            if (clientService != null)
                            {
                                var clientServiceAdjustment = _clientManager.GetAllListClientServiceAdjustment(client.Id).FirstOrDefault();

                                if (clientServiceAdjustment == null && clientService.LastGeneratedInvoice != null)
                                {
                                    var edition = _editionManager.GetEdition((int)LastEditionId);
                                    string LastEditionDisplayName = edition.DisplayName;
                                    var invoice = _invoiceRepository.GetAllList(d => d.InvoiceLines.Any(f => f.Service.Name == LastEditionDisplayName && d.ClientId == client.Id)).FirstOrDefault();

                                    if (invoice != null && invoice.Status == Status.Completed)
                                    {
                                        clientService.SetServiceId(service.Id);

                                        if (clientService.WorkerLastEjecutionDate != null)
                                        {
                                            var cronExpression = (service.CronExpression != InvoiceAppService.CRON_ANNUAL) ? service.CronExpression : string.Format(InvoiceAppService.CRON_ANNUAL, client.CreationTime.Month);

                                            var tempCron = new CronExpression(cronExpression);

                                            var nextExecutionDate = tempCron.GetNextValidTimeAfter((DateTime)clientService.WorkerLastEjecutionDate).Value.LocalDateTime;

                                            clientService.WorkerNextEjecutionDate = nextExecutionDate;
                                        }

                                        decimal Quantity = 0;

                                        bool IsEditionAnnual = false;
                                        if ((LastEditionDisplayName == EditionManager.PymeJrAnnualEditionDisplayName || LastEditionDisplayName == EditionManager.ProfesionalAnnualEditionDisplayName || LastEditionDisplayName == EditionManager.PymeJrAnnualEditionDisplayName
                                || LastEditionDisplayName == EditionManager.Pyme1AnnualEditionDisplayName || LastEditionDisplayName == EditionManager.Pyme2AnnualEditionDisplayName))
                                        {
                                            IsEditionAnnual = true;
                                        }
                                        var DaysDiff = DateTime.DaysInMonth(DateTimeZone.Now().Year, DateTimeZone.Now().Month) - DateTimeZone.Now().Day;
                                        decimal AmountToBilled = 0;
                                        decimal AmountToBilledLastEdition = 0;
                                        if ((@tenant.Edition.DisplayName == EditionManager.PymeJrAnnualEditionDisplayName || @tenant.Edition.DisplayName == EditionManager.ProfesionalAnnualEditionDisplayName || @tenant.Edition.DisplayName == EditionManager.PymeJrAnnualEditionDisplayName
                                    || @tenant.Edition.DisplayName == EditionManager.Pyme1AnnualEditionDisplayName || @tenant.Edition.DisplayName == EditionManager.Pyme2AnnualEditionDisplayName))
                                        {

                                            if (!IsEditionAnnual)
                                            {
                                                AmountToBilled = (DaysDiff * invoice.InvoiceLines.FirstOrDefault().Service.Price) / DateTime.DaysInMonth(DateTimeZone.Now().Year, DateTimeZone.Now().Month);
                                                AmountToBilledLastEdition = (DaysDiff * service.Price) / DateTime.DaysInMonth(DateTimeZone.Now().Year, DateTimeZone.Now().Month);
                                            }
                                            else
                                            {
                                                var MonthDiff = 12 - DateTimeZone.Now().Month;
                                                AmountToBilled = (MonthDiff * (invoice.InvoiceLines.FirstOrDefault().Service.Price)) / 12;
                                                AmountToBilledLastEdition = (MonthDiff * (service.Price)) / 12;
                                            }

                                            Quantity = Math.Round((((AmountToBilledLastEdition - AmountToBilled) * 1) / service.Price), 2);
                                        }
                                        else
                                        {
                                            AmountToBilled = (DaysDiff * invoice.InvoiceLines.FirstOrDefault().Service.Price) / DateTime.DaysInMonth(DateTimeZone.Now().Year, DateTimeZone.Now().Month);
                                            AmountToBilledLastEdition = (DaysDiff * invoice.InvoiceLines.FirstOrDefault().Service.Price) / DateTime.DaysInMonth(DateTimeZone.Now().Year, DateTimeZone.Now().Month);

                                            Quantity = Math.Round((((AmountToBilledLastEdition - AmountToBilled) * 1) / service.Price), 2);
                                        }

                                        DateTime NextBillDate;
                                        DateTime InitDate;
                                        if (DateTimeZone.Now().Day <= 15)
                                        {
                                            NextBillDate = DateTimeZone.Now();
                                            InitDate = new DateTime(NextBillDate.Year, NextBillDate.Month, NextBillDate.Day);
                                        }
                                        else
                                        {
                                            NextBillDate = DateTimeZone.Now().AddMonths(1);
                                            InitDate = new DateTime(NextBillDate.Year, NextBillDate.Month, 1);
                                        }

                                        if (Quantity != 0)
                                        {
                                            _clientManager.AddServiceToClient(service, client, "", InitDate, false, false, Quantity, 0, ClientServiceState.Adjustment);
                                        }
                                        _clientManager.UpdateClientService(clientService);
                                    }
                                    else if (invoice != null && invoice.Status == Status.Parked) {
                                        tenant.EditionId = LastEditionId;
                                        input.EditionId = (int)LastEditionId;
                                        throw new UserFriendlyException("No es posible realizar un cambio de plan ya que tiene un factura pendiente por pagar.");
                                    }
                                } 
                                else if (clientServiceAdjustment == null && clientService.LastGeneratedInvoice == null)
                                {
                                    tenant.EditionId = LastEditionId;
                                    input.EditionId = (int)LastEditionId;
                                    throw new UserFriendlyException("Nos encontramos generando su factura, posteriormente que cancele la primera factura podra hacer el cambio a un plan superior.");
                                }
                                else
                                {
                                    tenant.EditionId = LastEditionId;
                                    input.EditionId = (int)LastEditionId;
                                    throw new UserFriendlyException("No es posible realizar un cambio de plan en menos de un mes.");
                                }
                            }
                        }
                    }

                    await UnitOfWorkManager.Current.SaveChangesAsync();
                } 
            }

            //// actualiza el email
            //var user = _userAppService.GetUserByRole(StaticRoleNames.Tenants.Admin);

            //user.EmailAddress = input.AdminEmailAddress;

            //await _userManager.SetEmailAsync(user.Id, user.EmailAddress);

            // actualiza el register 
            //var register = _registerRepository.Get(input.RegisterID);
            //register.LastNoteDebitNumber = input.LastNoteDebitNumber;
            //register.LastNoteCreditNumber = input.LastNoteCreditNumber;
            //register.LastInvoiceNumber = input.LastInvoiceNumber;
            //register.LastVoucherNumber = input.LastVoucherNumber;
            //register.LastTicketNumber = input.LastTicketNumber;
            //_registerRepository.Update(register);

            _reportSettingsAppService.CreateDefaultConfigurationIfNone(tenant.Id);
        }

        [UnitOfWork]
        public async Task Update(TenantDto input)
        {
            var @tenant = _tenantRepository.Get(input.Id);

            if (input == null)
            {
                throw new UserFriendlyException("Could not found the tenant, maybe it's deleted.");
            }

            @tenant.Name = input.Name;
            @tenant.Address = input.Address;
            @tenant.BarrioId = input.BarrioId;
            @tenant.BussinesName = input.ComercialName;
            @tenant.CodigoMoneda = input.CodigoMoneda;
            @tenant.ComercialName = input.ComercialName;
            @tenant.CountryID = input.CountryID;
            // @tenant.EditionId = input.EditionId;
            @tenant.Email = input.Email;
            @tenant.Fax = input.Fax;
            // @tenant.IdentificationNumber = input.IdentificationNumber;
            // @tenant.IdentificationType = input.IdentificationType;
            @tenant.PhoneNumber = input.PhoneNumber;
            // @tenant.TenancyName = input.TenancyName;
            @tenant.AlternativeEmail = input.AlternativeEmail;
            @tenant.IsTutorialCompania = true;


            // actualiza el register
            var register = _registerRepository.Get(input.RegisterID);
            register.LastNoteDebitNumber = input.LastNoteDebitNumber;
            register.LastNoteCreditNumber = input.LastNoteCreditNumber;
            register.LastInvoiceNumber = input.LastInvoiceNumber;
            _registerRepository.Update(register);

            _reportSettingsAppService.CreateDefaultConfigurationIfNone(tenant.Id);
        }
        /// <summary>
        /// Obtiene los tenat que estan configurados a ese puerto - banco
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public List<AgreementConectivity> GetTenantsByPort(int? port = null)
        {
            List<AgreementConectivity> tenant = new List<AgreementConectivity>();

            if (port!=null)
            {
                tenant = TenantManager.GetTenantsAgreementByPort((int)port);
            }
            else
            {
                tenant = TenantManager.GetTenantsAgreement();
            }

            //List<int> ListTenant = new List<int>();

            //foreach (var item in tenant)
            //{
            //    ListTenant.Add(item.TenantID);
            //}

            return tenant;
        }

        /// <summary>
        /// Obtiene el tipo de acceso configurado -- banco
        /// </summary>
        /// <param name="listTenant"></param>
        /// <returns></returns>
        public TipoLLaveAcceso GetTenantTipoAcceso(List<int> listTenant)
        {
            return TenantManager.GetTenantsKeyType(listTenant);
        }

        private void AddTenantToTicoPayClientsList(int ticoPayTenantId, Tenant tenant, User admin, Service service, int provinciaID, int distritoID, int cantonID)
        {
            if (tenant != null)
            {
                Country country = null;
                if (tenant.CountryID != null)
                {
                    country = _addressService.GetCountryById(tenant.CountryID.Value);
                }
                CreateClientInput client = new CreateClientInput
                {
                    Name = tenant.Name,
                    LastName = "N/D",
                    NameComercial = tenant.ComercialName,
                    Identification = tenant.IdentificationNumber,
                    IdentificationType = tenant.IdentificationType,
                    Birthday = DateTime.Now,
                    Gender = Gender.Other,
                    PhoneNumber = tenant.PhoneNumber,
                    MobilNumber = tenant.PhoneNumber,
                    Fax = tenant.Fax,
                    Email = tenant.Email,
                    BarrioId = tenant.BarrioId,
                    ProvinciaID = provinciaID,
                    DistritoID = distritoID,
                    CantonID = cantonID,
                    IsoCountry = country != null ? country.CountryCode : null,
                    Address = tenant.Address,
                    ContactName = admin.Name,
                    ContactMobilNumber = tenant.PhoneNumber,
                    ContactPhoneNumber = tenant.PhoneNumber,
                    ContactEmail = admin.EmailAddress,
                    Note = "TICOPAY CUSTOMER",
                };
                if (service != null)
                {
                    var serv = service.MapTo<ServiceDto>();
                    client.ClientServiceList = new List<ServiceDto>();
                    client.ClientServiceList.Add(serv);
                }
                _clientAppService.Create(ticoPayTenantId, client);
            }
        }

        private void AddTenantToTicoPayClientsList(Client client, Service service)
        {
            _clientManager.AddServiceToClient(service, @client, "", DateTimeZone.Now(), false, false, 1, 0);
        }

        [UnitOfWork]
        [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
        public void SendPayNotificationIfNeeded(Tenant tenant, int daysOfGrace)
        {
            if (tenant == null)
                return;

            if ((tenant.LastPayNotificationSendedAt == null && !HasPayFirstInvoice(tenant.IdentificationNumber) && PassFirstNotificationDay(tenant)) || ((DateTime.UtcNow - tenant.LastPayNotificationSendedAt)?.Days >= 7))
            {
                SendPayNotification(tenant, daysOfGrace);
                tenant.LastPayNotificationSendedAt = DateTime.UtcNow;
            }

            if (daysOfGrace <= 0 && !IsClientExonerate(tenant.IdentificationNumber))
            {
                SendAccountSuspendNotification(tenant);
                tenant.IsActive = false;
                tenant.MotiveSuspension = Tenant.InactiveReason.InvoicePending;
            }

            _tenantRepository.Update(tenant);
            _unitOfWorkManager.Current.SaveChanges();
        }

        public bool IsClientExonerate(string IdentificationNumber)
        {
            Client client = _clientAppService.GetTicoPayClientByIdentification(IdentificationNumber);
            if (client != null && client.Groups.Count > 0 && client.Groups.Any(g => g.Group.Name == "AsadaCloud"))
            {
                return true;
            }
            return false;
        }


        public bool HasPayFirstInvoice(string IdentificationNumber)
        {
            Client client = _clientAppService.GetTicoPayClientByIdentification(IdentificationNumber);
            if (client != null)
            {

                var tenantCreationDate = client.CreationTime;

                var bottomInvoiceLimit = DateTimeZone.Convert(tenantCreationDate);
                var topInvoiceLimit = bottomInvoiceLimit.AddMonths(1);

                var recentlyInvoice = _invoiceRepository.Count(d => d.Tenant.Id == 2 && d.ClientId == client.Id && d.DueDate >= bottomInvoiceLimit && d.DueDate <= topInvoiceLimit && d.Status == Status.Parked);

                return (recentlyInvoice >= 1) ? false : true;

            }
            return true;
        }

        public bool PassFirstNotificationDay(Tenant tenant)
        {
            Client client = _clientAppService.GetTicoPayClientByIdentification(tenant.IdentificationNumber);

            var tenantCreationDate = client.CreationTime;

            var bottomInvoiceLimit = DateTimeZone.Convert(tenantCreationDate);
            var topInvoiceLimit = bottomInvoiceLimit.AddMonths(1);

            var recentlyInvoice = _invoiceRepository.GetAll().Where(d => d.Tenant.Id == 2 && d.ClientId == client.Id && d.DueDate >= bottomInvoiceLimit && d.DueDate <= topInvoiceLimit && d.Status == Status.Parked).FirstOrDefault();

            var f = (recentlyInvoice != null) ? (DateTimeZone.Now() - recentlyInvoice.DueDate).Days == 1 : false;
            return f;

        }

        private void SendPayNotification(Tenant tenant, int daysOfGrace)
        {
            SendMailTP mail = new SendMailTP();
            StringBuilder body = new StringBuilder();
            body.Append("<p>Estimado " + tenant.Name + "</p>");
            body.Append("<br/>");
            body.Append("<p>Actualmente usted posee facturas pendientes de pago, le invitamos a realizar el pago :</p>");
            body.Append(emailCuentaTicopay);
            body.Append($"Estimado cliente se le recuerda que dispone de tres ({daysOfGrace}) días para pagar esta factura, de lo contrario su cuenta será desactivada hasta recibir el pago correspondiente");

            mail.SendNoReplyMail(new string[] { tenant.Email }, "Factura Pendiente de Pago", body.ToString());
        }

        private void SendAccountSuspendNotification(Tenant tenant)
        {
            SendMailTP mail = new SendMailTP();
            StringBuilder body = new StringBuilder();
            body.Append("<p>Estimado " + tenant.Name + "</p>");
            body.Append("<br/>");
            body.Append("<p>Su cuenta ha sido suspendida temporalmente, le invitamos a realizar el pago :</p>");
            body.Append(emailCuentaTicopay);
            body.Append("<p>Por favor enviar soporte de pago a pagos@ticopays.com, y continua disfrutando de todos los beneficios que te ofrece TicoPay.</p>");

            mail.SendNoReplyMail(new string[] { tenant.Email }, "Cuenta Suspendida", body.ToString());
        }

        public TenantDto GetBy(Expression<Func<Tenant, bool>> predicate)
        {
            return _tenantRepository.GetAll().Where(predicate).FirstOrDefault().MapTo<TenantDto>();
        }

        public void UpdateCostoSms(int id, decimal costoSms)
        {
            var tenant = _tenantRepository.Get(id);
            if (tenant == null)
            {
                throw new UserFriendlyException("Could not found the tenant, maybe it's deleted.");
            }
            tenant.CostoSms = costoSms;
            _tenantRepository.Update(tenant);
        }

        public bool IsValidTenancyName(string tenancyName)
        {
            return Regex.IsMatch(tenancyName, Tenant.TenancyNameRegex);
        }

        public bool IsTenancyNameTaken(string tenancyName)
        {
            var tenant = _tenantManager.FindByTenancyName(tenancyName);
            return (tenant != null);
        }

        private void UpdateCertificado(Certificate certificate)
        {
            try
            {
                _certificateRepository.Update(certificate);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public Task<bool> CheckTenantAPIPermission(string tenancyName)
        {
            var tenant = _tenantManager.FindByTenancyName(tenancyName);
            if (tenant != null && tenant.Edition != null && tenant.Edition.DisplayName == EditionManager.ProfesionalJrEditionDisplayName ||
                tenant.Edition.DisplayName == EditionManager.ProfesionalJrAnnualEditionName ||
                tenant.Edition.DisplayName == EditionManager.ProfesionalEditionDisplayName ||
                tenant.Edition.DisplayName == EditionManager.ProfesionalAnnualEditionName ||
                tenant.Edition.DisplayName == EditionManager.FreeEditionDisplayName)
            {
                throw new UserFriendlyException(ErrorCodeHelper.EditionHasNotApiAccess, "El plan de la compañía no tiene acceso al API de Ticopay.");
            }
            else
            {
                return TaskEx.FromResult(false);
            }
        }

        public Task<bool> CheckTenantValidConfig(string tenancyName)
        {
            var tenant = _tenantManager.FindByTenancyName(tenancyName);
            if (tenant != null)
            {
                if ((tenant.TipoFirma == Tenant.FirmType.Llave || tenant.TipoFirma == Tenant.FirmType.Todos) && (tenant.Certificates != null && tenant.Certificates.Count <= 0 && tenant.ValidateHacienda))
                {
                    throw new UserFriendlyException(ErrorCodeHelper.EditionHasNotApiAccess, "La compañía no tiene llave criptográfica configurada.");
                }

                if ((tenant.UserTribunet == null || tenant.PasswordTribunet == null) && tenant.ValidateHacienda)
                {
                    throw new UserFriendlyException(ErrorCodeHelper.TenantHasNotHaciendaCredential, "La compañía no tiene credenciales de hacienda configurada.");
                }

                if (!(tenant.BarrioId != null && tenant.CountryID != null && tenant.Address != null))
                {
                    throw new UserFriendlyException(ErrorCodeHelper.TenantAddressIsNotComplete, "La dirección fiscal de la compañía no esta completa.");
                }
                return TaskEx.FromResult(true);
            }
            else
            {
                return TaskEx.FromResult(false);
            }
        }

        public bool isDrawerEnabled(int tenatId, bool isBranchOffices)
        {
            bool isEnabled = false;
            var tenant = _tenantManager.Get(tenatId);
            if (tenant.EditionId != null)            {
                
                var edition = _editionManager.GetActiveTicoPayEditions().Where(x =>x.Id== tenant.EditionId).FirstOrDefault();

                if (!isBranchOffices &&
                    (edition.Name == EditionManager.ProfesionalEditionName || edition.Name == EditionManager.PymeJrEditionName ||
                    edition.Name == EditionManager.ProfesionalAnnualEditionName || edition.Name == EditionManager.PymeJrAnnualEditionName))
                {

                    var feature = _editionFeatureRepository.GetAll().Where(e => e.EditionId == tenant.EditionId && e.Name == FeatureName.DrawerLimit.ToString()).FirstOrDefault();
                    var drawerCant = _drawerRepository.GetAll().Where(a => a.TenantId == tenatId).Count();
                    int featureValue = Convert.ToInt16(feature.Value);

                    if (drawerCant < featureValue)
                        isEnabled = true;


                }
                else
                {
                    var predicate = PredicateBuilder.New<TicoPayEdition>(true);
                    predicate = predicate.And(d => !d.IsDeleted && d.EditionType == edition.EditionType && d.CloseForSale == edition.CloseForSale
                    && d.Price <= edition.Price && d.Name.Contains(EditionManager.Pyme1EditionName));

                    if (_editionManager.GetActiveTicoPayEditions().Where(predicate).Count() > 0)
                        isEnabled = true;
                }
            }            
            
            return isEnabled;
        }
    }
}