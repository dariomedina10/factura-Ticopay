using Abp;
using Abp.Dependency;
using Castle.Facilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var bootstrapper = new AbpBootstrapper())
                {
                    bootstrapper.Initialize();

                    var tester = IocManager.Instance.Resolve<SocketBNServices>();

                    tester.Run();
                }
                Console.ReadLine();
            }
            catch (Exception e)
            {
            }

        }
    }
}
