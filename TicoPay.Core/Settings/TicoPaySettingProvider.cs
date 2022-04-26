using Abp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Settings
{
    public class TicoPaySettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                    {
                    new SettingDefinition(
                        Abp.Zero.Configuration.AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin ,
                        "false",
                        scopes: SettingScopes.All
                        ),
                };
        }
    }
}
