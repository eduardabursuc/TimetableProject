using Application.DTOs;
using Application.UseCases.Commands.TimetableCommands;
using Application.UseCases.Queries.TimetableQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    
    public class TimetablesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<TimetableDto>> CreateTimetable([FromBody] CreateTimetableCommand command)
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

            return CreatedAtAction(
                actionName: nameof(GetTimetableById),
                routeValues: new { id = result.Data.Id },
                value: result.Data
            );
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTimetable(Guid id, [FromBody] UpdateTimetableCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TimetableDto>> GetTimetableById(Guid id)
        {
            var result = await mediator.Send(new GetTimetableByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<List<TimetableDto>>> GetAllTimetables()
        {
            var result = await mediator.Send(new GetAllTimetablesQuery());

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTimetable(Guid id)
        {
            var result = await mediator.Send(new DeleteTimetableCommand(id));

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.ErrorMessage);
        }


    }
}