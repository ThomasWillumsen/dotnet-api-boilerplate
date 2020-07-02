using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.Models.Request
{
    public class CreateExampleRequest
    {
        /// <summary>
        /// The name of the example
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}