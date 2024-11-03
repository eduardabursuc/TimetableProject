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
            return CreatedAtAction(nameof(GetById), new { id = result }, command);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Guid>> UpdateConstraint(Guid id, UpdateConstraintCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in the URL does not match ID in the command.");
            }
            await mediator.Send(command);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConstraintDTO>> GetById(Guid id)
        {
            var constraint = await mediator.Send(new GetConstraintByIdQuery { Id = id });
            if (constraint == null)
            {
                return NotFound();
            }
            return constraint;
        }

        [HttpGet]
        public async Task<ActionResult<List<ConstraintDTO>>> GetAll()
        {
            return await mediator.Send(new GetAllConstraintsQuery());
        }
    }
}