using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(KindnessWall.Startup))]

namespace KindnessWall
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
