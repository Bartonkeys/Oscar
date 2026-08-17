using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Features.Common;

namespace Oscar.Mrit.Features.MRITIntegration.Commands
{
    public class UpdateMatchStatusCommand : IRequest<Result>
    {
        public IEnumerable<MatchStatusDto> Statuses { get; set; }
    }

    public class UpdateMatchStatusCommandHandler : SimpleAbstractBaseHandler<UpdateMatchStatusCommand>
    {
        public UpdateMatchStatusCommandHandler(OscarContext dbContext, IMapper mapper,
            IValidator<UpdateMatchStatusCommand> validator, ILogger<UpdateMatchStatusCommand> logger) : base(dbContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(UpdateMatchStatusCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.ToString());

            var today = DateTime.Now;

            var onMusicMatches = new List<OnMusicMatch>();
            foreach (var status in request.Statuses)
                OscarContext.OnMusicMatches.SingleOrDefault(m => m.WorksId == status.WorksId)
                    .ToMaybe()
                    .Match(s =>
                        {
                            s.OnMusicMatchStatusId = (int) status.MatchStatus;
                            s.Message = status.Message;
                            s.DateModified = today;
                        },
                        () =>
                        {
                            var onMusicMatch = new OnMusicMatch
                            {
                                WorksId = status.WorksId,
                                OnMusicMatchStatusId = (int) status.MatchStatus,
                                Message = status.Message,
                                DateCreated = today,
                                DateModified = today
                            };
                            onMusicMatches.Add(onMusicMatch);
                        });

            await OscarContext.AddRangeAsync(onMusicMatches, cancellationToken);
            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}