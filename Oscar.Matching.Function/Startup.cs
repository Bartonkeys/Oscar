using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Oscar.Core.Extensions;
using Oscar.DI;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(Oscar.Function.Startup))]
namespace Oscar.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.UseSqlServer(configuration);
            builder.Services.ConfigureFeatures(Assembly.GetExecutingAssembly());

            builder.Services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(configuration["oscarstorage"], preferMsi: true);
                clientBuilder.AddQueueServiceClient(configuration["oscarstorage"], preferMsi: true);
            });
        }
    }
}