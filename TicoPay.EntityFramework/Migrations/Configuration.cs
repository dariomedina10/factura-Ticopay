using System.Data.Entity.Migrations;
using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using TicoPay.Migrations.SeedData;
using EntityFramework.DynamicFilters;
using System.Linq;
using TicoPay.Editions;
using TicoPay.Invoices;
using TicoPay.Authorization;
using TicoPay.MultiTenancy;

namespace TicoPay.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<TicoPay.EntityFramework.TicoPayDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TicoPay";
        }

        protected override void Seed(TicoPay.EntityFramework.TicoPayDbContext context)
        {
            context.DisableAllFilters();

            Tenant = context.Tenants.FirstOrDefault(t => t.TenancyName == TicoPay.MultiTenancy.Tenant.DefaultTenantName);

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                new TenantRoleAndUserBuilder(context, 1).Create();

                new TicoPayTenantCreator(context).Create();
                new DefaultFeaturesCreator(context).Create();

                //new ReportsSettingsCreator(context).Create();
            }

            #region Arreglos en planes de TicoPay
            #region Planes Mensuales

            //var profesionalJr = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.ProfesionalJrEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (profesionalJr != null && profesionalJr.CloseForSale)
            //{
            //    new EditionCreator(context, EditionManager.ProfesionalJrEditionName, EditionManager.ProfesionalJrEditionDisplayName, 4, TicopayEditionType.Monthly).Create("10", "1", EditionCreator.CRON_MONTHLY);
            //}

            //var profesional = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.ProfesionalEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (profesional == null || (profesional != null && profesional.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.ProfesionalEditionName, EditionManager.ProfesionalEditionDisplayName, 9, TicopayEditionType.Monthly).Create("500", "2", EditionCreator.CRON_MONTHLY);
            //}

            //var pymeJr = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.PymeJrEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (pymeJr == null || (pymeJr != null && pymeJr.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.PymeJrEditionName, EditionManager.PymeJrEditionDisplayName, 15, TicopayEditionType.Monthly).Create("600", "3", EditionCreator.CRON_MONTHLY);
            //}

            //var pyme1 = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.Pyme1EditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (pyme1 == null || (pyme1 != null && pyme1.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.Pyme1EditionName, EditionManager.Pyme1EditionDisplayName, 30, TicopayEditionType.Monthly).Create("1000", int.MaxValue.ToString(), EditionCreator.CRON_MONTHLY);
            //}

            //var pyme2 = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.Pyme2EditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (pyme2 == null || (pyme2 != null && pyme2.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.Pyme2EditionName, EditionManager.Pyme2EditionDisplayName, 45, TicopayEditionType.Monthly).Create("2000", int.MaxValue.ToString(), EditionCreator.CRON_MONTHLY);
            //}
            #endregion

            #region Planes Anuales

            //var profesionalJrAnnual = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.ProfesionalJrAnnualEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (profesionalJrAnnual == null || (profesionalJrAnnual != null && profesionalJrAnnual.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.ProfesionalJrAnnualEditionName, EditionManager.ProfesionalJrAnualEditionDisplayName, 48, TicopayEditionType.Annual).Create("10", "1", EditionCreator.CRON_ANNUAL);
            //}

            //var profesionalAnnual = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.ProfesionalAnnualEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (profesionalAnnual == null || (profesionalAnnual != null && profesionalAnnual.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.ProfesionalAnnualEditionName, EditionManager.ProfesionalAnnualEditionDisplayName, 108, TicopayEditionType.Annual).Create("500", "2", EditionCreator.CRON_ANNUAL);
            //}

            //var pymeJrAnnual = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.PymeJrAnnualEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (pymeJrAnnual == null || (pymeJrAnnual != null && pymeJrAnnual.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.PymeJrAnnualEditionName, EditionManager.PymeJrAnnualEditionDisplayName, 180, TicopayEditionType.Annual).Create("600", "3", EditionCreator.CRON_ANNUAL);
            //}

            //var pyme1Annual = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.Pyme1AnnualEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (pyme1Annual == null || (pyme1Annual != null && pyme1Annual.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.Pyme1AnnualEditionName, EditionManager.Pyme1AnnualEditionDisplayName, 360, TicopayEditionType.Annual).Create("1000", int.MaxValue.ToString(), EditionCreator.CRON_ANNUAL);
            //}

            //var pyme2Annual = (TicoPayEdition)context.Editions.Where(e => e.Name == EditionManager.Pyme2AnnualEditionName).OrderByDescending(e => e.Id).Take(1).FirstOrDefault();
            //if (pyme2Annual == null || (pyme2Annual != null && pyme2Annual.CloseForSale))
            //{
            //    new EditionCreator(context, EditionManager.Pyme2AnnualEditionName, EditionManager.Pyme2AnnualEditionDisplayName, 540, TicopayEditionType.Annual).Create("2000", int.MaxValue.ToString(), EditionCreator.CRON_ANNUAL);
            //}

            #endregion

            //new PriceEditionCreator(context).Create(true);

            //new TicopayTypeEditionEditor(context).Create();
            #endregion

            //#region Arreglos para agregar limites de cajas
            new AddDrawerLimit(context).Create();
            //#endregion

            //#region Bancos

            new AddListBanks(context).Create();

            //#endregion
            //#region Arreglos en Tenant Ticopay
            //new UpdateAddressShort(context).Create(); 
            //#endregion

            //#region Arreglos de Inconsistencia de datos en facturas

            //new CheckInconsistencyInInvoices(context).Check();

            //new CheckInconsistencyInTicopayInvoices(context).Check();
            //#endregion

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            #region Arreglos de Usuarios de Soporte de www.ticopays.com
            new SupportRoleAndUserCreator(context).Create();
            #endregion

            #region Arreglos de permisos en roles
            //new UpdatePermissionAccess(context).Create(PermissionNames.Billing_ConfirmXML);
            //new UpdatePermissionAccess(context).Create(PermissionNames.Integration);
            //new UpdatePermissionAccess(context).Create(PermissionNames.Integration_SVConta);
            //new UpdatePermissionAccess(context).Create(PermissionNames.Integration_Zoho);
            //new UpdatePermissionAccess(context).Create(PermissionNames.Maintenance_Products);
            //new UpdatePermissionAccess(context).Create(PermissionNames.Dashboard);
            #endregion

            //#region Actualizar Tenant con Llave Criptografica
            //new UpdateFirmType(context).Create();
            //#endregion

            #region Actualizar detalle notas
            //new AddLinesNotes(context).Create();
            #endregion

            #region Actualizar menú facturacion 
            //new updateNavigationSeed(context).create();
            #endregion

            #region Actualizar sucursal y caja por defecto
            //new drawerRegisterSeed(context).create();

            //new drawerRegisterSeed(context).FixOpenDefaultDrawer();
            #endregion

            #region Actualizar sitio de pago

            //new UpdatePaymentOrigin(context).UpdateByPaymentOrigin();

            #endregion
            context.SaveChanges();
        }
    }
}
