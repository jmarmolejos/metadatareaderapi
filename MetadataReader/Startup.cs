using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MetadataReader.Startup))]
namespace MetadataReader
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}