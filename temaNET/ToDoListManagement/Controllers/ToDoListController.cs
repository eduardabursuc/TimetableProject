using Application.UseCases.Commands;
using Application.DTOs;
using Application.UseCases.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ToDoItemsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ToDoItemDTO>>> GetToDoItems()
        {
            return await mediator.Send(new GetToDoItemsQuery());
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateToDoItem(CreateToDoItemCommand command)
        {
            var id = await mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItemDTO>> GetById(Guid id)
        {
            var toDoItem = await mediator.Send(new GetToDoItemByIdQuery { Id = id });
            if (toDoItem == null)
            {
                return NotFound();
            }
            return toDoItem;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await mediator.Send(new DeleteToDoItemByIdCommand { Id = id });
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateToDoItemCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in the URL does not match ID in the command.");
            }
            await mediator.Send(command);
            return NoContent();
        }
    }
}
