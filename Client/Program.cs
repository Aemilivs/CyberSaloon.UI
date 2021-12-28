using CyberSaloon.Client.Pages.Common;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using RazorComponentsPreview;
using Microsoft.Extensions.Configuration;
using System.IO;
using CyberSaloon.Client.Helpers;

namespace CyberSaloon.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var scope = "CyberSaloon.ServerAPI";
            var serverUrl = builder.Configuration.GetValue<string>("CyberSaloon.ServerAPI.Url");
            var clientUrl = builder.Configuration.GetValue<string>("CyberSaloon.ClientAPI.Url");
            builder
                .Services
                .AddHttpClient(
                        "CyberSaloon.CoreAPI", 
                        it => it.BaseAddress = new Uri(clientUrl)
                    )
                .AddHttpMessageHandler(
                    it =>
                        it
                            .GetService<AuthorizationMessageHandler>()
                            .ConfigureHandler(
                                authorizedUrls: new[] { clientUrl },
                                scopes: new[] { scope }
                            )
                );

            builder
                .Services
                .AddHttpClient(
                        "CyberSaloon.CoreAPI.Anonymous",
                        it => it.BaseAddress = new Uri(clientUrl)
                    );

            builder
                .Services
                .AddHttpClient(
                        "CyberSaloon.ServerAPI", 
                        it => it.BaseAddress = new Uri(serverUrl)
                    )
                .AddHttpMessageHandler(
                    it =>
                        it
                            .GetService<AuthorizationMessageHandler>()
                            .ConfigureHandler(
                                authorizedUrls: new[] { serverUrl },
                                scopes: new[] { scope }
                            )
                );

            builder
                .Services
                .AddHttpClient(
                        "CyberSaloon.ServerAPI.Anonymous",
                        it => it.BaseAddress = new Uri(serverUrl)
                    );

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(it => it.GetService<IHttpClientFactory>().CreateClient(scope));
            builder.Services.AddTransient<ICoreAPIClient, CoreAPIClient>();
            builder.Services.AddTransient<IIdentityServerAPIClient, IdentityServerAPIClient>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            builder.Services.AddScoped<LocalStorage>();
            builder.Services.AddScoped<SessionStorage>();
            builder.Services.AddProtectedBrowserStorage();
            builder.Services.AddRazorComponentsRuntimeCompilation();
            
            builder.Services.AddApiAuthorization();
            
            var application = builder.Build();
            
            await application.RunAsync();
        }
    }
}
