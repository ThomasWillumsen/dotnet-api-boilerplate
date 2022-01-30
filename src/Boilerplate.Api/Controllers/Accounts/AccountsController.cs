using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Boilerplate.Api.Domain.Commands.Accounts;
using Microsoft.AspNetCore.Authorization;
using Boilerplate.Api.Domain.Queries.Accounts;
using System.Linq;
using Boilerplate.Api.Infrastructure.Authorization;

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
    [Authorize(AuthPolicies.Admin)]
    [HttpPost(Name = nameof(CreateAccount))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest body)
    {
        var account = await _mediator.Send(new CreateAccount.Command(body.FullName, body.Email, body.IsAdmin));
        return Created("", new AccountResponse(account));
    }

    /// <summary>
    /// Update an existing account
    /// </summary>
    [Authorize]
    [HttpPut("{id}", Name = nameof(UpdateAccount))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<AccountResponse>> UpdateAccount(
        [FromRoute] int id,
        [FromBody] UpdateAccountRequest body)
    {
        var account = await _mediator.Send(new UpdateAccount.Command(id, body.FullName, body.Email));
        return Ok(new AccountResponse(account));
    }

    /// <summary>
    /// Update an account to have admin permissions
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpPut("{id}/updateIsAdmin", Name = nameof(UpdateAccountIsAdmin))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateAccountIsAdmin(
        [FromRoute] int id,
        [FromBody] UpdateAccountIsAdminRequest body)
    {
        await _mediator.Send(new UpdateAccountIsAdmin.Command(id, body.IsAdmin));
        return NoContent();
    }

    /// <summary>
    /// Get accounts
    /// </summary>
    [Authorize(AuthPolicies.Admin)]
    [HttpGet(Name = nameof(GetAccounts))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
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