using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Socket
{
   [DependsOn(typeof(TicoPayCoreModule), typeof(TicoPayApplicationModule), typeof(TicoPayDataModule))]
    public class SocketModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

    }
}
