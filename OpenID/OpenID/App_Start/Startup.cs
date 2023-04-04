using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(OpenID.Startup))]

namespace OpenID
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IdentityModelEventSource.ShowPII = true; //Add this line

            ConfigureAuth(app);
        }
    }
}