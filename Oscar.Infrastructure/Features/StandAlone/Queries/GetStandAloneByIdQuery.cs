using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.StandAlone.Queries
{
    public class GetStandAloneByIdQuery: BaseTableQuery, IRequest<Result<StandAloneDto>>
    {
        public int Id { get; set; }
    }

    public class StandAloneByIdHandler : AbstractBaseHandler<GetStandAloneByIdQuery, StandAloneDto>
    {
        private readonly IConfiguration _config;

        public StandAloneByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetStandAloneByIdQuery> validator, ILogger<GetStandAloneByIdQuery> logger, IConfiguration config) : base(oscarContext, mapper, validator, logger)
        {
            _config = config;
        }

        protected override async Task<Result<StandAloneDto>> HandleRequest(GetStandAloneByIdQuery request, CancellationToken cancellationToken)
        {
            var standAlone = await OscarContext.StandAlones
                .AsNoTracking()
                .Include(i => i.Genre)
                .Include(i => i.Documents)
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .Include(i => i.Titles)
                .Include(i => i.Conflicts)
                .Include(i => i.WorksType)
                .Include(i => i.Countries)
                .Include(i => i.Companies)
                .Include(i => i.AlternativeTitles)
                .Include(i => i.Producers)
                .Include(i => i.Directors)
                .Include(i => i.Actors)
                .Include(i => i.Distributors)
                .Include(i => i.ScreenWriters)
                .Include(i => i.ScriptWriters)
                .Include(i => i.WorksStatusHistory)
                .Include(sr => sr.SocietyReferences)!.ThenInclude(s => s.Society)
                .Include(cr => cr.ClientReferences)!.ThenInclude(c => c.Client)
                .Include(l => l.Languages)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.Society)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.RegistrationBatch)
                .Include(i => i.ReRegistrations)!.ThenInclude(s => s.Society)
                .Include(i => i.Mandates).ThenInclude(i => i.MandateType)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken: cancellationToken);

            if (standAlone == null)
                return Result.Fail<StandAloneDto>("Not found");

            standAlone.Registrations = standAlone?.Registrations?.OrderByDescending(r => r.DateRegistered).GroupBy(r => r.Society?.Id).Select(x => x.First()).ToList();

            Logger.LogInformation((int)StandAloneFeatureEvent.Get, CommandResult.SUCCESS);

            var standAloneDto = Mapper.Map<StandAloneDto>(standAlone);
            foreach (var doc in standAloneDto.Documents)
            {
                doc.PublicUrl = _config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;
            }

            return Result.Ok(standAloneDto);
        }

    }
}
