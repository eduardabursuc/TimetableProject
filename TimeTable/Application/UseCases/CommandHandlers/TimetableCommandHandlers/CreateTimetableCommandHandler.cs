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
        : IRequestHandler<CreateTimetableCommand, Result<Timetable>>
    {
        public async Task<Result<Timetable>> Handle(CreateTimetableCommand request, CancellationToken cancellationToken)
        {
            //instance.LoadFromJson("Configuration/config.json");
            var arcConsistency = new ArcConsistency(instance);
            if(arcConsistency.ApplyArcConsistencyAndBacktracking(out var solution))
            {
                var timetable = arcConsistency.GetTimetable(solution);
                await repository.AddAsync(timetable);
                return Result<Timetable>.Success(timetable);
            } else {
                return Result<Timetable>.Failure("No solution found");
            }
        }
    }
}