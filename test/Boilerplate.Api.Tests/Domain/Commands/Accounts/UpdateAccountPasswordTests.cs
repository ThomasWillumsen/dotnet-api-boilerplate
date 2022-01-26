using System;
using Boilerplate.Api.Domain.Commands.Accounts;
using Boilerplate.Api.Domain.Exceptions;
using Boilerplate.Api.Domain.Services;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Boilerplate.Api.Tests.Domain.Commands.Accounts;

public class UpdateAccountPasswordTests : TestBase
{
    private readonly IPasswordService _mockPasswordService;
    private IRequestHandler<UpdateAccountPassword.Command> _sut;

    public UpdateAccountPasswordTests()
    {
        _mockPasswordService = Substitute.For<IPasswordService>();
        _sut = new UpdateAccountPassword.Handler(new AppDbContext(_dbOptions), _mockPasswordService);
    }

    [Fact]
    public async void UpdateAccountPassword_ThrowsBusinessRuleException_WhenPasswordTokenDoesntExist()
    {
        // Arrange
        var pwToken = Guid.NewGuid();
        var command = new UpdateAccountPassword.Command(pwToken, "newpassword");

        // Act
        var exception = await Record.ExceptionAsync(async () => await _sut.Handle(command, default));

        // Assert
        Assert.IsType<BusinessRuleException>(exception);
    }

    [Fact]
    public async void UpdateAccountPassword_UpdatesAccountWithParams()
    {
        // Arrange
        var pwToken = Guid.NewGuid();
        var password = "somepassword";
        AccountEntity account;
        using (var context = new AppDbContext(_dbOptions))
        {
            account = new AccountEntity("asdasd", "jkh214h21@mail.com"){
                ResetPasswordToken = pwToken};
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }

        var pwDto = new PasswordDto("somehash", new byte[] { 0x20, 0x20});
        _mockPasswordService.EncryptPassword(password).Returns(pwDto);

        var command = new UpdateAccountPassword.Command(pwToken, password);

        // Act
        await _sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var accAfter = await context.Accounts.FirstAsync(x => x.Id == account.Id);

            Assert.Equal(pwDto.Hash, accAfter.Password);
            Assert.Equal(pwDto.Salt, accAfter.Salt);
            Assert.Null(accAfter.ResetPasswordToken);
        }
    }
}