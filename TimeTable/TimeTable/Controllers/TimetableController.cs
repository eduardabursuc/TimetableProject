using Application.DTOs;
using Application.UseCases.Commands.TimetableCommands;
using Application.UseCases.Queries.TimetableQueries;
using Application.Utils;
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
 
        [HttpGet("byRoom")]
        public async Task<ActionResult<TimetableDto>> GetTimetableByRoom([FromQuery] Guid id, [FromQuery] string roomName)
        {
            var result = await mediator.Send(new GetTimetableByRoomQuery { Id = id, RoomName = roomName });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [HttpGet("byGroup")]
        public async Task<ActionResult<TimetableDto>> GetTimetableByGroup([FromQuery] Guid id, [FromQuery] string groupName)
        {
            var result = await mediator.Send(new GetTimetableByGroupQuery { Id = id, GroupName = groupName });
       
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
       
            return Ok(result.Data);
        }
        
        [HttpGet("byProfessor")]
        public async Task<ActionResult<TimetableDto>> GetTimetableByProfessor([FromQuery] Guid id, [FromQuery] Guid professorId)
        {
            var result = await mediator.Send(new GetTimetableByProfessorQuery { Id = id, ProfessorId = professorId });
   
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
        
        [HttpGet("paginated")]
        public async Task<ActionResult<PagedResult<TimetableDto>>> GetFilteredTimetables([FromQuery] int page, [FromQuery] int pageSize)
        {
            var query = new GetFilteredTimetablesQuery
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await mediator.Send(query);
            
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            
            return NotFound(result.ErrorMessage);
        }


    }
}