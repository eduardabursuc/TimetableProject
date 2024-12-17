using Application.DTOs;
using Application.UseCases.Commands.GroupCommands;
using Application.UseCases.Commands.ProfessorCommands;
using Application.UseCases.Queries.GroupQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GroupDto>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetGroupByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateGroupCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [Authorize(Roles = "admin")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Unit>> Update(Guid id, [FromBody] UpdateGroupCommand command)
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

            return Ok(result.Data);
        }
        
        [Authorize(Roles = "admin")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Unit>> Delete(Guid id)
        {
            var result = await mediator.Send(new DeleteGroupCommand(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

    }
}


