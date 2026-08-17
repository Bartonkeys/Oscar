using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BartonKeys.Extensions;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Logging;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.Mrit.Data;

namespace Oscar.Mrit.Features.FelixMrit.Mapping
{
    internal class PersonOfInterestResolver : IValueResolver<FelixMritMatchDto, Match, ICollection<PersonOfInterest>>
    {
        private readonly FelixMritContext _felixMritContext;
        private readonly ILogger<PersonOfInterestResolver> _logger;

        public PersonOfInterestResolver(FelixMritContext felixMritContext, ILogger<PersonOfInterestResolver> logger)
        {
            _felixMritContext = felixMritContext;
            _logger = logger;
        }

        public ICollection<PersonOfInterest> Resolve(FelixMritMatchDto source, Match destination, ICollection<PersonOfInterest> destMember,
            ResolutionContext context)
        {

            _logger.LogInformation($"Start resolving persons of interest");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var personOfInterestList = new List<PersonOfInterest>();
            personOfInterestList.AddRange(source.Actors.Select(a =>
                new PersonOfInterest
                {
                    Person = new Person
                    {
                        Forename = a.Forename,
                        MiddleNames = a.MiddleNames,
                        Surname = a.Surname,
                        SimpleName = (a.Forename + a.MiddleNames + a.Surname).SimplifyPersonName()
                    },
                    PersonType = _felixMritContext.PersonTypes.Find((int)PersonTypeEnum.Actor)
                }));

            personOfInterestList.AddRange(source.Directors.Select(a => new PersonOfInterest
            {
                Person = new Person
                {
                    Forename = a.Forename,
                    MiddleNames = a.MiddleNames,
                    Surname = a.Surname,
                    SimpleName = (a.Forename + a.MiddleNames + a.Surname).SimplifyPersonName()
                },
                PersonType = _felixMritContext.PersonTypes.Find((int)PersonTypeEnum.Director)
            }));

            personOfInterestList.AddRange(source.Producers.Select(a => new PersonOfInterest
            {
                Person = new Person
                {
                    Forename = a.Forename,
                    MiddleNames = a.MiddleNames,
                    Surname = a.Surname,
                    SimpleName = (a.Forename + a.MiddleNames + a.Surname).SimplifyPersonName()
                },
                PersonType = _felixMritContext.PersonTypes.Find((int)PersonTypeEnum.Producer)
            }));

            personOfInterestList.AddRange(source.Writers.Select(a => new PersonOfInterest
            {
                Person = new Person
                {
                    Forename = a.Forename,
                    MiddleNames = a.MiddleNames,
                    Surname = a.Surname,
                    SimpleName = (a.Forename + a.MiddleNames + a.Surname).SimplifyPersonName()
                },
                PersonType = _felixMritContext.PersonTypes.Find((int)PersonTypeEnum.Writer)
            }));

            personOfInterestList.AddRange(source.Creators.Select(a => new PersonOfInterest
            {
                Person = new Person
                {
                    Forename = a.Forename,
                    MiddleNames = a.MiddleNames,
                    Surname = a.Surname,
                    SimpleName = (a.Forename + a.MiddleNames + a.Surname).SimplifyPersonName()
                },
                PersonType = _felixMritContext.PersonTypes.Find((int)PersonTypeEnum.Creator)
            }));

            foreach (var personOfInterest in personOfInterestList)
            {
                _felixMritContext.People
                    .Cacheable()
                    .SingleOrDefault(p => p.SimpleName == personOfInterest.Person.SimpleName)
                    .ToMaybe()
                    .Match(p => personOfInterest.Person = p, () =>
                    {
                        _felixMritContext.People.Add(personOfInterest.Person);
                        _felixMritContext.SaveChanges();
                    });
            }

            watch.Stop();
            _logger.LogInformation($"Processed persons of interest in {watch.ElapsedMilliseconds} milliseconds");

            return personOfInterestList;
        }
    }
}