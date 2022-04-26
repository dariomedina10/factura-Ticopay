using System;
using System.Web.Http;
using Abp.Web;
using Castle.Facilities.Logging;
using System.Web.Mvc;
using Abp.Dependency;
using TicoPay.Common;
using System.Web;
using TicoPay.Web.Binder;

namespace TicoPay.Web
{
    public class MvcApplication : AbpWebApplication<TicoPayWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;

            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            base.Application_Start(sender, e);
        }

        protected override void Application_BeginRequest(object sender, EventArgs e)
        {
            base.Application_BeginRequest(sender, e);

            try
            {
                IocManager.Instance.RegisterIfNot<IBaseUrlResolver, DefaultBaseUrlResolver>(DependencyLifeStyle.Singleton);
            }
            catch (Castle.MicroKernel.ComponentRegistrationException)
            {
            }
            IBaseUrlResolver baseUrlResolver = IocManager.Instance.Resolve<IBaseUrlResolver>();
            if (HttpContext.Current != null && baseUrlResolver != null && !baseUrlResolver.IsResolved)
            {
                baseUrlResolver.ResolveBaseUrlFromRequest(HttpContext.Current.Request);
            }
        }

        protected override void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            base.Application_Error(sender, e);
        }
    }
}
