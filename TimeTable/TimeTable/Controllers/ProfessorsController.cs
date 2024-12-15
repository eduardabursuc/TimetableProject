using Application.DTOs;
using Application.UseCases.Commands.ProfessorCommands;
using Application.UseCases.Queries.ProfessorQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfessorsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateProfessor([FromBody] CreateProfessorCommand command)
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProfessor(Guid id, [FromBody] UpdateProfessorCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body");
            }
            
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProfessorDto>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetProfessorByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProfessorDto>>> GetAll(string userEmail)
        {
            var result = await mediator.Send(new GetAllProfessorsQuery { UserEmail = userEmail });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProfessor(Guid id)
        {
            var result = await mediator.Send(new DeleteProfessorCommand(id));

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.ErrorMessage);
        }
        
        [HttpPost("add-timetable")]
        public async Task<IActionResult> AddTimetableToProfessor([FromBody] AddTimetableToProfessorCommand command)
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
    }
}