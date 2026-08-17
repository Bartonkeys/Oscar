using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.Season.Queries;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]")]
    [ApiController]
    public class SeasonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeasonController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getSeasonByIdQuery = new GetSeasonByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getSeasonByIdQuery);

            if (result.IsSuccess)
            {
                return result.Value == null ? NotFound() : Ok(result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(SeasonDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] SeasonAddDto seasonAddDto)
        {
            var addSeasonCommand = new AddSeasonCommand();
            addSeasonCommand.SeasonAddDto = seasonAddDto;
            var result = await _mediator.Send(addSeasonCommand);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody] SeasonUpdateDto seasonUpdateDto)
        {
            var updateSeasonCommand = new UpdateSeasonCommand();
            updateSeasonCommand.SeasonUpdateDto = seasonUpdateDto;
            updateSeasonCommand.Id = id;
            var result = await _mediator.Send(updateSeasonCommand);

            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return result.Error == CommandResult.NOTFOUND ? NotFound() : BadRequest(new { ErrorMessage = result.Error });
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteSeasonCommand = new DeleteSeasonCommand();
            deleteSeasonCommand.Id = id;
            var result = await _mediator.Send(deleteSeasonCommand);

            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return result.Error == CommandResult.NOTFOUND ? NotFound() : BadRequest(new { ErrorMessage = result.Error });
            }
        }


    }
}
