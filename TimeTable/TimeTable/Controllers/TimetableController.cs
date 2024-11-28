using Application.DTOs;
using Application.UseCases.Commands.TimetableCommands;
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
        
        [HttpGet("{id}")]
        public async Task<ActionResult<TimetableDto>> GetTimetableById(Guid id)
        {
            return Ok();
        }


    }
}