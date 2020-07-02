using Microsoft.AspNetCore.Mvc;
using Boilerplate.Api.Models.Response;
using Boilerplate.Core.BLL;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Boilerplate.Api.Models.Request;
using Boilerplate.Api.ErrorHandling;

namespace Boilerplate.Api.Controllers
{

    /// <summary>
    /// Example API
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        private readonly IExampleLogic _exampleLogic;

        public ExamplesController(IExampleLogic exampleLogic)
        {
            this._exampleLogic = exampleLogic;
        }

        /// <summary>
        /// Get examples
        /// </summary>
        /// <remarks>
        /// This is where you fetch examples blabla...
        /// </remarks>
        [HttpGet(Name = nameof(GetExamples))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ExampleResponse>> GetExamples()
        {
            var examples = await _exampleLogic.GetExamples();
            return Ok(examples.Select(x => new ExampleResponse(x)).ToArray());
        }

        /// <summary>
        /// Create an example
        /// </summary>
        /// <remarks>
        /// This is where you create new examples blabla...
        /// </remarks>
        [HttpPost(Name = nameof(CreateExample))]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ExampleResponse>> CreateExample([FromBody] CreateExampleRequest body)
        {
            var example = await _exampleLogic.CreateExample(body.Name);
            return Created("", new ExampleResponse(example));
        }
    }
}
