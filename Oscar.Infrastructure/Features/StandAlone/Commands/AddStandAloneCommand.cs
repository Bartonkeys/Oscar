using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.StandAlone.Commands
{
    public class AddStandAloneCommand: IRequest<Result<StandAloneDto>>
    {
        public StandAloneAddDto StandAloneAddDto { get; set; }
    }

    public class AddStandAloneCommandHandler : AbstractBaseHandler<AddStandAloneCommand, StandAloneDto>
    {
        public AddStandAloneCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddStandAloneCommand> validator, ILogger<AddStandAloneCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<StandAloneDto>> HandleRequest(AddStandAloneCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.StandAloneAddDto.Titles.Where(t => t.Id < 0))
                item.Id = 0;

            var standAlone = Mapper.Map<Core.Entities.StandAlone>(request.StandAloneAddDto);
            standAlone.Countries = new List<Core.Entities.Country>();
            standAlone.Directors = new List<Core.Entities.Director>();
            standAlone.Companies = new List<Core.Entities.Company>();
            standAlone.Clients = new List<Core.Entities.Client>();
            standAlone.Catalogues = new List<Core.Entities.Catalogue>();
            standAlone.Mandates = new List<Core.Entities.Mandate>();
            WorksHelper.SetCollection(standAlone.Countries, request.StandAloneAddDto.CountryIds, OscarContext);
            WorksHelper.SetCollection(standAlone.Directors, request.StandAloneAddDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection(standAlone.Languages, request.StandAloneAddDto.LanguageIds, OscarContext);
            WorksHelper.SetCollection(standAlone.Companies, request.StandAloneAddDto.CompanyIds, OscarContext);
            WorksHelper.SetCollection(standAlone.Clients, request.StandAloneAddDto.ClientIds, OscarContext);
            WorksHelper.SetCollection(standAlone.Catalogues, request.StandAloneAddDto.CatalogueIds, OscarContext);
            WorksHelper.SetMandates(standAlone.Mandates, request.StandAloneAddDto.MandateTypes, OscarContext);

            standAlone.CompactRef = AutoGenerateCompactRef();

            standAlone.ClientReferences = new List<ClientReference>() { new ClientReference() };

            OscarContext.Add(standAlone);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)StandAloneFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<StandAloneDto>(standAlone));
        }
    }
}
