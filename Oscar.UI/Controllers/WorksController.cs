using System.Web.Http;
using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Infrastructure.Features.Works.Queries;


namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class WorksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getWorksByIdQuery = new GetWorksByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getWorksByIdQuery);

            if (result.IsSuccess)
            {
                return result.Value == null ? NotFound() : Ok(result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }

        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Title([FromUri] SearchWorksQuery request)
        {
            var result = await _mediator.Send(request);

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
        [ProducesResponseType(typeof(IEnumerable<WorksDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromBody] GetWorksQuery getWorksQuery)
        {
            var result = await _mediator.Send(getWorksQuery);
                
            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpPost]
        [ProducesResponseType(typeof(WorksDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AddWorksCommand addWorksCommand)
        {
            var result = await _mediator.Send(addWorksCommand);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<WorksDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Copy([FromBody] CopyWorksCommand copyWorksCommand)
        {
            var result = await _mediator.Send(copyWorksCommand);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        [HttpPatch]
        [ProducesResponseType(typeof(WorksDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateWorksCommand updateWorksCommand)
        {
            var result = await _mediator.Send(updateWorksCommand);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Title()
        {
            var getWorksTitleQuery = new GetWorksTitleQuery();

            var result = await _mediator.Send(getWorksTitleQuery);

            if (result.IsSuccess)
            {
                return result.Value == null ? NotFound() : Ok(result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }


    }
}
