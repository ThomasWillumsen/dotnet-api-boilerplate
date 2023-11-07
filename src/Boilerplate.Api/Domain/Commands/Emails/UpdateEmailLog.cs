using MediatR;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Api.Domain.Commands.Emails;

public static class UpdateEmailLog
{
    public record Command(Guid reference, EmailEventEnum emailEvent) : IRequest;

    public class Handler : IRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var emailLog = await _dbContext.EmailLogs.FirstOrDefaultAsync(x => x.Reference == request.reference);
            if (emailLog == null)
                throw new Exception($"No email log with reference {request.reference} was found");

            emailLog.Event = request.emailEvent;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}