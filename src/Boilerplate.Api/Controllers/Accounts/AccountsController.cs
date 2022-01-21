using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Boilerplate.Api.Domain.Commands.Accounts;
using Microsoft.AspNetCore.Authorization;
using Boilerplate.Api.Domain.Queries.Accounts;
using System.Linq;
using Boilerplate.Api.Domain.Commands.Accounts.LoginAccount;

namespace Boilerplate.Api.Controllers.Accounts;

/// <summary>
/// Accounts API
/// </summary>
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    [Authorize]
    [HttpPost(Name = nameof(CreateAccount))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest body)
    {
        var account = await _mediator.Send(new CreateAccount.Command(body.FullName, body.Email));
        return Created("", new AccountResponse(account));
    }

    /// <summary>
    /// Get accounts
    /// </summary>
    [HttpGet(Name = nameof(GetAccounts))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<AccountResponse>> GetAccounts()
    {
        var accounts = await _mediator.Send(new GetAccounts.Query());
        return Ok(accounts.Select(x => new AccountResponse(x)));
    }

    /// <summary>
    /// Login to an existing account
    /// </summary>
    /// <remarks>Returns a jwt token to be used with bearer authentication</remarks>
    [HttpPost("login", Name = nameof(LoginAccount))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<LoginResponse>> LoginAccount([FromBody] LoginAccountRequest body)
    {
        var loginResult = await _mediator.Send(new LoginAccount.Command(body.Email, body.Password));
        return Ok(new LoginResponse(loginResult.JwtToken, loginResult.Account));
    }

    /// <summary>
    /// Reset the password of an existing account.
    /// </summary>
    /// <remarks>An email will be sent to the owner with a password token to use when updating the password.</remarks>
    [HttpPost("resetPassword", Name = nameof(ResetAccountPassword))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> ResetAccountPassword([FromBody] ResetAccountPasswordRequest body)
    {
        await _mediator.Send(new ResetAccountPassword.Command(body.Email));
        return NoContent();
    }

    /// <summary>
    /// Update the password of an account using the token from the reset password email.
    /// </summary>
    [HttpPut("updatePassword", Name = nameof(UpdateAccountPassword))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateAccountPassword([FromBody] UpdateAccountPasswordRequest body)
    {
        await _mediator.Send(new UpdateAccountPassword.Command(body.ResetPasswordToken, body.Password));
        return NoContent();
    }
}