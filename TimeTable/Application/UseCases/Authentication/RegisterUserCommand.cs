using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication
{
    public class RegisterUserCommand : IRequest<Result<string>>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string AccountType { get; set; }
    }
}