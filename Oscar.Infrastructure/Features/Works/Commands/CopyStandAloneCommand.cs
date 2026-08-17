using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Extensions;

namespace Oscar.Infrastructure.Features.Works.Commands
{
    public class CopyStandAloneCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }

        public int NewClientID { get; set; }
        
        public int NewCatalogueID {  get; set; }

        public bool Relinquish { get; set; }

    }

    public class CopyStandAloneCommandHandler : AbstractBaseHandler<CopyStandAloneCommand, string>
    {
        public CopyStandAloneCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<CopyStandAloneCommand> validator, ILogger<CopyStandAloneCommand> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        
        protected override async Task<Result<string>> HandleRequest(CopyStandAloneCommand request, CancellationToken cancellationToken)
        {
            var copiedWorks = new List<Core.Entities.StandAlone>();

            //if (request.Works?.Count < 1)
            //    return Result.Fail<StandAloneDto>("No list of works supplied for copy");

            var work = OscarContext.StandAlones
                        .Include(w => w.Clients)
                        .Include(w => w.Catalogues)
                        .Include(w => w.Actors)
                        .Include(w => w.AlternativeTitles)
                        .Include(w => w.Companies)
                        .Include(w => w.Conflicts)
                        .Include(w => w.Countries)
                        .Include(w => w.Directors)
                        .Include(w => w.Distributors)
                        .Include(w => w.Rights)!.ThenInclude(c => c.Countries)
                        .Include(w => w.Rights)!.ThenInclude(c => c.LanguageRights).ThenInclude(l => l.Language)
                        .Include(w => w.Rights)!.ThenInclude(c => c.ChannelRights).ThenInclude(c => c.Channel)
                        .Include(w => w.Rights)!.ThenInclude(c => c.Type)
                        .Include(w => w.Titles)
                        .Include(w => w.Languages)
                        .Include(w => w.Producers)
                        .Include(w => w.ScreenWriters)
                        .Include(w => w.WorksStatusHistory)
                        .Include(w => w.WorksType)
                        .AsSplitQuery()
                        .SingleOrDefault(w => w.Id == request.Id);

            var newClient = OscarContext.Clients.FirstOrDefault(c => c.Id == request.NewClientID);

            Core.Entities.Catalogue newCat = null;

            if (request.NewCatalogueID > 0)
                newCat = OscarContext.Catalogues.FirstOrDefault(c => c.Id == request.NewCatalogueID);

            if (newClient == null)
                return Result.Fail<string>("No valid client id supplied for copy");

            if (request.Relinquish)
            {
                if (work.Clients != null)
                    foreach (var client in work.Clients)
                        work.Clients?.Remove(client);

                if (work.Catalogues != null && newCat != null)
                    foreach (var cat in work.Catalogues)
                        work.Catalogues?.Remove(cat);

                work.Clients?.Add(newClient);

                if (newCat != null)
                    work.Catalogues?.Add(newCat);
            }
            else
            {
                var cloneWork = new Core.Entities.StandAlone(); 

                var values = OscarContext.Entry(work).CurrentValues.Clone();
                OscarContext.Entry(cloneWork).CurrentValues.SetValues(values);
                cloneWork.Id = 0;

                //do not copy over AS400RefNo
                cloneWork.AS400RefNo = null;

                if (cloneWork != null)
                {
                    OscarContext.Works.Add(cloneWork);

                    cloneWork.Actors = work.Actors.Load(OscarContext);
                    cloneWork.AlternativeTitles = work.AlternativeTitles.Clone(OscarContext);
                    cloneWork.Companies = work.Companies.Load(OscarContext);
                    cloneWork.Conflicts = work.Conflicts.Clone(OscarContext);
                    cloneWork.Countries = work.Countries.Load(OscarContext);
                    cloneWork.Directors = work.Directors.Load(OscarContext);
                    cloneWork.Distributors = work.Distributors.Load(OscarContext);
                    cloneWork.Rights = work.Rights.CloneRights(OscarContext);
                    cloneWork.Titles = work.Titles.Clone(OscarContext);
                    cloneWork.Languages = work.Languages.Load(OscarContext);
                    cloneWork.Producers = work.Producers.Load(OscarContext);
                    cloneWork.ScreenWriters = work.ScreenWriters.Load(OscarContext);
                    cloneWork.WorksStatusHistory = work.WorksStatusHistory.Clone(OscarContext);
                    //cloneWork.WorksType = work.WorksType.Clone(OscarContext);
                    cloneWork.CompactRef = AutoGenerateCompactRef();

                    cloneWork.Clients = new List<Client> { newClient };

                    if (newCat != null)
                        cloneWork.Catalogues = new List<Core.Entities.Catalogue> { newCat };

                    copiedWorks.Add(cloneWork);
                }
            }
            

            OscarContext.SaveChanges();
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
