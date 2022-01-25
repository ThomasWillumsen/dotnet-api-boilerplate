using System;
using System.Threading;
using Boilerplate.Api.Domain.Commands.Accounts;
using Boilerplate.Api.Domain.Commands.Emails;
using Boilerplate.Api.Infrastructure.Database;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Boilerplate.Api.Tests.Domain.Commands.Accounts;

public class CreateAccountTests : IDisposable
{
    private readonly IMediator _mockMediatr;
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _dbOptions;

    public CreateAccountTests()
    {
        _mockMediatr = Substitute.For<IMediator>();
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        using (var context = new AppDbContext(_dbOptions))
        {
            context.Database.EnsureCreated();
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public async void CreateAccount_CreatesAccountWithParams()
    {
        // Arrange
        var command = new CreateAccount.Command("Donald Trump", "jhlk@asd.com", false);

        // Act
        var sut = new CreateAccount.Handler(new AppDbContext(_dbOptions), _mockMediatr);
        var account = await sut.Handle(command, default);

        // Assert
        using (var context = new AppDbContext(_dbOptions))
        {
            var createdAccount = await context.Accounts
                .Include(x => x.Claims)
                .FirstAsync(x => x.Id == account.Id);

            Assert.Equal(command.Email, createdAccount.Email);
            Assert.Equal(command.FullName, createdAccount.FullName);
            Assert.Equal(command.Email, createdAccount.Email);
            Assert.NotNull(createdAccount.ResetPasswordToken);
        }
    }

    [Fact]
    public async void CreateAccount_InvokeMediatorSendResetPasswordMail()
    {
        // Arrange
        var command = new CreateAccount.Command("Donald Trump", "fhgddfgh@asd.com", false);

        // Act
        var sut = new CreateAccount.Handler(new AppDbContext(_dbOptions), _mockMediatr);
        var account = await sut.Handle(command, default);

        // Assert
        await _mockMediatr.Received(1).Send(Arg.Any<SendResetPasswordMail.Command>(), Arg.Any<CancellationToken>());
    }
}