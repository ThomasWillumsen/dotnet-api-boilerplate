using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;
using Boilerplate.Api.Domain.Commands.Emails;

namespace Boilerplate.Api.Domain.Commands.Accounts;

public static class ResetAccountPassword
{
    public record Command(string Email) : IRequest;

    public class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (account == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_EMAIL_DOESNT_EXIST);

            var resetToken = Guid.NewGuid();
            account.ResetPasswordToken = resetToken;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new SendResetPasswordMail.Command(account.Email, account.FullName, resetToken));
        }
    }
}