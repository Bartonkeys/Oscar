using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Actor.Commands
{
    public class AddPersonCommand<T>: IRequest<Result<PersonDto>> where T: PersonEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AddPersonCommandHandler<T> : AbstractBaseHandler<AddPersonCommand<T>, PersonDto> where T : PersonEntity, new()
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public AddPersonCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddPersonCommand<T>> validator, ILogger<AddPersonCommand<T>> logger, IConfiguration configuration, ICacheService cache) : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<PersonDto>> HandleRequest(AddPersonCommand<T> request, CancellationToken cancellationToken)
        {
            var dbSet = OscarContext.Set<T>();
            
            var person = new T()
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var match = dbSet.FirstOrDefault(p => p.FirstName == person.FirstName && p.LastName == person.LastName);
            if (match != null)
            {
                return Result.Fail<PersonDto>("Already exists");
            }

            await dbSet.AddAsync(person);

            await OscarContext.SaveChangesAsync(cancellationToken);

            if (bool.Parse(_config["UseCache"]) == true)
            { _cache.InvalidateCacheForEntity(person); }

            Logger.LogInformation((int)ActorFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<PersonDto>(person));
        }
    }
}
