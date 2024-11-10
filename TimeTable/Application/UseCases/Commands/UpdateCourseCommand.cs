using Application.UseCases.Commands;
using Domain.Common;
using MediatR;
using System;

namespace Application.UseCases.Commands
{
    public class UpdateCourseCommand : CreateCourseCommand, IRequest<Result<string>>
    {

    }
}