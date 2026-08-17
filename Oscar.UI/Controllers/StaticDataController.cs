

using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Matching.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("staticData")]
    [ApiController]
    public class StaticDataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StaticDataController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("all")]
        [ProducesResponseType(typeof(EnumDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GatAll()
        {
            var staticDataQuery = new GetClientStaticDataQuery();
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("client/grades")]
        [ProducesResponseType(typeof(EnumDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGrades()
        {
            var staticDataQuery = new GetClientStaticDataQuery(Enums.ClientGrade) ;
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("client/types")]
        [ProducesResponseType(typeof(EnumDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTypes()
        {
            var staticDataQuery = new GetClientStaticDataQuery(Enums.ClientType);
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("client/statuses")]
        [ProducesResponseType(typeof(EnumDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStatuses()
        {
            var staticDataQuery = new GetClientStaticDataQuery(Enums.Status);
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("country/all")]
        [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCountries()
        {
            var staticDataQuery = new GetCountryQuery();
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("matching/requestStatus")]
        [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMatchingRequestStatus()
        {
            var staticDataQuery = new GetMatchingStaticDataQuery(Enums.MatchingRequestStatus);
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("matching/rules")]
        [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMatchingRules()
        {
            var staticDataQuery = new GetMatchingStaticDataQuery(Enums.MatchRules);
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("works/genre")]
        [ProducesResponseType(typeof(GenreDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorksGenre()
        {
            var staticDataQuery = new GetGenreStaticDataQuery();
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("works/language")]
        [ProducesResponseType(typeof(LanguageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorksLanguage()
        {
            var staticDataQuery = new GetLanguageStaticDataQuery();
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpGet("works/status")]
        [ProducesResponseType(typeof(LanguageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorksStatus()
        {
            var staticDataQuery = new GetWorksStaticDataQuery(Enums.WorksStatus);
            var result = await _mediator.Send(staticDataQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }
    }
}