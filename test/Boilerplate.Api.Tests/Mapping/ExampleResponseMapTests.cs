using System;
using Boilerplate.Api.Models.Request;
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
            Assert.Equal(example.Type, responseModel.Type);
        }

        [Fact]
        public void ExampleRequest_MapCorrectlyToEntityModel()
        {
            // Arrange
            var example = new CreateExampleRequest()
            {
                Name = "test1",
                Type = TypeEnum.B
            };

            // Act
            var entityModel = example.MapToEntity();

            // Assert
            Assert.Equal(example.Name, entityModel.Name);
            Assert.Equal(example.Type, entityModel.Type);
        }
    }
}