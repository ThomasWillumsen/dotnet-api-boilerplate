using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Tests.TestUtils;
using System;
using Boilerplate.Api.Controllers.Accounts;
using Boilerplate.Api.Infrastructure.Database;

namespace Boilerplate.Api.Tests.Controllers;

public class AccountsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AccountsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Account_FullSystemTest()
    {
        // Arrange
        var email = "someemail@asd.com";
        var updatedEmail = "someotheremail@asd.com";
        var fullName = "donald trump";
        var updatedFullName = "kim jong un";
        var passwordFirst = "somepassword12312";
        HttpClient httpClient = null;

        // Act
        // 1. Create Account
        httpClient = _factory.CreateNewHttpClient(true);
        var createAccountResponse = await httpClient.PostAsync($"/api/v1/accounts",
            new CreateAccountRequest
            {
                Email = email,
                FullName = fullName,
                IsAdmin = false
            }.ToHttpStringContent());
        var account = await createAccountResponse.DeserializeHttpResponse<AccountResponse>();

        // 2. Update Password
        Guid? pwResetToken = null;
        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>())
        {
            var createdAccount = await appDbContext.Accounts.FirstAsync();
            pwResetToken = createdAccount.ResetPasswordToken;
        }

        httpClient = _factory.CreateNewHttpClient(false);
        var updatePasswordResponse = await httpClient.PutAsync($"/api/v1/accounts/updatePassword",
            new UpdateAccountPasswordRequest
            {
                Password = passwordFirst,
                ResetPasswordToken = pwResetToken.Value
            }.ToHttpStringContent());

        // 3. Login
        httpClient = _factory.CreateNewHttpClient(false);
        var loginResponse = await httpClient.PostAsync($"/api/v1/accounts/login",
            new LoginAccountRequest
            {
                Email = email,
                Password = passwordFirst
            }.ToHttpStringContent());

        // 4. Reset password
        httpClient = _factory.CreateNewHttpClient(false);
        var resetPwResponse = await httpClient.PostAsync($"/api/v1/accounts/resetPassword",
            new ResetAccountPasswordRequest
            {
                Email = email
            }.ToHttpStringContent());

        // 5. Update Account
        httpClient = _factory.CreateNewHttpClient(true);
        var updateAccountResponse = await httpClient.PutAsync($"/api/v1/accounts/{account.Id}",
            new UpdateAccountRequest
            {
                Email = updatedEmail,
                FullName = updatedFullName
            }.ToHttpStringContent());

        // 6. Update Account IsAdmin permissions
        httpClient = _factory.CreateNewHttpClient(true);
        var updateIsAdminResponse = await httpClient.PutAsync($"/api/v1/accounts/{account.Id}/updateIsAdmin",
            new UpdateAccountIsAdminRequest
            {
                IsAdmin = true
            }.ToHttpStringContent());

        // Assert
        Assert.Equal(HttpStatusCode.Created, createAccountResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, updatePasswordResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, resetPwResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, updateAccountResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, updateIsAdminResponse.StatusCode);

        using (var appDbContext = _factory.GetScopedServiceProvider().GetService<AppDbContext>())
        {
            var createdAccount = await appDbContext.Accounts
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == account.Id);
            Assert.Equal(updatedFullName, createdAccount.FullName);
            Assert.Equal(updatedEmail, createdAccount.Email);
            Assert.Equal(true, createdAccount.Claims.Count >= 2);
            Assert.NotNull(createdAccount.Password);
            Assert.NotNull(createdAccount.Salt);

            var loginResponseObj = await loginResponse.DeserializeHttpResponse<LoginResponse>();
            Assert.NotNull(loginResponseObj.Jwt);
        }
    }
}