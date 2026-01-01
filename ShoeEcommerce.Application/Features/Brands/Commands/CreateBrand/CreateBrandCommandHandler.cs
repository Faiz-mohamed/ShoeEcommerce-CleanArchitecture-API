using FluentValidation;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Features.Brands.DTOs;
using ShoeEcommerce.Domain.Entities;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommandHandler
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IValidator<CreateBrandRequest> _validator;

        public CreateBrandCommandHandler(
            IBrandRepository brandRepository,
            IValidator<CreateBrandRequest> validator)
        {
            _brandRepository = brandRepository;
            _validator = validator;
        }

        public async Task<Guid> Handle(CreateBrandRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failure => failure.Key, failure => failure.ToArray());

                throw new ValidationException(errors);
            }

            if (!await _brandRepository.IsNameUniqueAsync(request.Name))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Name", new[] { $"The brand '{request.Name}' already exists." } }
                };
                throw new ValidationException(errors);
            }

            var slug = request.Name.ToLower().Trim().Replace(" ", "-");

            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Slug = slug,
                Description = request.Description,
                IsActive = request.IsActive,
            };

            await _brandRepository.AddAsync(brand);

            return brand.Id;
        }
    }
}