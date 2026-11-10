using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase

{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "user:update")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new FetchAllUserCommand());
        return Ok(result);
    }
}