using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Controllers.Accounts;

public class CreateAccountRequest : BaseAccountRequest
{
    [Required]
    public bool IsAdmin { get; set; }
}