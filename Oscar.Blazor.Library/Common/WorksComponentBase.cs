using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Actor.Queries;
using Oscar.Infrastructure.Features.Director.Queries;
using Oscar.Infrastructure.Features.Distributor.Queries;
using Oscar.Infrastructure.Features.Producer.Queries;
using Oscar.Infrastructure.Features.ScreenWriter.Queries;
using Oscar.Infrastructure.Features.ScriptWriter.Queries;

namespace Oscar.Blazor.Library.Common
{
    public class WorksComponentBase: OscarComponentBase
    {

        //protected async Task<List<PersonDto>?> LoadPersonsAsync(PersonType type)
        //{
        //    switch (type)
        //    {
        //        case PersonType.Actor:
        //            return (await Mediator.Send(new GetAllActorsQuery())).Value.OrderBy(x => x.LastName).ToList();
        //        case PersonType.Director:
        //            return (await Mediator.Send(new GetAllDirectorsQuery())).Value.OrderBy(x => x.LastName).ToList();
        //        case PersonType.Producer:
        //            return (await Mediator.Send(new GetAllProducersQuery())).Value.OrderBy(x => x.LastName).ToList();
        //        case PersonType.Distributor:
        //            return (await Mediator.Send(new GetAllDistributorsQuery())).Value.OrderBy(x => x.LastName).ToList();
        //        case PersonType.ScreenWriter:
        //            return (await Mediator.Send(new GetAllScreenWritersQuery())).Value.OrderBy(x => x.LastName).ToList();
        //        case PersonType.ScriptWriter:
        //            return (await Mediator.Send(new GetAllScriptWritersQuery())).Value.OrderBy(x => x.LastName).ToList();
        //    }

        //    return null;
        //}

    }
}
