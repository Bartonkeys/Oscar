using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Series.Commands;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeriesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getSeriesByIdQuery = new GetSeriesByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getSeriesByIdQuery);

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
        [ProducesResponseType(typeof(SeriesDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] SeriesAddDto seriesAddDto)
        {
            var addSeriesCommand = new AddSeriesCommand();
            addSeriesCommand.SeriesAddDto = seriesAddDto;
            var result = await _mediator.Send(addSeriesCommand);

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
        public async Task<IActionResult> Put(int id, [FromBody] SeriesUpdateDto seriesUpdateDto)
        {
            var updateSeriesCommand = new UpdateSeriesCommand();
            updateSeriesCommand.SeriesUpdateDto = seriesUpdateDto;
            updateSeriesCommand.Id = id;
            var result = await _mediator.Send(updateSeriesCommand);

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
            var deleteSeriesCommand = new DeleteSeriesCommand();
            deleteSeriesCommand.Id = id;
            var result = await _mediator.Send(deleteSeriesCommand);

            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return result.Error == CommandResult.NOTFOUND ? NotFound() : BadRequest(new { ErrorMessage = result.Error });
            }
        }

        [HttpPost("searchByTitle")]
        [ProducesResponseType(typeof(WorksDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchByTitle(SeriesSearchByTitleQuery request)
        {
            var result = await _mediator.Send(request);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { ErrorMessage = result.Error });
        }

    }
}
