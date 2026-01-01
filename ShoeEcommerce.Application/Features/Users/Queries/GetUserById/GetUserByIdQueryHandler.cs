using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Features.Users.DTOs;

namespace ShoeEcommerce.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery
{
    public Guid UserId { get; set; }
}

public class GetUserByIdQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery query)
    {
        var u = await _userRepository.GetUserByIdAsync(query.UserId);

        if (u == null) throw new NotFoundException("User", query.UserId);

        return new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber ?? "N/A",
            RoleName = u.Role.Name,
            IsBlocked = u.UserBlocks.Any(b => b.IsActive),
            CreatedAt = u.CreatedAt
        };
    }
}