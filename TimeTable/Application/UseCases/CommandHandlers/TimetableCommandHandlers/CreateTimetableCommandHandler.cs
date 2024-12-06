using Application.Services;
using Application.UseCases.Commands.TimetableCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class CreateTimetableCommandHandler(ITimetableRepository repository, IMapper mapper, Instance instance, IGroupRepository groupRepository, IProfessorRepository professorRepository, ICourseRepository courseRepository, IConstraintRepository constraintRepository, IRoomRepository roomRepository)
        : IRequestHandler<CreateTimetableCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateTimetableCommand request, CancellationToken cancellationToken)
        {
            instance.Timeslots = request.Timeslots;
            instance.Events = request.Events;
            var timetableGenerator = new TimetableGenerator(instance, roomRepository, groupRepository, professorRepository, courseRepository, constraintRepository);
            try
            {
                var timetable = timetableGenerator.GenerateBestTimetable(out var solution);
                await repository.AddAsync(timetable);
                return Result<Guid>.Success(timetable.Id);
            } catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
    }
}