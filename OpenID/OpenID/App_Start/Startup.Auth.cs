using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using OpenAthens.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using System.Threading.Tasks;

namespace OpenID
{
    public partial class Startup
    {
        public static string OidcAuthority = ConfigurationManager.AppSettings["oidc:Authority"];
        public static string OidcRedirectUrl = ConfigurationManager.AppSettings["oidc:RedirectUrl"];
        public static string OidcClientId = ConfigurationManager.AppSettings["oidc:ClientId"];
        public static string OidcClientSecret = ConfigurationManager.AppSettings["oidc:ClientSecret"];
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                Authority = OidcAuthority,
                ClientId = OidcClientId,
                ClientSecret = OidcClientSecret,
                GetClaimsFromUserInfoEndpoint = true,
                PostLogoutRedirectUri = OidcRedirectUrl,
                RedirectUri = OidcRedirectUrl,
                ResponseType = OpenIdConnectResponseType.Code,
                Scope = "openid email profile",
                SignInAsAuthenticationType = "Cookies",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = context =>
                    {
                        // Retrieve user claims from the ID token
                        var idToken = context.ProtocolMessage.IdToken;
                        var claims = context.AuthenticationTicket.Identity.Claims;

                        return Task.FromResult(0);
                    }
                }
            };
            app.UseOpenIdConnectAuthentication(oidcOptions);
        }
    }
}