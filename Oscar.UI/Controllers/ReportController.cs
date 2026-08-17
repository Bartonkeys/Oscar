
using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Report.Queries;
using Oscar.Infrastructure.Features.Report.Commands;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Report.Services;

namespace Oscar.UI.Controllers
{
    [Authorize]
    [RequiredScope("access_oscar_user")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(int id, [FromBody] GetReportDataByIdQuery getReportDataByIdQuery)
        {
            getReportDataByIdQuery.Id = id;

            var result = await _mediator.Send(getReportDataByIdQuery);

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
        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ReportAddDto reportAddDto)
        {
            var addReportCommand = new AddReportCommand
            {
                ReportAddDto = reportAddDto
            };
            var result = await _mediator.Send(addReportCommand);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value);
            }
            else
            {
                return BadRequest(new { ErrorMessage = result.Error });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var getReportByIdQuery = new GetReportByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getReportByIdQuery);

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
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromBody]GetReportsQuery getReportsQuery)
        {
            var result = await _mediator.Send(getReportsQuery);

            return result.OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(new { ErrorMessage = r.Error }));
        }

        //TODO: this controller id for debug on reporting only - should be removed for production 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public string TestReport()
        {
            string jsonString = @"[
  {
    ""ClientName"": ""CABLE READY CORPORATION"",
    ""ClientReference"": ""7016"",
    ""Email"": ""LOU@CABLEREADY.NET"",
    ""Works"": [
      {
        ""Id"": 248360,
        ""IMaestroWorkCode"": ""234594"",
        ""WorksStatus"": 1,
        ""WorksTitle"": [
          {
            ""Title"": ""INSIDE THE ACTORS STUDIO SEASON 1""
          }
        ]
      }
     
    ]
  },
  {
    ""ClientName"": ""PARADE MEDIA GROUP"",
    ""ClientReference"": ""21984"",
    ""Email"": ""MATTHEW.ASHCROFT@PARADE.MEDIA"",
    ""Works"": [
      {
        ""Id"": 525763,
        ""IMaestroWorkCode"": ""461337"",
        ""WorksTitle"": [
          {
            ""Title"": ""INSIDE THE BOX WITH JACK STEIN- SEASON 1""
          }
        ]
      },
      {
        ""Id"": 554602,
        ""IMaestroWorkCode"": ""465322"",
        ""WorksStatus"": 1,
        ""WorksTitle"": [
          {
            ""Title"": ""INSIDE NAVY STRATEGIES- SEASON 1""
          }
        ]
      },
      {
        ""Id"": 554604,
        ""IMaestroWorkCode"": ""465324"",
        ""WorksStatus"": 1,
        ""WorksTitle"": [
          {
            ""Title"": ""INSIDE RUSSIAN SOCIETY- SEASON 1""
          }
        ]
      }
    ]
  }
]";
            var csv = ReportHelperService.ConvertJsonToCsv(jsonString);

            return csv;
        }
    }
}
