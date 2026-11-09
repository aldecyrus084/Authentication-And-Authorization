using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Features.Authentication;
using MyApp.Application.Features.Registration;
using MyApp.Application.Interfaces.Repository;
using MyApp.Application.Interfaces.Services;

namespace MyApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.isSuccess)
                return BadRequest(response.message);

            return Ok(response);
        }
    }
}
