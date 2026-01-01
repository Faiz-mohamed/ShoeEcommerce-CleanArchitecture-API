using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeEcommerce.Application.Features.Users.Commands.BlockUser;
using ShoeEcommerce.Application.Features.Users.DTOs;
using ShoeEcommerce.Application.Features.Users.Queries.GetAllUsers;
using ShoeEcommerce.Application.Features.Users.Queries.GetUserById;

namespace ShoeEcommerce.API.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly GetAllUsersQueryHandler _getAllHandler;
    private readonly GetUserByIdQueryHandler _getByIdHandler;
    private readonly BlockUserCommandHandler _blockHandler;

    public UsersController(
        GetAllUsersQueryHandler getAllHandler,
        GetUserByIdQueryHandler getByIdHandler,
        BlockUserCommandHandler blockHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _blockHandler = blockHandler;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _getAllHandler.Handle(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _getByIdHandler.Handle(new GetUserByIdQuery { UserId = id });
        return Ok(user);
    }

    [HttpPost("block")]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest request)
    {
        var result = await _blockHandler.Handle(request);
        return Ok(new { Message = result });
    }
}