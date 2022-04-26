using System.Reflection;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Zero;
using Abp.Zero.Configuration;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using TicoPay.Settings;

namespace TicoPay
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class TicoPayCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            //Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);
            //Configuration.Modules.Zero().EntityTypes.Role = typeof(Invoice);
            //Configuration.Modules.Zero().EntityTypes.User = typeof(Note);
            //Configuration.Modules.Zero().EntityTypes.User = typeof(Client);

            //Remove the following line to disable multi-tenancy.
            Configuration.MultiTenancy.IsEnabled = true;

            //Add/remove localization sources here
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    TicoPayConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "TicoPay.Localization.Source"
                        )
                    )
                );

            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Authorization.Providers.Add<TicoPayAuthorizationProvider>();

            Configuration.Settings.Providers.Add<TicoPaySettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
