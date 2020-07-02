using Microsoft.Extensions.Logging;
using Moq;
using Boilerplate.Core.BLL;
using System;
using System.Threading.Tasks;
using Xunit;
using Boilerplate.Core.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Boilerplate.Settings;
using Boilerplate.Core.Database.Entities;

namespace Boilerplate.Core.Tests.Documents
{
    public class ExampleLogicTests
    {
        private readonly Mock<ILogger<ExampleLogic>> _mockLogger;
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<IOptionsMonitor<Appsettings>> _mockAppsettings;

        public ExampleLogicTests()
        {
            _mockLogger = new Mock<ILogger<ExampleLogic>>(MockBehavior.Loose);
            _mockDbContext = new Mock<IAppDbContext>(MockBehavior.Strict);
            _mockAppsettings = new Mock<IOptionsMonitor<Appsettings>>(MockBehavior.Strict);
            _mockAppsettings.Setup(x => x.CurrentValue).Returns(new Appsettings
            {
            });
        }

        #region GetExamples
        [Fact]
        public async Task GetExamples_ReturnsEntriesFromDb()
        {
            // Arrange
            var examples = new List<ExampleEntity>
            {
                new ExampleEntity(){
                    Id = 1,
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
            var SUT = new ExampleLogic(
                _mockLogger.Object,
                _mockAppsettings.Object,
                new AppDbContext(dbContextOptions));
            var result = await SUT.GetExamples();

            // Assert
            Assert.Single(result);
            Assert.Equal(examples[0].Id, result[0].Id);
        }
        #endregion

        #region CreateExample
        [Fact]
        public async Task CreateExample_CreatesEntryInDbAndReturnsEntry()
        {
            // Arrange
            var name = "new example";

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            var SUT = new ExampleLogic(
                _mockLogger.Object,
                _mockAppsettings.Object,
                new AppDbContext(dbContextOptions));
            var result = await SUT.CreateExample(name);

            // Assert
            using (var dbContext = new AppDbContext(dbContextOptions))
            {
                var createdEntries = await dbContext.Examples.ToArrayAsync();
                Assert.Single(createdEntries);
                Assert.Equal(createdEntries[0].Id, result.Id);
            }
        }
        #endregion
    }
}