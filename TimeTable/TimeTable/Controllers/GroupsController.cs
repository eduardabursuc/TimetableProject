using Application.DTOs;
using Application.UseCases.Queries.GroupQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class GroupsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetAll(string userEmail)

        {
            var result = await mediator.Send(new GetAllGroupsQuery { UserEmail = userEmail });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

    }
}


