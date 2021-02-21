using System.ComponentModel.DataAnnotations;
using Boilerplate.Core.Database.Entities;

namespace Boilerplate.Api.Models.Request
{
    public class CreateExampleRequest
    {
        /// <summary>
        /// The name of the example
        /// </summary>
        [Required]
        public string Name { get; set; }
        public TypeEnum Type { get; set; }

        public ExampleEntity MapToEntity()
        {
            return new ExampleEntity
            {
                Name = Name,
                Type = Type
            };
        }
    }
}