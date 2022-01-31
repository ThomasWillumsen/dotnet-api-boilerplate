using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Controllers.Accounts;

public class BaseAccountRequest
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string FullName { get; set; } = null!;
}