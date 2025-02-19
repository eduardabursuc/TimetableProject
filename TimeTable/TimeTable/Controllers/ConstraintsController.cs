﻿using Application.DTOs;
using Application.UseCases.Commands.ConstraintCommands;
using Application.UseCases.Queries.ConstraintQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeTable.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConstraintsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "admin,professor")]
        [HttpPost]
        public async Task<IActionResult> CreateConstraint([FromBody] CreateConstraintCommand command)
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
        
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ConstraintDto>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetConstraintByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ConstraintDto>>> GetAll(Guid timetableId)
        {
            var result = await mediator.Send(new GetAllConstraintsQuery{ TimetableId = timetableId });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        
        [Authorize(Roles = "admin,professor")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteConstraint(Guid id)
        {
            var result = await mediator.Send(new DeleteConstraintCommand(id));

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound(result.ErrorMessage);
        }
        
        [HttpGet("forProfessor")]
        public async Task<ActionResult<List<ConstraintDto>>> GetAllForProfessor(string professorEmail, Guid timetableId)
        {
            var result = await mediator.Send(new GetAllForProfessorQuery { ProfessorEmail = professorEmail, TimetableId = timetableId });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}