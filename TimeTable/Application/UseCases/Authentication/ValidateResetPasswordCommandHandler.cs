using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.Authentication
{
    public class ValidateResetPasswordCommandHandler(IUserRepository userRepository)
        : IRequestHandler<ValidateResetPasswordCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ValidateResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (!user.IsSuccess)
            {
                return Result<string>.Failure("User not found.");
            }
            
            user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await userRepository.UpdateUserAsync(user.Data);

            return Result<string>.Success("Password reset successfully");
        }
    }
}