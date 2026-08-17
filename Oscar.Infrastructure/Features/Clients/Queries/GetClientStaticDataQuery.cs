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


namespace Oscar.Infrastructure.Features.Clients.Queries
{
    public class GetClientStaticDataQuery : IRequest<Result<IEnumerable<EnumDTO>>>
    {
        public Enums? EnumName { get; set; }

        public GetClientStaticDataQuery()
        {

        }

        public GetClientStaticDataQuery(Enums enumName)
        {
            this.EnumName = enumName;
        }
    }

    public class ClientStaticDataHandler : AbstractBaseHandler<GetClientStaticDataQuery, IEnumerable<EnumDTO>>
    {
        public ClientStaticDataHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetClientStaticDataQuery> validator, ILogger<GetClientStaticDataQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<EnumDTO>>> HandleRequest(GetClientStaticDataQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EnumDTO> enumDTOs;

            switch (request.EnumName)
            {
                case Enums.ClientGrade:
                    enumDTOs = Enum<ClientGrade>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));
                    break;

                case Enums.Status:
                    enumDTOs = Enum<Status>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));
                    break;

                case Enums.ClientType:
                    enumDTOs = Enum<ClientType>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));
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
