using System.ComponentModel.DataAnnotations;
using Boilerplate.Core.Database.Entities;

namespace Boilerplate.Api.Models.Response
{
    public class ExampleResponse
    {
        public ExampleResponse() { }
        public ExampleResponse(ExampleEntity model)
        {
            Id = model.Id;
            Name = model.Name;
        }

        /// <summary>
        /// The unique identifier of the example
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the example
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}