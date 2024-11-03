using FluentValidation;

namespace Application.UseCases.Commands
{
    public class CreateConstraintCommandValidator : AbstractValidator<CreateConstraintCommand>
    {
        public CreateConstraintCommandValidator()
        {
            RuleFor(c => c.Type)
                .IsInEnum()
                .WithMessage("Constraint type is required.");

            RuleFor(c => c.ProfessorId)
                .NotEmpty()
                .WithMessage("Professor ID is required.");

            RuleFor(c => c.CourseId)
                .NotEmpty()
                .WithMessage("Course ID is required.");

            RuleFor(c => c.RoomId)
                .NotEmpty()
                .WithMessage("Room ID is required.");

            RuleFor(c => c.Timeslots)
                .NotEmpty()
                .WithMessage("At least one timeslot ID is required.");

            RuleFor(c => c.GroupId)
                .NotEmpty()
                .WithMessage("Group ID is required.");

            RuleFor(c => c.Event)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Event is required and must be less than 100 characters.");

            RuleFor(c => c.WantedRoomId)
                .NotEmpty()
                .When(c => c.WantedRoomId.HasValue)
                .WithMessage("Wanted room ID must be specified if provided.");

            RuleFor(c => c.WantedTimeslotId)
                .NotEmpty()
                .When(c => c.WantedTimeslotId.HasValue)
                .WithMessage("Wanted timeslot ID must be specified if provided.");
        }
    }
}
