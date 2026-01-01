using FluentValidation;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Features.Categories.DTOs;
using ShoeEcommerce.Domain.Entities;
using ValidationException = ShoeEcommerce.Application.Common.Exceptions.ValidationException;

namespace ShoeEcommerce.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CreateCategoryRequest> _validator;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IValidator<CreateCategoryRequest> validator)
        {
            _categoryRepository = categoryRepository;
            _validator = validator;
        }

        public async Task<Guid> Handle(CreateCategoryRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failure => failure.Key, failure => failure.ToArray());

                throw new ValidationException(errors);
            }

            if (!await _categoryRepository.IsNameUniqueAsync(request.Name))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Name", new[] { $"The category '{request.Name}' already exists." } }
                };
                throw new ValidationException(errors);
            }

            if (request.ParentCategoryId.HasValue)
            {
                if (!await _categoryRepository.ExistsAsync(request.ParentCategoryId.Value))
                {
                    var errors = new Dictionary<string, string[]>
                    {
                        { "ParentCategoryId", new[] { "The selected parent category does not exist." } }
                    };
                    throw new ValidationException(errors);
                }
            }

            var slug = request.Name.ToLower().Trim().Replace(" ", "-");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Slug = slug,
                Description = request.Description,
                IsActive = request.IsActive,
                ParentCategoryId = request.ParentCategoryId
            };

            await _categoryRepository.AddAsync(category);

            return category.Id;
        }
    }
}