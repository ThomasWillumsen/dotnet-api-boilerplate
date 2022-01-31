using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Controllers.Accounts;

public class ResetAccountPasswordRequest
{
    [Required]
    public string Email { get; set; } = null!;
}