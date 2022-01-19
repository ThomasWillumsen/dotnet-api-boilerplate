using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Controllers.Accounts;

public class CreateAccountRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string FullName { get; set; }
}