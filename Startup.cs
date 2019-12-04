using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HRworks.Startup))]
namespace HRworks
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
