using Abp;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bootstrapper = new AbpBootstrapper())
            {
                bootstrapper.Initialize();

                //Getting a Tester object from DI and running it
                using (var tester = bootstrapper.IocManager.ResolveAsDisposable<SocketBNServices>())
                {
                    tester.Object.GetTenant();
                } //Disposes tester and all it's dependencies

                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
