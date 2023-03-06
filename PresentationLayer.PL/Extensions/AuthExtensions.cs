using Microsoft.Identity.Web;

namespace PresentationLayer.PL.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthenticationWithAuthorizationSupport(this IServiceCollection services, IConfiguration config)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(config, "AzureAdB2C");

            return services;
        }
    }
}
