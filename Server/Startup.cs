using CyberSaloon.Server.Data;
using CyberSaloon.Server.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Profiles;

namespace CyberSaloon.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(it => it.UseSqlServer(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services
                .AddDefaultIdentity<ApplicationUser>(it => it.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services
                .AddAuthentication()
                .AddIdentityServerJwt();

            var url = _configuration.GetValue<string>("CyberSaloon.ClientUI.Url");
            services.AddCors(options => 
                options
                    .AddPolicy(
                        "CyberSaloon.Client",
                        builder => 
                            builder
                                .WithOrigins(url)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .WithExposedHeaders("X-Pagination")
                    )
                );

            services.AddScoped<IProfileService, ProfileService>();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder builder, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                builder.UseDeveloperExceptionPage();
                builder.UseMigrationsEndPoint();
                builder.UseWebAssemblyDebugging();
            }
            else
            {
                builder.UseExceptionHandler("/Error");
                builder.UseHsts();
                builder.UseHttpsRedirection();
            }

            builder.UseBlazorFrameworkFiles();
            builder.UseStaticFiles();

            builder.UseRouting();
            builder.UseCors("CyberSaloon.Client");

            builder.UseIdentityServer();
            builder.UseAuthentication();
            builder.UseAuthorization();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers().RequireCors("CyberSaloon.Client");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
