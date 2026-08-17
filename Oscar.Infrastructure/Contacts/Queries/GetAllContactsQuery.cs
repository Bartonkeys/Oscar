using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Actor.Queries;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Contacts.Queries
{
    public class GetAllContactsQuery : IRequest<Result<IEnumerable<ContactDto>>>
    {
    }

    public class GetAllContactsQuerysHandler : AbstractBaseHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
    {
        public GetAllContactsQuerysHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllContactsQuery> validator,
            ILogger<GetAllContactsQuery> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<ContactDto>>> HandleRequest(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var contacts = OscarContext.Contacts.AsNoTracking().ToList();

            Logger.LogInformation((int)ContactFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(contacts.Select(a => Mapper.Map<ContactDto>(a)));
        }

    }
}
