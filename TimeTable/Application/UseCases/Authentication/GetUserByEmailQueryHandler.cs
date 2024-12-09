using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.UseCases.Authentication;

public class GetUserByIdQueryHandler(IUserRepository repository, IMapper mapper)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByEmailAsync(request.Email);
    
        if (!result.IsSuccess) return Result<UserDto>.Failure(result.ErrorMessage);
    
        var userDto = mapper.Map<UserDto>(result.Data);
        return Result<UserDto>.Success(userDto);
    }
}