using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Oscar.Core.Providers;
using Oscar.Data.Context;
using Oscar.DI;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.UnitTests;

public abstract class UnitTestBase
{
    protected readonly OscarContext OscarContext;
    protected readonly IMediator Mediator;

    protected UnitTestBase()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<OscarContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.ConfigureFeatures(Assembly.GetAssembly(typeof(GetWorksTitleQuery))!);
        services.AddDistributedMemoryCache();

        services.AddTransient<IConfiguration>(_ =>
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseCache"] = "false"
            });
            return configurationBuilder.Build();
        });

        var userProviderMock = new Mock<IUserProvider>();
        userProviderMock.Setup(u => u.GetUserName()).Returns("UNIT_TEST");
        userProviderMock.Setup(u => u.GetName()).Returns("UNIT_TEST");
        services.AddTransient(_ => userProviderMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        Mediator = serviceProvider.GetRequiredService<IMediator>();
        OscarContext = serviceProvider.GetRequiredService<OscarContext>();
    }
}
