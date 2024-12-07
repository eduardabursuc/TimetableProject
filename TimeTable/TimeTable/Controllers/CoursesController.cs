using Application.DTOs;
using Application.UseCases.Commands.CourseCommands;
using Application.UseCases.Queries.CourseQueries;
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

            return CreatedAtAction(nameof(GetById), new { courseId = result.Data }, result.Data);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
        {
            if (id != command.Id)
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

        [HttpGet("{courseId}")]
        public async Task<ActionResult<CourseDto>> GetById(Guid courseId)
        {
            var result = await mediator.Send(new GetCourseByIdQuery { CourseId = courseId });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetAll(string userEmail)
        {
            var result = await mediator.Send(new GetAllCoursesQuery { UserEmail = userEmail});

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var result = await mediator.Send(new DeleteCourseCommand(id));

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.ErrorMessage);
        }
    }
}