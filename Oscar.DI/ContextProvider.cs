using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oscar.Data.Context;

namespace Oscar.DI
{
    public static class ContextProvider
    {
        public static void UseSqlServer(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            services.AddDbContext<OscarContext>(options 
                => options.UseSqlServer(configuration.GetConnectionString("OscarConnection"), providerOptions => providerOptions.EnableRetryOnFailure()), serviceLifetime);
        }
    }
}
