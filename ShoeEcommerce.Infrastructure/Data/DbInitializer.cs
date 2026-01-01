using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Domain.Entities;
using System.Text.Json;

namespace ShoeEcommerce.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var passwordHasher = services.GetRequiredService<IPasswordHasher>();
                var configuration = services.GetRequiredService<IConfiguration>();

                // 1. Migrate Database
                if (context.Database.IsSqlServer())
                {
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying migrations...");
                        await context.Database.MigrateAsync();
                    }
                }

                // 2. Seed Identity (Roles & Admin)
                await SeedIdentityAsync(context, passwordHasher, configuration, logger);

                // 3. Seed Catalog (Brands, Categories, Products)
                await SeedCatalogAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        private static async Task SeedIdentityAsync(AppDbContext context, IPasswordHasher passwordHasher, IConfiguration configuration, ILogger logger)
        {
            if (!await context.Roles.AnyAsync())
            {
                logger.LogInformation("Seeding Roles...");
                var roles = new[]
                {
                    new Role { Id = Guid.NewGuid(), Name = "admin", Description = "Administrator", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Role { Id = Guid.NewGuid(), Name = "customer", Description = "Regular Customer", IsActive = true, CreatedAt = DateTime.UtcNow }
                };
                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            var adminEmail = configuration["AdminSettings:Email"];
            if (!string.IsNullOrEmpty(adminEmail) && !await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                logger.LogInformation("Seeding Admin User...");
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "admin");
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpperInvariant(),
                    FullName = configuration["AdminSettings:FullName"] ?? "Admin",
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = passwordHasher.HashPassword(configuration["AdminSettings:Password"] ?? "Admin123!"),
                    RoleId = adminRole.Id,
                    EmailConfirmed = true
                };
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedCatalogAsync(AppDbContext context, ILogger logger)
        {
            // A. Seed Categories (Using your specific GUIDs)
            if (!await context.Categories.AnyAsync())
            {
                logger.LogInformation("Seeding Categories...");
                var categories = new[]
                {
                    new Category { Id = Guid.Parse("438BDC8B-FE0E-45A2-8EB1-78E798F09C77"), Name = "Men", Slug = "men", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Category { Id = Guid.Parse("456980D6-0459-4B61-BB80-E1900A6117DB"), Name = "Women", Slug = "women", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Category { Id = Guid.Parse("D4031E51-34E1-4208-BC5A-7C6054CF5DA5"), Name = "Kids", Slug = "kids", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Category { Id = Guid.Parse("3BEAAF35-3FF1-4235-A788-1AA73CE8721E"), Name = "Sports", Slug = "sports", IsActive = true, CreatedAt = DateTime.UtcNow }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // B. Seed Brands (Using your specific GUIDs)
            if (!await context.Brands.AnyAsync())
            {
                logger.LogInformation("Seeding Brands...");
                var brands = new[]
                {
                    new Brand { Id = Guid.Parse("422f3469-d784-4943-9b9d-c3dd85832f7a"), Name = "Nike", Slug = "nike", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("fd9d5c92-22eb-4f09-aef6-0e7de9cae2fd"), Name = "Adidas", Slug = "adidas", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("d5d66478-a017-416c-ac06-da7b1242a6ab"), Name = "Puma", Slug = "puma", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("b8cf792f-601e-4411-827c-b628cf2188ed"), Name = "Reebok", Slug = "reebok", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("d22a58e9-b7bb-451c-b44b-6b93d71a2ec3"), Name = "New Balance", Slug = "new-balance", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("d9bb6f8f-b11c-4694-b056-76ae0917fd15"), Name = "ASICS", Slug = "asics", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("757dc97c-c878-48c6-9fb6-378863a3a9be"), Name = "Skechers", Slug = "skechers", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("f9d1c8bd-f905-4c1b-a843-8fb89152185d"), Name = "Vans", Slug = "vans", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("4077c99c-324d-4c53-9751-6a6e9a39db9c"), Name = "Converse", Slug = "converse", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Brand { Id = Guid.Parse("fbdb76df-24d6-4f7f-980a-db2b996edb6b"), Name = "Under Armour", Slug = "under-armour", IsActive = true, CreatedAt = DateTime.UtcNow }
                };
                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }

            // C. Seed Products (Only if empty)
            if (!await context.Products.AnyAsync())
            {
                logger.LogInformation("Seeding 100 Dummy Products...");
                var brands = await context.Brands.ToListAsync();
                var categories = await context.Categories.ToListAsync();
                var random = new Random();

                var modelNames = new[] { "Air Zoom", "Ultra Boost", "Classic", "Pegasus", "Free Run", "NMD", "Yeezy", "Cortez", "Blazer", "Dunk", "Huarache", "Presto", "VaporMax", "React" };
                var adjectives = new[] { "Pro", "Elite", "Max", "Ultra", "Lite", "Prime", "Original", "Retro", "Modern", "Sport" };
                var colors = new[] { "Black", "White", "Red", "Blue", "Green", "Yellow", "Orange", "Grey" };
                var sizes = new[] { "7", "8", "9", "10", "11", "12" };

                var productsToAdd = new List<Product>();

                for (int i = 0; i < 100; i++)
                {
                    // 1. Pick Random Brand & Category
                    var brand = brands[random.Next(brands.Count)];
                    var category = categories[random.Next(categories.Count)];

                    var model = modelNames[random.Next(modelNames.Length)];
                    var adj = adjectives[random.Next(adjectives.Length)];

                    var name = $"{brand.Name} {model} {adj}";
                    var productId = Guid.NewGuid();

                    // 2. Create Product
                    var product = new Product
                    {
                        Id = productId,
                        Name = name,
                        Slug = $"{name.ToLower().Replace(" ", "-")}-{Guid.NewGuid().ToString()[..8]}",
                        Description = $"The {name} offers superior comfort and style. Perfect for {category.Name} who value performance.",
                        Status = true, // Matches your Product.cs
                        BrandId = brand.Id,
                        CreatedAt = DateTime.UtcNow,
                        MainImageUrl = $"https://placehold.co/600x400?text={name.Replace(" ", "+")}",

                        // Link Category
                        ProductCategories = new List<ProductCategory>
                        {
                            new ProductCategory { ProductId = productId, CategoryId = category.Id }
                        },
                        Variants = new List<ProductVariants>()
                    };

                    // 3. Create Variants (3 to 10 per product)
                    int variantCount = random.Next(3, 11);
                    for (int v = 0; v < variantCount; v++)
                    {
                        var size = sizes[random.Next(sizes.Length)];
                        var color = colors[random.Next(colors.Length)];
                        var variantId = Guid.NewGuid();
                        var price = random.Next(50, 200) + (random.Next(0, 99) * 0.01m);

                        var variant = new ProductVariants
                        {
                            Id = variantId,
                            ProductId = productId,
                            Sku = $"{brand.Name.Substring(0, 3).ToUpper()}-{model.Substring(0, 3).ToUpper()}-{random.Next(1000, 9999)}",
                            Size = size,
                            Colour = color, // Matches your ProductVariants.cs
                            Price = price,
                            Weight = (decimal)(random.NextDouble() * 1.5),
                            InventoryQty = random.Next(10, 100),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        // 4. Add Images to Variant
                        // Note: We add it to the Variant's collection directly
                        variant.ProductImages.Add(new ProductImages
                        {
                            Id = Guid.NewGuid(),
                            ProductId = productId,
                            VariantId = variantId,
                            ImagesJson = JsonSerializer.Serialize(new List<string>
                            {
                                $"https://placehold.co/600x400?text={name.Replace(" ", "+")}+{color}+1",
                                $"https://placehold.co/600x400?text={name.Replace(" ", "+")}+{color}+2"
                            }),
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        });

                        product.Variants.Add(variant);
                    }

                    productsToAdd.Add(product);
                }

                await context.Products.AddRangeAsync(productsToAdd);
                await context.SaveChangesAsync();
                logger.LogInformation("Catalog seeding completed: 100 Products added.");
            }
        }
    }
}