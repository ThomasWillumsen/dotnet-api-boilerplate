using System;
using Boilerplate.Api.Infrastructure.Database.Entities;

namespace Boilerplate.Api.Controllers.Accounts;

public class AccountResponse
{
    #pragma warning disable CS8618
    public AccountResponse() { }
    #pragma warning restore CS8618
    public AccountResponse(AccountEntity model)
    {
        Id = model.Id;
        Email = model.Email;
        FullName = model.FullName;
        CreatedDate = model.CreatedDate;
        LastModifiedDate = model.LastModifiedDate;
        IsAdmin = model.HasAdminClaim();
    }

    public int Id { get; set; }
    public string Email { get; set; }
    public string? FullName { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}