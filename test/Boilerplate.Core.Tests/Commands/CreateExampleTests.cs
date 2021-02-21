using System;
using System.Threading.Tasks;
using Xunit;
using Boilerplate.Core.Database;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Core.Database.Entities;
using NSubstitute;
using System.Threading;
using System.Linq;
using Boilerplate.Core.Commands;
using Microsoft.Extensions.Logging;

namespace Boilerplate.Core.Tests.Commands
{
    public class CreateExampleTests
    {
        private readonly IAppDbContext _mockDbContext;
        private readonly ILogger<CreateExample.Handler> _mockLogger;

        public CreateExampleTests()
        {
            _mockDbContext = Substitute.For<IAppDbContext>();
            _mockLogger = Substitute.For<ILogger<CreateExample.Handler>>();
        }

        [Fact]
        public async Task CreateExample_PersitsEntryInDb()
        {
            // Arrange
            var example = new ExampleEntity()
            {
                Name = "test1",
                Type = TypeEnum.B,
                CreatedDate = DateTime.Now
            };

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            var SUT = new CreateExample.Handler(new AppDbContext(dbContextOptions), _mockLogger);
            var result = await SUT.Handle(new CreateExample.Command(example), new CancellationToken(false));

            // Assert
            using (var dbContext = new AppDbContext(dbContextOptions))
            {
                var createdResult = dbContext.Examples.First();
                Assert.Equal(example.Name, createdResult.Name);
                Assert.Equal(example.Type, createdResult.Type);
            }
        }
    }
}