using System;
using System.Configuration;
using Abp.Owin;
using Hangfire;
using TicoPay.Api.Controllers;
using TicoPay.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Abp.Hangfire;

[assembly: OwinStartup(typeof(Startup))]

namespace TicoPay.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.UseAbp();
           
            app.UseOAuthBearerAuthentication(AccountController.OAuthBearerOptions);
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
           
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            if (IsTrue("ExternalAuth.Facebook.IsEnabled"))
            {
                app.UseFacebookAuthentication(CreateFacebookAuthOptions());
            }

            if (IsTrue("ExternalAuth.Twitter.IsEnabled"))
            {
                app.UseTwitterAuthentication(CreateTwitterAuthOptions());
            }

            if (IsTrue("ExternalAuth.Google.IsEnabled"))
            {
                app.UseGoogleAuthentication(CreateGoogleAuthOptions());
            }

            app.MapSignalR();
            #region Comentar para hacer debug en produccion sin hangfire
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = String.Format("{0}.{1}",
                Environment.MachineName,
                Guid.NewGuid().ToString())
            });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AbpHangfireAuthorizationFilter() }
            });
            #endregion
        }

        private static FacebookAuthenticationOptions CreateFacebookAuthOptions()
        {
            var options = new FacebookAuthenticationOptions
            {
                AppId = ConfigurationManager.AppSettings["ExternalAuth.Facebook.AppId"],
                AppSecret = ConfigurationManager.AppSettings["ExternalAuth.Facebook.AppSecret"]
            };

            options.Scope.Add("email");
            options.Scope.Add("public_profile");

            return options;
        }

        private static TwitterAuthenticationOptions CreateTwitterAuthOptions()
        {
            return new TwitterAuthenticationOptions
            {
                ConsumerKey = ConfigurationManager.AppSettings["ExternalAuth.Twitter.ConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["ExternalAuth.Twitter.ConsumerSecret"]
            };
        }

        private static GoogleOAuth2AuthenticationOptions CreateGoogleAuthOptions()
        {
            return new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["ExternalAuth.Google.ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["ExternalAuth.Google.ClientSecret"]
            };
        }

        private static bool IsTrue(string appSettingName)
        {
            return string.Equals(
                ConfigurationManager.AppSettings[appSettingName],
                "true",
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}