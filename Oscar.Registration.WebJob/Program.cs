using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.Azure;
using Oscar.DI;

namespace Oscar.Registration.WebJob
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.UseSqlServer(configuration);
                services.ConfigureFeatures(Assembly.GetExecutingAssembly());
                services.AddAzureClients(clientBuilder =>
                {
                    clientBuilder.AddBlobServiceClient(configuration["AzureWebJobsStorage"]);
                    clientBuilder.AddQueueServiceClient(configuration["AzureWebJobsStorage"]);
                });
            });
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorageQueues();
            });
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
