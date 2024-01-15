using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BinusZoom.Startup))]
namespace BinusZoom
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
