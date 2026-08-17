using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Configuration;

namespace Oscar.Infrastructure.Features.ProductionCompany.Commands
{
    public class AddCompanyCommand : IRequest<Result<CompanyDto>>
    {
        public CompanyAddDto CompanyAddDto { get; set; }
    }

    public class AddCompanyCommandHandler : AbstractBaseHandler<AddCompanyCommand, CompanyDto>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public AddCompanyCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddCompanyCommand> validator, ILogger<AddCompanyCommand> logger, IConfiguration configuration, ICacheService cache) : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<CompanyDto>> HandleRequest(AddCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = Mapper.Map<Core.Entities.Company>(request.CompanyAddDto);
            OscarContext.Add(company);
            await OscarContext.SaveChangesAsync(cancellationToken);

            if (bool.Parse(_config["UseCache"]) == true)
            { _cache.InvalidateCacheForEntity(company); }

            Logger.LogInformation((int)CompanyFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<CompanyDto>(company));
        }
    }
}
