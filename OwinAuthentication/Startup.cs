using System.Web.Http;
using Owin;
using Microsoft.Owin.Security.OAuth;
using OwinAuthentication.Models;
using System;

namespace OwinAuthentication
{
    /// <summary>
    /// Install-Package Microsoft.AspNet.WebApi.OwinSelfHost -Pre
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This method is required by Katana:
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            // Configuring OWIn Authentication
            // add to 
            ConfigureAuth(app);

            var webApiConfiguration = ConfigureWebApi();
            app.UseWebApi(webApiConfiguration);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new Microsoft.Owin.PathString("/Token"),
                Provider = new ApplicationOAuthServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),

                // Only do this for demo!!
                AllowInsecureHttp = true
            };

            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
                );
            return config;
        }
    }
}
