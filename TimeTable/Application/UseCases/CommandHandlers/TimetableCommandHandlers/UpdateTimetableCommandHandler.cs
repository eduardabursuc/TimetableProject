using Application.Services;
using Application.UseCases.Commands.TimetableCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class UpdateTimetableCommandHandler(ITimetableRepository repository, IMapper mapper)
            : IRequestHandler<UpdateTimetableCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateTimetableCommand request, CancellationToken cancellationToken)
        {
            var timetable = mapper.Map<Timetable>(request);
            var result = await repository.UpdateAsync(timetable);
            return result.IsSuccess ? Result<Guid>.Success(result.Data) : Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
