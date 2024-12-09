using Domain.Common;
using MediatR;

namespace Application.UseCases.Authentication
{
    public class RegisterUserCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string AccountType { get; set; }
    }
}