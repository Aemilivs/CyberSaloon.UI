using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CyberSaloon.Server.Areas.Identity.IdentityHostingStartup))]
namespace CyberSaloon.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}