using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Episode.Queries;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EpisodeController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getEpisodeByIdQuery = new GetEpisodeByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getEpisodeByIdQuery);

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
        [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] EpisodeAddDto episodeAddDto)
        {
            var addEpisodeCommand = new AddEpisodeCommand();
            addEpisodeCommand.EpisodeAddDto = episodeAddDto;
            var result = await _mediator.Send(addEpisodeCommand);

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
        public async Task<IActionResult> Put(int id, [FromBody] EpisodeUpdateDto episodeUpdateDto)
        {
            var updateEpisodeCommand = new UpdateEpisodeCommand();
            updateEpisodeCommand.EpisodeUpdateDto = episodeUpdateDto;
            updateEpisodeCommand.Id = id;
            var result = await _mediator.Send(updateEpisodeCommand);

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
            var deleteEpisodeCommand = new DeleteEpisodeCommand();
            deleteEpisodeCommand.Id = id;
            var result = await _mediator.Send(deleteEpisodeCommand);

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
