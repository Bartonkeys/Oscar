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

namespace Oscar.Infrastructure.Features.Catalogue.Commands
{
    public class UpdateCatalogueCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public CatalogueUpdateDto CatalogueUpdateDto { get; set; }
    }

    public class UpdateCatalogueCommandHandler : AbstractBaseHandler<UpdateCatalogueCommand, string>
    {
        public UpdateCatalogueCommandHandler(OscarContext oscarContext, IMapper mapper,
            IValidator<UpdateCatalogueCommand> validator, ILogger<UpdateCatalogueCommand> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(UpdateCatalogueCommand request, CancellationToken cancellationToken)
        {
            var catalogue = await OscarContext.Catalogues
                //.Include(c => c.Societies)
                .Include(c => c.Rights)
                .Include(c => c.OtherNames)
                .Include(c => c.Mandates).ThenInclude(c => c.MandateType)
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (catalogue == null)
            {
                Logger.LogInformation((int)CatalogueFeatureEvent.UpdateNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            catalogue.Id = request.Id;
            catalogue.Name = request.CatalogueUpdateDto.Name;
            catalogue.IMaestroClientCode = request.CatalogueUpdateDto.IMaestroClientCode;
            catalogue.AgicoaClientRef = request.CatalogueUpdateDto.AgicoaClientRef;
            catalogue.GeneralNotes = request.CatalogueUpdateDto.GeneralNotes;

            SetOtherNames(request, catalogue);
            SetMandates(request, catalogue);

            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)CatalogueFeatureEvent.Update, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        private void SetOtherNames(UpdateCatalogueCommand request, Core.Entities.Catalogue? catalogue)
        {
            if (catalogue.OtherNames == null) catalogue.OtherNames = new HashSet<Core.Entities.OtherName>();

            foreach (var record in catalogue.OtherNames)
            {
                if (!request.CatalogueUpdateDto.OtherNames.Any(a => a.Id == record.Id))
                {
                    catalogue.OtherNames.Remove(record);
                    OscarContext.OtherName.Remove(record);
                }
            }

            if (request.CatalogueUpdateDto.OtherNames != null)
            {
                foreach (OtherNameDto updateOtherName in request.CatalogueUpdateDto.OtherNames)
                {

                    var newOtherName = Mapper.Map<OtherName>(updateOtherName);

                    if (!OscarContext.OtherName.Any(x => x.Id == updateOtherName.Id))
                    {
                        newOtherName.Id = 0;
                        if (updateOtherName.ClientId != null && updateOtherName.ClientId != -1)
                        {
                            newOtherName.Client = OscarContext.Clients.First(x => x.Id == updateOtherName.ClientId);
                            newOtherName.Catalogue = null;
                        }
                        else if (updateOtherName.CatalogueId != null && updateOtherName.CatalogueId != -1)
                        {
                            newOtherName.Catalogue = OscarContext.Catalogues.First(x => x.Id == updateOtherName.CatalogueId);
                            newOtherName.Client = null;
                        }

                        OscarContext.OtherName.Add(newOtherName);
                        catalogue.OtherNames.Add(newOtherName);
                    }
                }
            }
        }

        private void SetMandates(UpdateCatalogueCommand request, Core.Entities.Catalogue catalogue)
        {
            foreach (var rec in request.CatalogueUpdateDto.MandateTypes)
            {
                var toUpdate = catalogue.Mandates.FirstOrDefault(x => x.MandateType.Id == rec.Id);
                if (toUpdate != null)
                {
                    toUpdate.Mandated = rec.Mandated;
                    toUpdate.Client = OscarContext.Clients.First(x => x.Id == request.CatalogueUpdateDto.ClientId);
                    toUpdate.Catalogue = OscarContext.Catalogues.First(x => x.Id == request.Id);
                }
                else
                {
                    var newRecord = new Mandate();
                    newRecord.MandateType = OscarContext.MandateType.First(x => x.Id == rec.Id);
                    newRecord.Client = OscarContext.Clients.First(x => x.Id == request.CatalogueUpdateDto.ClientId);
                    newRecord.Catalogue = OscarContext.Catalogues.First(x => x.Id == request.Id);
                    newRecord.Mandated = rec.Mandated;
                    catalogue.Mandates.Add(newRecord);
                }
            }
        }

    }
}

