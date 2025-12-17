using ahis.template.application.Features.CountryFeatures.Command;
using ahis.template.application.Features.CountryFeatures.Query;
using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using ahis.template.domain.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ahis.template.api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(ResponseDto<List<CountryVM>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllCountryQuery { };
            return Response(await _mediator.Send(query));

        }

        //[HttpPost("Add")]
        //[ProducesResponseType(typeof(ResponseDto<object>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Produces("application/json")]
        //public async Task<IActionResult> AddCountry([FromBody] AddCountryCommand command)
        //{
        //    return Ok(await _mediator.Send(command));
        //}
    }
}
