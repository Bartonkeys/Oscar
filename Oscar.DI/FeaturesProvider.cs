using System.Reflection;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Providers;
using Oscar.Infrastructure.Behaviours;
using Oscar.Infrastructure.Features.Actor.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Clients.Validation;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Common.Services;
using Oscar.Infrastructure.Features.Matching.Contracts;
using Oscar.Infrastructure.Features.Matching.Services;
using Oscar.Infrastructure.Features.Person.Validation;
using Oscar.Infrastructure.Features.WorksImport.Services;
using Oscar.Infrastructure.Features.Registration.Contracts;
using Oscar.Infrastructure.Features.Registration.Services;
using Oscar.Infrastructure.Providers;
using static Oscar.Infrastructure.Behaviours.ExceptionBehaviour;

namespace Oscar.DI
{
    public static class FeaturesProvider
    {
        public static void ConfigureFeatures(this IServiceCollection services, Assembly assembly)
        {
            services.AddAutoMapper(typeof(AbstractBaseHandler<,>).GetTypeInfo().Assembly);
            services.AddMediatR(assembly, typeof(GetClientsQuery).GetTypeInfo().Assembly);
            services.AddValidatorsFromAssemblyContaining<GetClientsQueryValidation>(ServiceLifetime.Transient);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddTransient<IImporter, ImportService>();
            services.AddTransient<IExporter, ExportService>();
            services.AddTransient<IContainerService, ContainerService>();
            services.AddTransient<IQueueService, QueueService>();
            services.AddTransient<IMatchingService, MatchingService>();
            services.AddTransient<IDynamicExpressionBuilderService, DynamicExpressionBuilderService>();
            services.AddTransient<IWorksImportService, WorksImportService>();
            services.AddTransient(typeof(IRequestHandler<AddPersonCommand<Actor>, Result<PersonDto>>), typeof(AddPersonCommandHandler<Actor>));
            services.AddTransient(typeof(IRequestHandler<AddPersonCommand<Director>, Result<PersonDto>>), typeof(AddPersonCommandHandler<Director>));
            services.AddTransient(typeof(IRequestHandler<AddPersonCommand<Producer>, Result<PersonDto>>), typeof(AddPersonCommandHandler<Producer>));
            services.AddTransient(typeof(IRequestHandler<AddPersonCommand<ScreenWriter>, Result<PersonDto>>), typeof(AddPersonCommandHandler<ScreenWriter>));
            services.AddTransient(typeof(IRequestHandler<AddPersonCommand<ScriptWriter>, Result<PersonDto>>), typeof(AddPersonCommandHandler<ScriptWriter>));
            services.AddTransient(typeof(IRequestHandler<AddPersonCommand<Distributor>, Result<PersonDto>>), typeof(AddPersonCommandHandler<Distributor>));
            services.AddTransient(typeof(IValidator<AddPersonCommand<Actor>>), typeof(AddPersonCommandValidator<Actor>));
            services.AddTransient(typeof(IValidator<AddPersonCommand<Director>>), typeof(AddPersonCommandValidator<Director>));
            services.AddTransient(typeof(IValidator<AddPersonCommand<Producer>>), typeof(AddPersonCommandValidator<Producer>));
            services.AddTransient(typeof(IValidator<AddPersonCommand<ScreenWriter>>), typeof(AddPersonCommandValidator<ScreenWriter>));
            services.AddTransient(typeof(IValidator<AddPersonCommand<ScriptWriter>>), typeof(AddPersonCommandValidator<ScriptWriter>));
            services.AddTransient(typeof(IValidator<AddPersonCommand<Distributor>>), typeof(AddPersonCommandValidator<Distributor>));
            services.AddTransient<IRegistrationService<RegistrationWorksAgicoaExport>, AgicoaRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksSuissImageExport>, SuissImageRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksScreenrightsExport>, ScreenrightsRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksCCCDto>, CCCRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksCMCDto>, CMCRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksMPLCDto>, MPLCRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksCRCDto>, CRCRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksEGEDADto>, EGEDARegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksGWFFDto>, GWFFRegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksMPADto>, MPARegistrationService>();
            services.AddTransient<IRegistrationService<RegistrationWorksUpfarArgoaDto>, UpfarArgoaRegistrationService>();
            services.AddTransient<IBrowserDownload, BrowserDownloadService>();
            services.AddSingleton<IUserProvider, UserProvider>();
            services.AddTransient<ICacheService, CacheService>();
        }
    }
}
