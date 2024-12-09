using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.Authentication
{
    public class RegisterUserCommandHandler(IUserRepository repository)
        : IRequestHandler<RegisterUserCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var usr = repository.GetByEmailAsync(request.Email);
            if (usr.Result.IsSuccess) return Result<string>.Failure("User already exists");
            
            var user = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                AccountType = request.AccountType
            };
    
            await repository.Register(user, cancellationToken);
            return Result<string>.Success(user.Email);
        }
    }
}

