using Application.Services;
using Application.UseCases.Commands.TimetableCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.CommandHandlers.TimetableCommandHandlers
{
    public class CreateTimetableCommandHandler : IRequestHandler<CreateTimetableCommand, Result<Guid>>
    {
        private readonly ITimetableRepository repository;
        private readonly IMapper mapper;
        private readonly Instance instance;
        private readonly IGroupRepository groupRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IConstraintRepository constraintRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IProfessorRepository professorRepository;

        public CreateTimetableCommandHandler(
            ITimetableRepository repository,
            IMapper mapper,
            Instance instance,
            IGroupRepository groupRepository,
            ICourseRepository courseRepository,
            IConstraintRepository constraintRepository,
            IRoomRepository roomRepository,
            IProfessorRepository professorRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.instance = instance;
            this.groupRepository = groupRepository;
            this.courseRepository = courseRepository;
            this.constraintRepository = constraintRepository;
            this.roomRepository = roomRepository;
            this.professorRepository = professorRepository;
        }

        public async Task<Result<Guid>> Handle(CreateTimetableCommand request, CancellationToken cancellationToken)
        {
            instance.Timeslots = request.Timeslots;
            instance.Events = request.Events;
            Console.WriteLine("Creating timetable");

            var timetableGeneratorService = new TimetableGeneratorService(
                request.UserEmail,
                instance,
                roomRepository,
                groupRepository,
                courseRepository,
                constraintRepository,
                professorRepository
            );

            try
            {
                Console.WriteLine("create");
                var timetable = await timetableGeneratorService.GenerateBestTimetableAsync();
                timetable.CreatedAt = DateTime.Now.ToUniversalTime();
                timetable.Name = request.Name;
                timetable.UserEmail = request.UserEmail;
                await repository.AddAsync(timetable);
                return Result<Guid>.Success(timetable.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
    }
}
