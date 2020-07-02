using Moq;
using Boilerplate.Core.BLL;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Boilerplate.Api.Models.Response;
using System.Net;
using System;
using System.Collections.Generic;
using Boilerplate.Core.Database;
using Newtonsoft.Json;
using Boilerplate.Core.Database.Entities;
using Boilerplate.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Boilerplate.Api.Tersts.Controllers;
using Boilerplate.Api.Models.Request;
using System.Net.Http;
using System.Text;

namespace Boilerplate.Api.Tests.Controllers
{
    public class ExamplesControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<IExampleLogic> _mockExampleLogic;

        public ExamplesControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _serviceProvider = factory.Services.CreateScope().ServiceProvider;
            _mockExampleLogic = new Mock<IExampleLogic>(MockBehavior.Strict);
        }

        #region GetExamples
        [Fact]
        public async Task GetExamples_InvokeLogicAndReturnResult()
        {
            // Arrange
            var examples = new ExampleEntity[]
            {
                new ExampleEntity(){
                    Id = 1,
                    Name = "test1",
                    CreatedDate = DateTime.Now
                }
            };

            _mockExampleLogic.Setup(x => x.GetExamples())
                .ReturnsAsync(examples);

            // Act
            var SUT = new ExamplesController(
                _mockExampleLogic.Object
            );

            var result = await SUT.GetExamples();

            // Assert
            _mockExampleLogic.VerifyAll();
            var responseObj = (result.Result as OkObjectResult).Value as IEnumerable<ExampleResponse>;
            Assert.True(responseObj.Select(x => x.Id).SequenceEqual(examples.Select(x => x.Id)));
        }


        [Fact]
        public async Task GetExamples_EndpointSuccessTest()
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

            var appDbContext = _serviceProvider.GetService<IAppDbContext>();
            await appDbContext.Examples.AddRangeAsync(examples);
            await appDbContext.SaveChangesAsync();

            // Act
            var httpClient = _factory.Server.CreateClient();
            var httpResponse = await httpClient.GetAsync($"/api/v1/examples");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var responseObj = JsonConvert.DeserializeObject<IEnumerable<ExampleResponse>>(
                await httpResponse.Content.ReadAsStringAsync());
            Assert.NotEmpty(responseObj);
        }
        #endregion

        #region CreateExample
        [Fact]
        public async Task CreateExample_InvokeLogicAndReturnResult()
        {
            // Arrange
            var name = "new example";
            var example = new ExampleEntity()
            {
                Id = 1,
                Name = name,
                CreatedDate = DateTime.Now
            };

            _mockExampleLogic.Setup(x => x.CreateExample(name))
                .ReturnsAsync(example);

            // Act
            var SUT = new ExamplesController(
                _mockExampleLogic.Object
            );

            var result = await SUT.CreateExample(new CreateExampleRequest
            {
                Name = name
            });

            // Assert
            _mockExampleLogic.VerifyAll();
            var responseObj = (result.Result as CreatedResult).Value as ExampleResponse;
            Assert.Equal(example.Id, responseObj.Id);
        }


        [Fact]
        public async Task CreateExample_EndpointSuccessTest()
        {
            // Arrange
            var request = new CreateExampleRequest
            {
                Name = "new example"
            };

            // Act
            var httpClient = _factory.Server.CreateClient();
            var httpResponse = await httpClient.PostAsync($"/api/v1/examples",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

            var responseObj = JsonConvert.DeserializeObject<ExampleResponse>(
                await httpResponse.Content.ReadAsStringAsync());
            Assert.Equal(request.Name, responseObj.Name);
        }
        #endregion
    }
}