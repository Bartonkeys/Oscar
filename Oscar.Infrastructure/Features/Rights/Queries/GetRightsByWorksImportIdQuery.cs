using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Queries
{
    public class GetRightsByWorksImportIdQuery: IRequest<Result<List<RightDto>>>
    {
        public int WorksImportId { get; set; }
    }

    public class GetRightsByWorksImportIdHandler : AbstractBaseHandler<GetRightsByWorksImportIdQuery, List<RightDto>>
    {
        public GetRightsByWorksImportIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRightsByWorksImportIdQuery> validator, ILogger<GetRightsByWorksImportIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<RightDto>>> HandleRequest(GetRightsByWorksImportIdQuery request, CancellationToken cancellationToken)
        {
            var worksRightsImports = await OscarContext
                .WorksRightsImports
                .AsNoTracking()
                .Where(x => x.WorksImport.Id == request.WorksImportId)
                .ToListAsync(cancellationToken: cancellationToken);

            var rightsDto = new List<RightDto>();
            if (worksRightsImports?.Count > 0)
            {
                foreach (var worksRightImport in worksRightsImports)
                {
                    rightsDto.Add(new RightDto()
                    {
                        Type = new RightsTypeDto { Name = worksRightImport.TypeName},
                        Countries = new List<CountryDto>() { new CountryDto { Code = worksRightImport.CountryCode} },
                        LanguageRights= new List<LanguageRightsDto>() { new LanguageRightsDto { Language = new LanguageDto { Name = worksRightImport.LanguageName } } },
                        ChannelRights = new List<ChannelRightsDto>() { new ChannelRightsDto { Channel = new ChannelDto { Name = worksRightImport.ChannelName } } },
                        StartOfRight = worksRightImport.StartOfRight,
                        EndOfRight = worksRightImport.EndOfRight,
                        StartOfValidity= worksRightImport.StartOfValidity,
                        EndOfValidity= worksRightImport.EndOfValidity,
                        Percentage = worksRightImport.Percentage
                    });
                }
            }

            return Result.Ok(rightsDto);
        }
    }

}
