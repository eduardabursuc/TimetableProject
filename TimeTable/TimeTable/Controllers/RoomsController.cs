using Application.DTOs;
using Application.UseCases.Queries.RoomQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class RoomsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<RoomDto>>> GetAll(string userEmail)
        {
            var result = await mediator.Send(new GetAllRoomsQuery { UserEmail = userEmail });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

    }
}


