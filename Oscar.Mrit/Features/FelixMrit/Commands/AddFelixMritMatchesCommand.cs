using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.MRIT.Core.Configuration;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Data;
using FluentValidation;

namespace Oscar.Mrit.Features.FelixMrit.Commands
{
    public class AddFelixMritMatchesCommand : IRequest<Result>
    {
        public IList<FelixMritMatchDto> Matches { get; set; }
    }

    public class AddFelixMritMatchesCommandHandler : IRequestHandler<AddFelixMritMatchesCommand, Result>
    {
        private readonly BatchSettings _batchSettings;
        private readonly ILogger<AddFelixMritMatchesCommand> _logger;
        private readonly FelixMritContext _dbContext;
        private readonly IValidator<AddFelixMritMatchesCommand> _validator;
        private readonly IMapper _mapper;

        public AddFelixMritMatchesCommandHandler(FelixMritContext dbContext, IMapper mapper, IValidator<AddFelixMritMatchesCommand> validator, ILogger<AddFelixMritMatchesCommand> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result> Handle(AddFelixMritMatchesCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail(validationResult.ToString());

            _logger.LogInformation($"Start processing matches");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var felixMritMatch in request.Matches)
            {
                _dbContext
                    .Matches
                    .Include(b => b.BatchJob)
                    .Include(t => t.Transmissions)
                    .SingleOrDefault(m => m.RecordId == felixMritMatch.RecordId && m.BatchJob.BatchJobKey == felixMritMatch.BatchJobKey)
                    .ToMaybe()
                    .Match(m =>
                    {
                        _logger.LogInformation($"Process transmissions for existing match {m.Id} with title {m.ProductionTitle}");
                        var transmissions = _mapper.Map<ICollection<Transmission>>(felixMritMatch.Transmissions);
                        // m.Transmissions.ToList().AddRange(transmissions);
                        foreach (var tx in transmissions)
                            m.Transmissions.Add(tx);
                    }, async () =>
                    {
                        _logger.LogInformation($"Create new match for title {felixMritMatch.ProductionTitle}");
                        var match = _mapper.Map<Match>(felixMritMatch);
                        await _dbContext.Matches.AddAsync(match, cancellationToken);
                    });

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            watch.Stop();
            _logger.LogInformation($"Processed matches in {watch.ElapsedMilliseconds} milliseconds");

            return Result.Ok();
        }

    }
}
