using Application.DTOs;
using Application.UseCases.Commands.TimetableCommands;
using Application.UseCases.Queries.TimetableQueries;
using Application.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    
    public class TimetablesController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<TimetableDto>> Create([FromBody] CreateTimetableCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(
                actionName: nameof(GetTimetableById),
                routeValues: new { id = result.Data },
                value: result.Data
            );
        }
        
        [Authorize(Roles = "admin")]
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
        public async Task<ActionResult<List<TimetableDto>>> GetAllTimetables(string userEmail)
        {
            var result = await mediator.Send(new GetAllTimetablesQuery {UserEmail = userEmail});

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [Authorize(Roles = "admin")]
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
        
        [HttpGet("forProfessor")]
        public async Task<ActionResult<List<TimetableDto>>> GetTimetablesForProfessor([FromQuery] string professorEmail)
        {
            var query = new GetAllForProfessorQuery
            {
                ProfessorEmail = professorEmail
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