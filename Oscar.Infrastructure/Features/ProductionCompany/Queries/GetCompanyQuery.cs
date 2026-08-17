using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.ProductionCompany.Queries
{
    public class GetCompanyQuery : BaseTableQuery, IRequest<Result<IEntityTable<CompanyDto>>>
    {
        public int Id { get; set; }
    }

    public class GetCompanyQueryHandler : AbstractBaseHandler<GetCompanyQuery, IEntityTable<CompanyDto>>
    {
        public GetCompanyQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCompanyQuery> validator, ILogger<GetCompanyQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<CompanyDto>>> HandleRequest(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)CompanyFeatureEvent.Get, CommandResult.SUCCESS);

            var companies = OscarContext.Companies;
            var total = companies.Count();

            return Result.Ok(EntityTable<CompanyDto>.Create(companies.Select(c => Mapper.Map<CompanyDto>(c))).WithTotal(total));
        }
        
    }
}
