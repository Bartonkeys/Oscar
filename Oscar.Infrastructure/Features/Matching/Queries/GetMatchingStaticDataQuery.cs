using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using System.Linq.Expressions;


namespace Oscar.Infrastructure.Features.Matching.Queries
{
    public class GetMatchingStaticDataQuery : IRequest<Result<IEnumerable<EnumDTO>>>
    {
        public Enums? EnumName { get; set; }

        public GetMatchingStaticDataQuery()
        {

        }

        public GetMatchingStaticDataQuery(Enums enumName)
        {
            this.EnumName = enumName;
        }
    }

    public class MatchingStaticDataHandler : AbstractBaseHandler<GetMatchingStaticDataQuery, IEnumerable<EnumDTO>>
    {
        public MatchingStaticDataHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetMatchingStaticDataQuery> validator, ILogger<GetMatchingStaticDataQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<EnumDTO>>> HandleRequest(GetMatchingStaticDataQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EnumDTO> enumDTOs;

            switch (request.EnumName)
            {
                case Enums.MatchingRequestStatus:
                    enumDTOs = Enum<MatchRequestStatus>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));
                    break;

                case Enums.MatchRules:
                    enumDTOs = Enum<MatchRules>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));
                    break;

                default:
                    enumDTOs = Enum<Enums>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));
                    break;
            }


        Logger.LogInformation((int)StaticDataFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(enumDTOs);
        }

    }
}
