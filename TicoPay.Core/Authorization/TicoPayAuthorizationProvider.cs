using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace TicoPay.Authorization
{
    public class TicoPayAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //configuracion
            var pages = context.GetPermissionOrNull(PermissionNames.Pages);
            if (pages == null)
            {
                pages = context.CreatePermission(PermissionNames.Pages, L("Configuration"));
            }

            var users = pages.CreateChildPermission(PermissionNames.Pages_Users, L("Users"));

            var tax = pages.CreateChildPermission(PermissionNames.Pages_Tax, L("Tax"));

            var groupclients = pages.CreateChildPermission(PermissionNames.Pages_GroupClients, L("GroupClients"));

            var roles = pages.CreateChildPermission(PermissionNames.Pages_Roles, L("Roles"));

            var tenants = pages.CreateChildPermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Tenant);

            var groupConcepts = pages.CreateChildPermission(PermissionNames.Pages_GroupConcepts, L("GroupConcepts"));

            var groupReportSettings = pages.CreateChildPermission(PermissionNames.Pages_ReportSettings, L("Configuration.ReportSettings"));

            //mantenimiento

            var maintance = context.GetPermissionOrNull(PermissionNames.Maintenance);

            if (maintance == null)
            {
                maintance = context.CreatePermission(PermissionNames.Maintenance, L("Maintenance"));
            }

            var clients = maintance.CreateChildPermission(PermissionNames.Maintenance_Clients, L("Clients"));

            var Services = maintance.CreateChildPermission(PermissionNames.Maintenance_Services, L("Services"));

            var Products = maintance.CreateChildPermission(PermissionNames.Maintenance_Products, L("Products"));
            //facturacion 

            var billingMaster = context.GetPermissionOrNull(PermissionNames.Billing);

            if (billingMaster == null)
            {
                billingMaster = context.CreatePermission(PermissionNames.Billing, L("Billing"));
            }

            var billing = billingMaster.CreateChildPermission(PermissionNames.Billing_Billing, L("Billing.Billing"));

            var confirmXML = billingMaster.CreateChildPermission(PermissionNames.Billing_ConfirmXML, L("Billing.ConfirmXML"));

            var openDrawer= billingMaster.CreateChildPermission(PermissionNames.Billing_OpenDrawer, L("Billing.OpenDrawer"));
            //Confirmar XML 

            //var confirmXML = context.GetPermissionOrNull(PermissionNames.ConfirmXML);

            //if (confirmXML == null)
            //{
            //    confirmXML = context.CreatePermission(PermissionNames.ConfirmXML, L("ConfirmXML"));
            //}

            //reportes
            var reports = context.GetPermissionOrNull(PermissionNames.Reports);

            if (reports == null)
            {
                reports = context.CreatePermission(PermissionNames.Reports, L("Reports"));
            }

            var account = reports.CreateChildPermission(PermissionNames.Reports_AccountsReceivable, L("AccountsReceivable"));

            var reportclients = reports.CreateChildPermission(PermissionNames.Reports_Clients, L("ReportClients"));

            var reportclosing = reports.CreateChildPermission(PermissionNames.Reports_Closing, L("ReportClosing"));

            var reportstatusinvoices = reports.CreateChildPermission(PermissionNames.Reports_StatusInvoices, L("StatusInvoices"));

            var reportinvoicesnotes = reports.CreateChildPermission(PermissionNames.Reports_InvoicesNotes, L("InvoicesNotes"));

            // var invoicesSentToTribunet = reports.CreateChildPermission(PermissionNames.Reports_InvoicesSentToTribunet, L("InvoicesSentToTribunet"));

            //integration
            var integration = context.GetPermissionOrNull(PermissionNames.Integration);

            if (integration == null)
            {
                integration = context.CreatePermission(PermissionNames.Integration, L("Integration"));
            }

            var svConta = integration.CreateChildPermission(PermissionNames.Integration_SVConta, L("Integration.SVConta"));

            var zoho = integration.CreateChildPermission(PermissionNames.Integration_Zoho, L("Integration.Zoho"));

            //var taxAdministration = context.GetPermissionOrNull(PermissionNames.TaxAdministration);

            //if (taxAdministration == null)
            //{
            //    taxAdministration = context.CreatePermission(PermissionNames.TaxAdministration, L("TaxAdministration"));
            //}
            //var viewInvoices = taxAdministration.CreateChildPermission(PermissionNames.TaxAdministration_ViewInvoices, L("ViewInvoices"));

            //reportes
            var downloads = context.GetPermissionOrNull(PermissionNames.Download_Manual_digital_Signature_Installer);

            if (downloads == null)
            {
                downloads = context.CreatePermission(PermissionNames.Download_Manual_digital_Signature_Installer, L("Downloads_FirmaDigital"));
            }

            var downloadsManual = downloads.CreateChildPermission(PermissionNames.Download_Manual_digital_Signature, L("Downloads_FirmaDigital.Manual"));

            var downloadsInstaller = downloads.CreateChildPermission(PermissionNames.Download_Installer_digital_Signature, L("Downloads_FirmaDigital.Installer"));

            var dashboard = context.GetPermissionOrNull(PermissionNames.Dashboard);

            if (dashboard == null)
            {
                dashboard = context.CreatePermission(PermissionNames.Dashboard, L("Dashboard"));
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TicoPayConsts.LocalizationSourceName);
        }
    }
}
