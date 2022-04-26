using System;
using Abp;
using Abp.Dependency;
using TicoPay;

namespace WorkerInvoiceCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var bootstrapper = AbpBootstrapper.Create<TicoPayApplicationModule>())
                {
                    bootstrapper.Initialize();
                    using (var tester = bootstrapper.IocManager.ResolveAsDisposable<WorkerProcess>())
                    {
                        tester.Object.Process();
                    }
                    //Test_Way_1(bootstrapper.IocManager);
                }
                Console.ReadLine();
            }
            catch (Exception)
            {
               // int hola = 0;
            }
        }

        private static void Test_Way_1(IIocManager iocManager)
        {
            var tester = iocManager.Resolve<WorkerProcess>();
            tester.Process();
            iocManager.Release(tester);
        }

    }
}
