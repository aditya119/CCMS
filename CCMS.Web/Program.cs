using Blazored.LocalStorage;
using Blazored.Toast;
using CCMS.Client;
using CCMS.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CCMS.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddBlazoredToast();

            services.AddAuthorizationCore();

            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICaseIdHandler, CaseIdHandler>();

            services.AddScoped<ConfigClient>();
            services.AddScoped<AttachmentClient>();
            services.AddScoped<AuthClient>();
            services.AddScoped<IdentityClient>();
            services.AddScoped<FactoryDataClient>();
            services.AddScoped<AppUserClient>();
            services.AddScoped<CaseClient>();
        }
    }
}
