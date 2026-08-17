using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using System.Text;
using System.Text.Json;
using static System.Int32;


namespace Oscar.Infrastructure.Features.Common
{
    public abstract class AbstractBaseHandler<T, TR> : IRequestHandler<T, Result<TR>> where T : IRequest<Result<TR>>
    {
        protected readonly OscarContext OscarContext;
        protected readonly IValidator<T> Validator;
        protected readonly ILogger<T> Logger;
        protected readonly IMapper Mapper;

        protected AbstractBaseHandler(OscarContext oscarContext, IMapper mapper, IValidator<T> validator, ILogger<T> logger)
        {
            Mapper = mapper;
            OscarContext = oscarContext;
            Validator = validator;
            Logger = logger;
        }

        public async Task<Result<TR>> Handle(T request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                Logger.LogInformation((int)FeatureEvent.ValidationFail, validationResult.ToString());
                return Result.Fail<TR>(validationResult.ToString());
            }
                
            return await HandleRequest(request, cancellationToken);
        }


        protected abstract Task<Result<TR>> HandleRequest(T request, CancellationToken cancellationToken);

        protected string? AutoGenerateCompactRef(int counter = 1)
        {
            var nextCompactRefString = OscarContext.Works.Select(w => w.CompactRef).Max();

            if (!TryParse(nextCompactRefString, out var nextCompactRef)) return string.Empty;

            nextCompactRef += counter;
            var result = nextCompactRef.ToString("D8");
            return result;
        }

        protected virtual IEnumerable<TR>? GetCachedDataDeserialize<TR>(byte[] dataFromCache)
        {
            if ((dataFromCache?.Length ?? 0) == 0) return null;
            try
            {
                var dataAsString = Encoding.UTF8.GetString(dataFromCache);
                return JsonSerializer.Deserialize<IEnumerable<TR>?>(dataAsString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}] {GetType().Name}.GetCachedDataDeserialize() => {e}");
                return null;
            }
        }
        
    }

    public abstract class SimpleAbstractBaseHandler<T> : IRequestHandler<T, Result> where T : IRequest<Result>
    {
        protected readonly OscarContext OscarContext;
        protected readonly IValidator<T> Validator;
        protected readonly ILogger<T> Logger;
        protected readonly IMapper Mapper;

        protected SimpleAbstractBaseHandler(OscarContext oscarContext, IMapper mapper, IValidator<T> validator, ILogger<T> logger)
        {
            Mapper = mapper;
            OscarContext = oscarContext;
            Validator = validator;
            Logger = logger;
        }

        public async Task<Result> Handle(T request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                Logger.LogInformation((int)FeatureEvent.ValidationFail, validationResult.ToString());
                return Result.Fail(validationResult.ToString());
            }

            return await HandleRequest(request, cancellationToken);
        }


        protected abstract Task<Result> HandleRequest(T request, CancellationToken cancellationToken);

        protected ICollection<TR> MapCollection<T, TR>(ICollection<T> collection, ICollection<TR>? destination)
            where T : IDto
            where TR : BaseEntity
        {
            foreach (var dto in collection)
            {
                if (dto.Id == 0)
                {
                    var entity = Mapper.Map<TR>(dto);
                    destination ??= new List<TR>();
                    destination.Add(entity);
                }
                else
                {
                    var entity = destination.SingleOrDefault(a => a.Id == dto.Id);
                    if (entity == null)
                    {
                        entity = OscarContext.Find<TR>(dto.Id);
                        if (entity != null) destination.Add(entity);
                    }
                    Mapper.Map(dto, entity);
                }
            }

            var toRemove = destination.Where(entity => collection.All(c => c.Id != entity.Id)).ToList();
            foreach (var entity in toRemove)
            {
                destination.Remove(entity);
            }

            return destination;
        }

    }
}
