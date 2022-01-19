using System;
using Boilerplate.Api.Infrastructure.Database.Entities;

namespace Boilerplate.Api.Controllers.Accounts;

public class AccountResponse
{
    public AccountResponse() { }
    public AccountResponse(AccountEntity model)
    {
        Email = model.Email;
        FullName = model.FullName;
        CreatedDate = model.CreatedDate;
        LastModifiedDate = model.LastModifiedDate;
    }

    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}