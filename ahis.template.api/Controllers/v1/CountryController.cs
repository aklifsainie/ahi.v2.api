using ahis.template.application.Features.CountryFeatures.Command;
using ahis.template.application.Features.CountryFeatures.Query;
using ahis.template.application.Shared.Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ahis.template.api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllCountryQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCountry([FromBody] AddCountryCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
