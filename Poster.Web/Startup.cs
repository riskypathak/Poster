using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Poster.Web.Startup))]
namespace Poster.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
