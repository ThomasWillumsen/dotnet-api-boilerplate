using System;
using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Controllers.Accounts;

public class UpdateAccountPasswordRequest
{
    [Required]
    public Guid ResetPasswordToken { get; set; }
    [Required]
    public string Password { get; set; } = null!;
}