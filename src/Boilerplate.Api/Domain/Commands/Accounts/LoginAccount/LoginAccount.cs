using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Api.Domain.Services;
using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Boilerplate.Api.Infrastructure.Authorization;

namespace Boilerplate.Api.Domain.Commands.Accounts;

public static class LoginAccount
{
    public record Command(string Email, string Password) : IRequest<LoginAccountResult>;

    public class Handler : IRequestHandler<Command, LoginAccountResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenHelper _jwtTokenHelper;

        public Handler(
            AppDbContext dbContext,
            IPasswordService passwordService,
            IJwtTokenHelper jwtTokenHelper)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<LoginAccountResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            if (account == null)
                throw new NotFoundException(ErrorCodes.Account.ACCOUNT_EMAIL_DOESNT_EXIST);

            if (string.IsNullOrEmpty(account.Password))
                throw new BusinessRuleException(ErrorCodes.Account.LOGIN_PASSWORD_NOT_CREATED);

            if (_passwordService.VerifyPassword(request.Password, account.Salt, account.Password) == false)
                throw new BusinessRuleException(ErrorCodes.Account.LOGIN_PASSWORD_INVALID);

            var token = _jwtTokenHelper.GenerateJwtToken(account);
            return new LoginAccountResult(account, token);
        }
    }
}