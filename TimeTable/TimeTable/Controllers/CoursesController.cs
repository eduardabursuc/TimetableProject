using Application.DTOs;
using Application.UseCases.Commands.CourseCommands;
using Application.UseCases.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CoursesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
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

            return CreatedAtAction(nameof(GetByName), new { courseName = result.Data }, result.Data);
        }

        [HttpPut("{courseName}")]
        public async Task<IActionResult> UpdateCourse(string courseName, [FromBody] UpdateCourseCommand command)
        {
            if (courseName != command.CourseName)
            {
                return BadRequest("Name in the URL does not match Name in the command.");
            }

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpGet("{courseName}")]
        public async Task<ActionResult<CourseDto>> GetByName(string courseName)
        {
            var result = await mediator.Send(new GetCourseByNameQuery { CourseName = courseName });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetAll()
        {
            var result = await mediator.Send(new GetAllCoursesQuery());

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteCourse(string name)
        {
            var result = await mediator.Send(new DeleteCourseCommand(name));

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.ErrorMessage);
        }
    }
}