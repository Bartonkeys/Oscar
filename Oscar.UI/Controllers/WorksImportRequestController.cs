using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using Oscar.Infrastructure.Features.WorksImport.Queries;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class WorksImportRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorksImportRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromForm] WorksImportRequestAddDto worksImportRequestAddDto)
        {
            var addWorksImportRequestCommand = new AddWorksImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };
            var result = await _mediator.Send(addWorksImportRequestCommand);

            if (result.IsSuccess)
            {
                return Ok();
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
        public async Task<IActionResult> Put(int id, [FromBody]WorksImportRequestDto worksImportRequestDto)
        {
            var worksImportRequestCommand = new ResubmitWorksImportRequestCommand();
            worksImportRequestCommand.Id = id;
            var result = await _mediator.Send(worksImportRequestCommand);

            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return result.Error == CommandResult.NOTFOUND ? NotFound() : BadRequest(new { ErrorMessage = result.Error });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getWorksImportRequestByIdQuery = new GetWorksImportRequestByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getWorksImportRequestByIdQuery);

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
        [Route("/WorksImportRequest/WorksImports/Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorksImports([FromForm] GetWorksImportsByRequestIdQuery getWorksImportsByRequestIdQuery)
        {
            var result = await _mediator.Send(getWorksImportsByRequestIdQuery);

            if (result.IsSuccess)
            {
                return result.Value == null ? NotFound() : Ok(result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }

        [HttpDelete]
        [Route("/WorksImportRequest/WorksImports/Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteWorksImports(int id)
        {
            var deleteWorksImportCommand = new DeleteWorksImportCommand()
            {
                Id = id
            };
            var result = await _mediator.Send(deleteWorksImportCommand);

            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status204NoContent);
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
            var rollbackWorksImportRequestCommand = new RollbackWorksImportRequestCommand
            {
                Id = id
            };
            var result = await _mediator.Send(rollbackWorksImportRequestCommand);

            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
                return result.Error == CommandResult.NOTFOUND ? NotFound() : BadRequest(new { ErrorMessage = result.Error });
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<WorksDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync([FromBody] GetWorksImportRequestsQuery getWorksImportRequestsQuery)
        {
            var result = await _mediator.Send(getWorksImportRequestsQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

    }
}
