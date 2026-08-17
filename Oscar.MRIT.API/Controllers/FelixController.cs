using BartonKeys.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Oscar.MRIT.API.Attributes;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.MRITModels;
using Oscar.Mrit.Features.FelixMrit.Commands;
using Oscar.Mrit.Features.MRITIntegration.Commands;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.MRIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FelixApiKey]
    public class FelixController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FelixController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieve feed of production data from Felix
        /// </summary>
        /// <param name="felixWorksQuery"></param>
        /// <returns></returns>
        [HttpPost("Feed")]
        [ProducesResponseType(typeof(IEnumerable<ProductionModel>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFeed([FromBody] FelixWorksQuery felixWorksQuery) =>
            (await _mediator.Send(felixWorksQuery)).OnBoth(r =>
                r.IsSuccess ? (IActionResult) Ok(r.Value) : BadRequest(r.Error));

        /// <summary>
        /// Retrieve clients and their associated catalogues that have been processed in MRIT.
        /// </summary>
        /// <returns>List of Clients with associated catalogues in MRIT</returns>
        [HttpGet("Catalogues")]
        [ProducesResponseType(typeof(IEnumerable<ClientCataloguesDto>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCatalogues() =>
            (await _mediator.Send(new CataloguesByClientQuery()))
            .OnBoth(r => r.IsSuccess ? (IActionResult) Ok(r.Value) : BadRequest(r.Error));

        /// <summary>
        /// Retrieve clients and their associated works that have been processed in MRIT.
        /// </summary>
        /// <returns>List of Clients with associated WorksIds in MRIT</returns>
        [HttpGet("Works")]
        [ProducesResponseType(typeof(ClientWorksDto), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorks(int clientId) =>
            (await _mediator.Send(new WorksByClientQuery {ClientId = clientId}))
            .OnBoth(r => r.IsSuccess ? (IActionResult) Ok(r.Value) : BadRequest(r.Error));

        /// <summary>
        /// Retrieve catalogues and their associated works
        /// </summary>
        /// <param name="catalogues"></param>
        /// <returns></returns>
        [HttpPost("CatalogueWorks")]
        [ProducesResponseType(typeof(IEnumerable<CatalogueWorksDto>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostCatalogueWorks([FromBody] List<CatalogueDto> catalogues) =>
            (await _mediator.Send(new WorksByCataloguesQuery() {Catalogues = catalogues}))
            .OnBoth(r => r.IsSuccess ? (IActionResult) Ok(r.Value) : BadRequest(r.Error));

        /// <summary>
        /// Retrieve clients and catalogues for a list of worksIds
        /// </summary>
        /// <param name="worksIds"></param>
        /// <returns></returns>
        [HttpPost("ClientCatalogues")]
        [ProducesResponseType(typeof(IEnumerable<ClientCataloguesDto>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorksClientsAndCatalogues([FromBody] List<int> worksIds) =>
            (await _mediator.Send(new ClientAndCatalogueByWorksQuery() { WorksIds = worksIds }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        /// <summary>
        /// Update match status directly in Felix
        /// </summary>
        /// <param name="felixMritMatches"></param>
        /// <returns></returns>
        [HttpPost("Matches")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostMatches([FromBody] AddFelixMritMatchesCommand felixMritMatches) =>
            (await _mediator.Send(felixMritMatches)).OnBoth(r =>
                r.IsSuccess ? (IActionResult)Ok() : BadRequest(r.Error));

        /// <summary>
        /// Post matches back to Felix
        /// </summary>
        /// <param name="updateMatchStatusCommand"></param>
        /// <returns></returns>
        [HttpPost("Status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostMatchStatus([FromBody] UpdateMatchStatusCommand updateMatchStatusCommand) =>
            (await _mediator.Send(updateMatchStatusCommand)).OnBoth(r =>
                r.IsSuccess ? (IActionResult) Ok() : BadRequest(r.Error));

        [HttpGet("Errors")]
        [ProducesResponseType(typeof(IEnumerable<MatchStatusDto>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUnsuccessfulMatches() =>
            (await _mediator.Send(new UnsuccessfulMatchStatusQuery()))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        /// <summary>
        /// Retrieve clients and their associated works that have been processed in MRIT.
        /// </summary>
        /// <returns>List of Clients with associated WorksIds in MRIT</returns>
        [HttpPost("Productions")]
        [ProducesResponseType(typeof(ClientWorksDto), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWorks(IEnumerable<int> worksIds) =>
            (await _mediator.Send(new WorksByIdQuery { WorksIds = worksIds }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));
    }
}
