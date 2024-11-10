using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstraintsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ConstraintsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateConstraint([FromBody] CreateConstraintCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConstraint(Guid id, [FromBody] UpdateConstraintCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in the URL does not match ID in the command.");
            }

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConstraintDto>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetConstraintByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<List<ConstraintDto>>> GetAll()
        {
            var result = await mediator.Send(new GetAllConstraintsQuery());

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}