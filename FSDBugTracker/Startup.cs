using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FSDBugTracker.Startup))]
namespace FSDBugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
