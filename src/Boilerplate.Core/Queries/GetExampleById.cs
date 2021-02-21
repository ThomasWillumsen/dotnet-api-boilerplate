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
    public static class GetExampleById
    {
        // query
        public record Query(int id) : IRequest<ExampleEntity>;

        // handler
        public class Handler : IRequestHandler<Query, ExampleEntity>
        {
            private readonly IAppDbContext _dbContext;

            public Handler(IAppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<ExampleEntity> Handle(Query request, CancellationToken cancellationToken)
            {
                var example = await _dbContext.Examples.FirstOrDefaultAsync(x => x.Id == request.id);
                if (example == null)
                    throw new KeyNotFoundException($"Example with id {request.id} was not found");

                return example;
            }
        }
    }
}