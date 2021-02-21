using Microsoft.AspNetCore.Mvc;
using Boilerplate.Api.Models.Response;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Boilerplate.Api.Models.Request;
using Boilerplate.Api.ErrorHandling;
using MediatR;
using Boilerplate.Core.Queries;
using System.Collections.Generic;
using Boilerplate.Core.Commands;

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
        private readonly IMediator _mediator;

        public ExamplesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Get examples
        /// </summary>
        /// <remarks>
        /// This is where you fetch examples blabla...
        /// </remarks>
        [HttpGet("{id}", Name = nameof(GetExample))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ExampleResponse>> GetExample([FromRoute] int id)
        {
            var example = await _mediator.Send(new GetExampleById.Query(id));
            return Ok(new ExampleResponse(example));
        }

        /// <summary>
        /// Get examples
        /// </summary>
        /// <remarks>
        /// This is where you fetch examples blabla...
        /// </remarks>
        [HttpGet(Name = nameof(GetExamples))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ExampleResponse>>> GetExamples()
        {
            var examples = await _mediator.Send(new GetExamples.Query());
            return Ok(examples.Select(x => new ExampleResponse(x)));
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
            var example = await _mediator.Send(new CreateExample.Command(body.MapToEntity()));
            return Created("", new ExampleResponse(example));
        }
    }
}
