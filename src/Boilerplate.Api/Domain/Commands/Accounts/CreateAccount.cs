using System;
using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Api.Infrastructure.Database.Entities;
using Boilerplate.Api.Domain.Services;
using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Boilerplate.Api.Domain.Commands.Emails;

namespace Boilerplate.Api.Domain.Commands.Accounts;

public static class CreateAccount
{
    public record Command(string FullName, string Email) : IRequest<AccountEntity>;

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
            await _dbContext.Accounts.AddAsync(newAccount);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is SqlException sqlEx &&
                    sqlEx.Number == 2601)
                    throw new ConflictException(ErrorCodes.Account.CREATEACCOUNT_EMAIL_ALREADY_EXIST);

                throw;
            }

            await _mediator.Send(new SendResetPasswordMail.Command(request.Email, request.FullName, newAccount.ResetPasswordToken.Value));
            return newAccount;
        }
    }
}