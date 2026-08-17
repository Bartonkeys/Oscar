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

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class GetWorksStaticDataQuery : IRequest<Result<IEnumerable<EnumDTO>>>
    {
        public Enums? EnumName { get; set; }

        public GetWorksStaticDataQuery()
        {

        }

        public GetWorksStaticDataQuery(Enums enumName)
        {
            this.EnumName = enumName;
        }
    }

    public class WorksStaticDataHandler : AbstractBaseHandler<GetWorksStaticDataQuery, IEnumerable<EnumDTO>>
    {
        public WorksStaticDataHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksStaticDataQuery> validator, ILogger<GetWorksStaticDataQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<EnumDTO>>> HandleRequest(GetWorksStaticDataQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<EnumDTO> enumDTOs = request.EnumName switch
            {
                Enums.WorksStatus => Enum<WorksStatus>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d)),
                _ => Enum<Enums>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d)),
            };
            Logger.LogInformation((int)StaticDataFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(enumDTOs);
        }

    }
}
