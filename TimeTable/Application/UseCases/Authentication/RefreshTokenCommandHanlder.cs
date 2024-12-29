using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.Authentication;

public class RefreshTokenCommandHanlder(IUserRepository userRepository) : IRequestHandler<RefreshTokenCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await userRepository.GetToken(request.Email);
        return !token.IsSuccess ? Result<string>.Failure(token.ErrorMessage) : token;
    }
}