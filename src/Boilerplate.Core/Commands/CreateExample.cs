using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Core.Database;
using Boilerplate.Core.Database.Entities;
using Boilerplate.Core.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Boilerplate.Core.Commands
{
    public static class CreateExample
    {
        public record Command(ExampleEntity example) : IRequest<ExampleEntity>;

        public class Handler : IRequestHandler<Command, ExampleEntity>
        {
            private readonly IAppDbContext _dbContext;
            private readonly ILogger<Handler> _logger;

            public Handler(IAppDbContext dbContext, ILogger<CreateExample.Handler> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task<ExampleEntity> Handle(Command request, CancellationToken cancellationToken)
            {
                using (new TimedOperation(_logger, "Creating example in DB"))
                {
                    await _dbContext.Examples.AddAsync(request.example);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return request.example;
                }
            }
        }
    }
}