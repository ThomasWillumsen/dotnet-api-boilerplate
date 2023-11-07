using Boilerplate.Api.Infrastructure.Database.Entities;
using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Boilerplate.Api.Domain.Commands.Emails;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Api.Domain.Commands.Accounts;

public static class CreateAccount
{
    public record Command(string FullName, string Email, bool IsAdmin) : IRequest<AccountEntity>;

    public class Handler : IRequestHandler<Command, AccountEntity>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(
            AppDbContext dbContext,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<AccountEntity> Handle(Command request, CancellationToken cancellationToken)
        {

            var newAccount = new AccountEntity(request.FullName, request.Email);
            newAccount.ResetPasswordToken = Guid.NewGuid();
            newAccount.Claims.Add(new AccountClaimEntity(ClaimTypeEnum.User));
            if (request.IsAdmin)
                newAccount.Claims.Add(new AccountClaimEntity(ClaimTypeEnum.Admin));

            await _dbContext.Accounts.AddAsync(newAccount);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is MySqlException mysqlEx &&
                    mysqlEx.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                    throw new ConflictException(ErrorCodesEnum.ACCOUNT_EMAIL_ALREADY_EXIST);

                throw;
            }

            await _mediator.Send(new SendResetPasswordMail.Command(request.Email, request.FullName, newAccount.ResetPasswordToken.Value));
            return newAccount;
        }
    }
}