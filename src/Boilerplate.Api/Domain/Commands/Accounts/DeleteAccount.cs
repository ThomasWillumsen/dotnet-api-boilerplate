using Boilerplate.Api.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.ErrorHandling;

namespace Boilerplate.Api.Domain.Commands.Accounts;

public static class DeleteAccount
{
    public record Command(int Id) : IRequest;

    public class Handler : IRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var existingAccount = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.Id);
            if(existingAccount == null)
                throw new NotFoundException(ErrorCodesEnum.ACCOUNT_ID_DOESNT_EXIST);    
            
            _dbContext.Remove(existingAccount);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}