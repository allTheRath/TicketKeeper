using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketKeeper.Startup))]
namespace TicketKeeper
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
