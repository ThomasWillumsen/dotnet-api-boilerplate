using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Boilerplate.Api.Models.Response;
using System.Net;
using System;
using System.Collections.Generic;
using Boilerplate.Core.Database;
using Boilerplate.Core.Database.Entities;
using Boilerplate.Api.Tersts.Controllers;
using Boilerplate.Api.Models.Request;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Boilerplate.Api.Tests.Controllers
{
    public class ExamplesControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly IServiceProvider _serviceProvider;

        public ExamplesControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _serviceProvider = factory.Services.CreateScope().ServiceProvider;
        }

        [Fact]
        public async Task GetExamples_EndpointSuccessTest()
        {
            // Arrange
            var examples = new List<ExampleEntity>
            {
                new ExampleEntity(){
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

            var responseObj = JsonSerializer.Deserialize<IEnumerable<ExampleResponse>>(await httpResponse.Content.ReadAsStringAsync());
            Assert.NotEmpty(responseObj);
        }

        [Fact]
        public async Task GetExample_EndpointSuccessTest()
        {
            // Arrange
            var examples = new List<ExampleEntity>
            {
                new ExampleEntity(){
                    Name = "test1",
                    CreatedDate = DateTime.Now
                },
                new ExampleEntity(){
                    Name = "test2",
                    CreatedDate = DateTime.Now
                }
            };

            var appDbContext = _serviceProvider.GetService<IAppDbContext>();
            await appDbContext.Examples.AddRangeAsync(examples);
            await appDbContext.SaveChangesAsync();

            // Act
            var httpClient = _factory.Server.CreateClient();
            var httpResponse = await httpClient.GetAsync($"/api/v1/examples/{examples[1].Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<ExampleResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(responseObj);
            Assert.Equal(examples[1].Id, responseObj.Id);
        }

        [Fact]
        public async Task CreateExample_EndpointSuccessTest()
        {
            // Arrange
            var request = new CreateExampleRequest
            {
                Name = "new example",
                Type = TypeEnum.B
            };

            // Act
            var httpClient = _factory.Server.CreateClient();
            var httpResponse = await httpClient.PostAsync($"/api/v1/examples",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<ExampleResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal(request.Name, responseObj.Name);
        }
    }
}