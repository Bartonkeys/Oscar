using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Infrastructure.Features.Matching.Queries;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MatchResultController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatchResultController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Get(int id)
        {
            var getMatchResultByIdQuery = new GetMatchResultByIdQuery();
            getMatchResultByIdQuery.Id = id;

            var result = await _mediator.Send(getMatchResultByIdQuery);

            if (result.IsFailure) return BadRequest(new { ErrorMessage = result.Error });
            
            if (result.Value == null || result.Value.FileBytes == null || result.Value.FileBytes.Length == 0) return NotFound();
                
            return File(result.Value.FileBytes, "text/plain", Path.GetFileName($"{result.Value.Reference}_MATCHED.csv"));
        }

    }
}
