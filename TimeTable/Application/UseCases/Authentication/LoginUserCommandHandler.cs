using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.Authentication;

public class LoginUserCommandHandler(IUserRepository userRepository) : IRequestHandler<LoginUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = request.Email,
            Password = request.Password
        };
        var token = await userRepository.Login(user);
        return !token.IsSuccess ? Result<string>.Failure(token.ErrorMessage) : token;
    }
}