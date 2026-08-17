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
    public class EpisideImportRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EpisideImportRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromForm] WorksImportRequestAddDto worksImportRequestAddDto)
        {
            var addWorksImportRequestCommand = new AddEpisodeImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };
            var result = await _mediator.Send(addWorksImportRequestCommand);

            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }

    }
}
