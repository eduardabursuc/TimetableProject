using Application.DTOs;
using Application.UseCases.Queries.TimetableQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    
    public class TimetablesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<TimetableDto>>> GetAll()
        {
            var result = await mediator.Send(new GetAllTimetablesQuery());

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

    }
}