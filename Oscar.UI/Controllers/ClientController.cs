using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getClientByIdQuery = new GetClientByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getClientByIdQuery); 

            if (result.IsSuccess)
            {
                return result.Value == null ? NotFound() : Ok(result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Basic()
        {
            var getClientBasicQuery = new GetClientBasicQuery();
            
            var result = await _mediator.Send(getClientBasicQuery);

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
        public async Task<IActionResult> Create([FromBody] ClientAddDto clientAddDto)
        {
            var addClientCommand = new AddClientCommand
            {
                ClientAddDto = clientAddDto
            };
            var result = await _mediator.Send(addClientCommand);

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
        public async Task<IActionResult> Put(int id, [FromBody] ClientUpdateDto clientUpdateDto)
        {
            var updateClientCommand = new UpdateClientCommand();
            updateClientCommand.ClientUpdateDto = clientUpdateDto;
            updateClientCommand.Id = id;
            var result = await _mediator.Send(updateClientCommand);

            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return result.Error == CommandResult.NOTFOUND ? NotFound() : BadRequest(new { ErrorMessage = result.Error });
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromBody] GetClientsQuery getClientsQuery)
        {

            var result = await _mediator.Send(getClientsQuery);
                
            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }


        [HttpPatch]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateClientCommand updateClientCommand)
        {
            var result = await _mediator.Send(updateClientCommand);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

       
    }
}
