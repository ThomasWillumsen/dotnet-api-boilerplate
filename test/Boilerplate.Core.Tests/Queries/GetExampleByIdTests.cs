using System;
using System.Threading.Tasks;
using Xunit;
using Boilerplate.Core.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Boilerplate.Core.Database.Entities;
using NSubstitute;
using Boilerplate.Core.Queries;
using System.Threading;
using System.Linq;

namespace Boilerplate.Core.Tests.Queries
{
    public class GetExampleByIdTests
    {
        private readonly IAppDbContext _mockDbContext;

        public GetExampleByIdTests()
        {
            _mockDbContext = Substitute.For<IAppDbContext>();
        }

        [Fact]
        public async Task GetExampleById_ReturnsEntriesFromDb()
        {
            // Arrange
            var examples = new List<ExampleEntity>
            {
                new ExampleEntity(){
                    Name = "test1",
                    CreatedDate = DateTime.Now
                }
            };
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var appDbContext = new AppDbContext(dbContextOptions);
            appDbContext.Examples.AddRange(examples);
            appDbContext.SaveChanges();

            // Act
            var SUT = new GetExampleById.Handler(new AppDbContext(dbContextOptions));
            var result = await SUT.Handle(new GetExampleById.Query(examples.First().Id), new CancellationToken(false));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(examples[0].Id, result.Id);
        }
    }
}