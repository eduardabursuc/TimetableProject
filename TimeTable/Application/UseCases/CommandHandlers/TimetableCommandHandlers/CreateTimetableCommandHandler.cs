using Application.Services;
using Application.UseCases.Commands.TimetableCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class CreateTimetableCommandHandler(ITimetableRepository repository, Instance instance, IGroupRepository groupRepository, ICourseRepository courseRepository, IConstraintRepository constraintRepository, IRoomRepository roomRepository)
        : IRequestHandler<CreateTimetableCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateTimetableCommand request, CancellationToken cancellationToken)
        {
            instance.Timeslots = request.Timeslots;
            instance.Events = request.Events;
            Console.WriteLine("Creating timetable");
            var timetableGenerator = new TimetableGenerator(request.UserEmail, instance, roomRepository, groupRepository, courseRepository, constraintRepository);
            try
            {
                Console.WriteLine("create");
                var timetable = timetableGenerator.GenerateBestTimetable(out var solution);
                timetable.CreatedAt = DateTime.Now.ToUniversalTime();
                timetable.Name = request.Name;
                timetable.UserEmail = request.UserEmail;
                await repository.AddAsync(timetable);
                return Result<Guid>.Success(timetable.Id);
            } catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
    }
}