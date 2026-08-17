using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Registration.Commands;
using Oscar.Infrastructure.Features.Common;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Matching.Queries;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] RegistrationBatchCreateDto registrationBatchCreateDto)
        {

            //var registrationBatchCreateDto = new RegistrationBatchCreateDto()
            //{

            //    registrationBatchCreateItemDtos = new List<RegistrationBatchCreateItemDto>()
            //    {
            //        new RegistrationBatchCreateItemDto(){
            //            ClientId = 1,
            //            CatalogueId = 1,
            //            WorksId = 188471,
            //            RegisterType = RegisterType.Zero
            //        }
            //    },

            //    RuntimeParamsJson = "test"

            //};
            var addRegistrationBatchCommand = new AddRegistrationBatchCommand
            {
                RegistrationBatchCreateDto = registrationBatchCreateDto
            };
            var result = await _mediator.Send(addRegistrationBatchCommand);

            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }

        //[HttpPost]
        //[ProducesResponseType(typeof(IEnumerable<WorksDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetAsync([FromBody] GetMatchRequestsQuery getMatchRequestsQuery)
        //{
        //    var result = await _mediator.Send(getMatchRequestsQuery);

        //    return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        //}


        //[HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var getMatchRequestByIdQuery = new GetMatchRequestByIdQuery
        //    {
        //        Id = id
        //    };
        //    var result = await _mediator.Send(getMatchRequestByIdQuery);

        //    if (result.IsSuccess)
        //    {
        //        return result.Value == null ? NotFound() : Ok(result.Value);
        //    }
        //    else
        //    {
        //        return BadRequest(new { ErrorMessage = result.Error });
        //    }
        //}

    }
}
