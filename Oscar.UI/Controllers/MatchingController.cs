using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Matching.Queries;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MatchingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatchingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Obsolete("This method only exists for testing purposes and will be removed - please do not use!")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<MatchTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromBody] GetMatchingQuery getMatchingQuery)
        {
            var result = await _mediator.Send(getMatchingQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }
    }
}
