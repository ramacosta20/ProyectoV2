using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PaginaProyecto.Startup))]
namespace PaginaProyecto
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
