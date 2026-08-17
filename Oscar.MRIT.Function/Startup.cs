using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oscar.Core.Extensions;
using Oscar.Core.Providers;
using Oscar.DI;
using Oscar.Infrastructure.Providers;
using Oscar.Mrit.Features.MRITIntegration.Commands;
using Oscar.Mrit.Features.MRITIntegration.Common;
using Oscar.MRIT.Core.Configuration;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Function;
using UserProvider = Oscar.Mrit.Features.Common.UserProvider;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Oscar.MRIT.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.UseSqlServer(configuration, ServiceLifetime.Scoped);
            builder.Services.AddSingleton<Oscar.Mrit.Features.Common.IUserProvider, UserProvider>();
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddAutoMapper(typeof(UpdateMatchStatusCommand).GetTypeInfo().Assembly);
            builder.Services.AddMediatR(typeof(UpdateMatchStatusCommand).GetTypeInfo().Assembly);
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateMatchStatusCommand>(ServiceLifetime.Transient);
            builder.Services.AddScoped<MritMapperFactory>();

            builder.Services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(configuration["oscarstorage"], preferMsi: true);
                clientBuilder.AddQueueServiceClient(configuration["oscarstorage"], preferMsi: true);
            });

            builder.Services.AddHttpClient("mrit", c =>
            {
                c.BaseAddress = new Uri(configuration["MritApi"]);
                c.Timeout = TimeSpan.FromMinutes(10);
                var theKey = configuration.GetValue<string>("MritApiKey");
               c.DefaultRequestHeaders.Add("ApiKey", configuration.GetValue<string>("MritApiKey"));
            });

            builder.Services.Configure<BlackListDto>(configuration.GetSection("Blacklist"));
            builder.Services.Configure<BatchSettings>(configuration.GetSection("BatchSettings"));
        }
    }
}