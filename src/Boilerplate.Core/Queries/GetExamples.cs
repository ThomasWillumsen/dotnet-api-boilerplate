using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Core.Database;
using Boilerplate.Core.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Core.Queries
{
    public static class GetExamples
    {
        // query
        public record Query() : IRequest<IEnumerable<ExampleEntity>>;

        // handler
        public class Handler : IRequestHandler<Query, IEnumerable<ExampleEntity>>
        {
            private readonly IAppDbContext _dbContext;

            public Handler(IAppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<IEnumerable<ExampleEntity>> Handle(Query request, CancellationToken cancellationToken)
            {
                var examples = await _dbContext.Examples.ToArrayAsync();
                return examples;
            }
        }
    }
}