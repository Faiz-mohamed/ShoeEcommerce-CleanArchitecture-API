using FluentValidation;
using ShoeEcommerce.Application.Common.Exceptions;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Users.DTOs;
using ShoeEcommerce.Domain.Entities;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Users.Commands.BlockUser
{
    public class BlockUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<BlockUserRequest> _validator;

        public BlockUserCommandHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IValidator<BlockUserRequest> validator)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<string> Handle(BlockUserRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(k => k.Key, v => v.ToArray());
                throw new ValidationException(errors);
            }

            var adminIdString = _currentUserService.UserId;
            if (!Guid.TryParse(adminIdString, out var adminId))
            {
                throw new UnauthorizedAccessException("Admin Access Only");
            }

            var targetUser = await _userRepository.GetUserByIdAsync(request.UserId);
            if (targetUser == null) throw new NotFoundException("User", request.UserId);

            if (targetUser.Id == adminId)
            {
                throw new Exception("You cannot block yourself.");
            }

            DateTime? expiresAt = null;
            if (request.ExpiresInDays.HasValue)
            {
                expiresAt = DateTime.UtcNow.AddDays(request.ExpiresInDays.Value);
            }

            var block = new UserBlock
            {
                UserId = request.UserId,
                AdminId = adminId,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            await _userRepository.AddUserBlockAsync(block);

            var durationText = request.ExpiresInDays.HasValue
                ? $"for {request.ExpiresInDays.Value} days"
                : "permanently";

            return $"User {targetUser.Email} has been blocked {durationText}.";
        }
    }
}