using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Api.Infrastructure.Database.Entities;
using Boilerplate.Api.Domain.Services;
using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;

namespace Boilerplate.Api.Domain.Commands.Accounts
{
    public static class LoginAccount
    {
        public record Command(string Email, string Password) : IRequest<AccountEntity>;

        public class Handler : IRequestHandler<Command, AccountEntity>
        {
            private readonly AppDbContext _dbContext;
            private readonly IPasswordService _passwordService;

            public Handler(
                AppDbContext dbContext,
                IPasswordService passwordService)
            {
                _dbContext = dbContext;
                _passwordService = passwordService;
            }

            public async Task<AccountEntity> Handle(Command request, CancellationToken cancellationToken)
            {
                var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == request.Email);
                if (account == null)
                    throw new NotFoundException(ErrorCodes.Account.LOGIN_EMAIL_DOESNT_EXIST);

                if (string.IsNullOrEmpty(account.Password))
                    throw new BusinessRuleException(ErrorCodes.Account.LOGIN_PASSWORD_NOT_CREATED);

                if (_passwordService.VerifyPassword(request.Password, account.Salt, account.Password) == false)
                    throw new BusinessRuleException(ErrorCodes.Account.LOGIN_PASSWORD_INVALID);

                return account;
            }
        }
    }
}