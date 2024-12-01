using Application.Services;
using Application.UseCases.Commands.TimetableCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class CreateTimetableCommandHandler(ITimetableRepository repository, IMapper mapper, Instance instance)
        : IRequestHandler<CreateTimetableCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateTimetableCommand request, CancellationToken cancellationToken)
        {
            var arcConsistency = new ArcConsistency(instance);
            if(arcConsistency.ApplyArcConsistencyAndBacktracking(out var solution))
            {
                var timetable = arcConsistency.GetTimetable(solution);
                await repository.AddAsync(timetable);
                return Result<Guid>.Success(timetable.Id);
            } else {
                return Result<Guid>.Failure("No solution found");
            }
        }
    }
}