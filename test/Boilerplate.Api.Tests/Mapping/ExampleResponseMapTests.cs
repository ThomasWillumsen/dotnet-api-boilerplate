using System;
using Boilerplate.Api.Models.Response;
using Boilerplate.Core.Database.Entities;
using Xunit;

namespace Boilerplate.Api.Tests.Mapping
{
    public class ExampleResponseMapTests
    {
        public ExampleResponseMapTests()
        {
        }

        [Fact]
        public void ExampleResponse_MapCorrectlyFromEntityModel()
        {
            // Arrange
            var example = new ExampleEntity()
            {
                Id = 1,
                Name = "test1",
                CreatedDate = DateTime.Now
            };

            // Act
            var responseModel = new ExampleResponse(example);

            // Assert
            Assert.Equal(example.Id, responseModel.Id);
            Assert.Equal(example.Name, responseModel.Name);
        }
    }
}