using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Api.Domain.Services;
using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Boilerplate.Api.Infrastructure.Authorization;

namespace Boilerplate.Api.Domain.Commands.Accounts.LoginAccount;

public static class LoginAccount
{
    public record Command(string Email, string Password) : IRequest<LoginAccountResult>;

    public class Handler : IRequestHandler<Command, LoginAccountResult>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LoginAccount.Command> _logger;
        private readonly AuthorizationSettings _authSettings;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenHelper _jwtTokenHelper;

        public Handler(
            AppDbContext dbContext,
            ILogger<LoginAccount.Command> logger,
            IOptions<AuthorizationSettings> authSettings,
            IPasswordService passwordService,
            IJwtTokenHelper jwtTokenHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _authSettings = authSettings.Value;
            _passwordService = passwordService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<LoginAccountResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (account == null)
                throw new NotFoundException(ErrorCodes.Account.LOGIN_EMAIL_DOESNT_EXIST);

            if (string.IsNullOrEmpty(account.Password))
                throw new BusinessRuleException(ErrorCodes.Account.LOGIN_PASSWORD_NOT_CREATED);

            if (_passwordService.VerifyPassword(request.Password, account.Salt, account.Password) == false)
                throw new BusinessRuleException(ErrorCodes.Account.LOGIN_PASSWORD_INVALID);

            var token = _jwtTokenHelper.GenerateJwtToken(account);
            return new LoginAccountResult(account, token);
        }
    }
}