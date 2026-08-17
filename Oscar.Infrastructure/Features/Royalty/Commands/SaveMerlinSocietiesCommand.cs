using AutoMapper;
using BartonKeys.Functional;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class SaveMerlinSocietiesCommand : IRequest<Result<List<MerlinSocietyDto>>>
    {
        public List<MerlinSocietyDto> MerlinSocieties { get; set; }
    }

    public class SaveMerlinSocietiesCommandHandler : AbstractBaseHandler<SaveMerlinSocietiesCommand, List<MerlinSocietyDto>>
    {
        public SaveMerlinSocietiesCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<SaveMerlinSocietiesCommand> validator, ILogger<SaveMerlinSocietiesCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<MerlinSocietyDto>>> HandleRequest(SaveMerlinSocietiesCommand request, CancellationToken cancellationToken)
        {
            //var merlinSocieties = Mapper.Map<List<MerlinSociety>>(request.MerlinSocieties);

            //delete differences
            var requestMerlinIds = request.MerlinSocieties.Select(m => m.MerlinId).ToHashSet();
            var existingMerlinSocieties = OscarContext.MerlinSocieties.ToList();
            var entitiesToDelete = existingMerlinSocieties.Where(m => !requestMerlinIds.Contains(m.MerlinId)).ToList();
            if (entitiesToDelete.Any())
            {
                OscarContext.MerlinSocieties.RemoveRange(entitiesToDelete);
            }

            //add or update
            foreach (var merlinSocietyDto in request.MerlinSocieties)
            {
                var existingEntity = OscarContext.MerlinSocieties.FirstOrDefault(m => m.MerlinId == merlinSocietyDto.MerlinId);
                if (existingEntity == null)
                {
                    var newEntity = Mapper.Map<MerlinSociety>(merlinSocietyDto);
                    OscarContext.MerlinSocieties.Add(newEntity);
                }
                else
                {
                    Mapper.Map(merlinSocietyDto, existingEntity);
                }
            }

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(Mapper.Map<List<MerlinSocietyDto>>(OscarContext.MerlinSocieties.ToList()));
        }
    }

}
