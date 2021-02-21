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
    public class GetExamplesTests
    {
        private readonly IAppDbContext _mockDbContext;

        public GetExamplesTests()
        {
            _mockDbContext = Substitute.For<IAppDbContext>();
        }

        [Fact]
        public async Task GetExamples_ReturnsEntriesFromDb()
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
            var SUT = new GetExamples.Handler(new AppDbContext(dbContextOptions));
            var result = await SUT.Handle(new GetExamples.Query(), new CancellationToken(false));

            // Assert
            Assert.Single(result);
            Assert.Equal(examples[0].Id, result.First().Id);
        }
    }
}