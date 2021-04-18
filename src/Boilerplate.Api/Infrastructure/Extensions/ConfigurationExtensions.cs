using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Api.Infrastructure.Extensions
{

    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureIOptions(this IServiceCollection services, IConfiguration configuration){
            services.Configure<Appsettings>(configuration);
            services.Configure<SendGridSettings>(configuration.GetSection(nameof(Appsettings.SendGrid)));
            services.Configure<AuthorizationSettings>(configuration.GetSection(nameof(Appsettings.Authorization)));
            services.Configure<RentalSettings>(configuration.GetSection(nameof(Appsettings.Rental)));
            return services;
        }
    }
}