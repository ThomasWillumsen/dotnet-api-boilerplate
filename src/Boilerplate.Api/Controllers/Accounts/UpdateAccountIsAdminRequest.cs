using System;
using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Controllers.Accounts;

public class UpdateAccountIsAdminRequest
{
    [Required]
    public bool IsAdmin { get; set; }
}