using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oscar.Data.Context;
using Oscar.MRIT.Client.Client;
using Oscar.MRIT.Core.Configuration;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Data;
using Oscar.Mrit.Features.Common;
using Oscar.Mrit.Features.MRITIntegration.Commands;
using Oscar.Mrit.Features.MRITIntegration.Common;

namespace Oscar.MRIT.Client.DependancyInjection
{
    public static class DependancyInjection
    {
        public static void ConfigureOscarMritClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OscarContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("OscarConnection"), providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddDbContext<FelixMritContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("FelixMrit")));
            services.AddSingleton<Oscar.Mrit.Features.Common.IUserProvider, UserProvider>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(typeof(UpdateMatchStatusCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(OscarMritClient).Assembly, typeof(UpdateMatchStatusCommand).Assembly);
            services.AddValidatorsFromAssemblyContaining<UpdateMatchStatusCommand>(ServiceLifetime.Transient);
            services.AddScoped<MritMapperFactory>();
            services.AddScoped<IOscarMritClient, OscarMritClient>();

            services.Configure<BlackListDto>(configuration.GetSection("Blacklist"));
            services.Configure<BatchSettings>(configuration.GetSection("BatchSettings"));
        }
    }
}
