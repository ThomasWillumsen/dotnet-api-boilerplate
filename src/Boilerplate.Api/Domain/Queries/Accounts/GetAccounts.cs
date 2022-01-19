using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Api.Infrastructure.Database;
using Boilerplate.Api.Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Api.Domain.Queries.Accounts;

public static class GetAccounts
{
    // query
    public record Query() : IRequest<IEnumerable<AccountEntity>>;

    // handler
    public class Handler : IRequestHandler<Query, IEnumerable<AccountEntity>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AccountEntity>> Handle(Query request, CancellationToken cancellationToken)
        {
            var accounts = await _dbContext.Accounts
                .AsNoTracking()
                .ToArrayAsync();
            return accounts;
        }
    }
}