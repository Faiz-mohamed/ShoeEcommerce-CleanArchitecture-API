using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Features.Users.DTOs;

namespace ShoeEcommerce.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery { }

public class GetAllUsersQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery query)
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber ?? "N/A",
            RoleName = u.Role.Name,
            IsBlocked = u.UserBlocks.Any(b => b.IsActive),
            CreatedAt = u.CreatedAt
        }).ToList();
    }
}