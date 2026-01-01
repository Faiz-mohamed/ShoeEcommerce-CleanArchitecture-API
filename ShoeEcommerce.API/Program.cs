using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoeEcommerce.API.Middleware;
using ShoeEcommerce.Application.Common.Interfaces.Repositories;
using ShoeEcommerce.Application.Common.Interfaces.Services;
using ShoeEcommerce.Application.Features.Authentication.Commands.ChangePassword;
using ShoeEcommerce.Application.Features.Authentication.Commands.Login;
using ShoeEcommerce.Application.Features.Authentication.Commands.Logout;
using ShoeEcommerce.Application.Features.Authentication.Commands.RefreshToken;
using ShoeEcommerce.Application.Features.Authentication.Commands.Register;
using ShoeEcommerce.Application.Features.Authentication.Validators;
using ShoeEcommerce.Application.Features.Brands.Commands.CreateBrand;
using ShoeEcommerce.Application.Features.Brands.DTOs;
using ShoeEcommerce.Application.Features.Brands.Validators;
using ShoeEcommerce.Application.Features.Cart.Commands.AddToCart;
using ShoeEcommerce.Application.Features.Cart.DTOs;
using ShoeEcommerce.Application.Features.Cart.Queries.GetCart;
using ShoeEcommerce.Application.Features.Cart.Validators;
using ShoeEcommerce.Application.Features.Categories.Commands.CreateCategory;
using ShoeEcommerce.Application.Features.Categories.DTOs;
using ShoeEcommerce.Application.Features.Categories.Validators;
using ShoeEcommerce.Application.Features.Orders.Commands.CreateOrder;
using ShoeEcommerce.Application.Features.Orders.Commands.VerifyPayment;
using ShoeEcommerce.Application.Features.Orders.DTOs;
using ShoeEcommerce.Application.Features.Orders.Validators;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductById;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductByVariant;
using ShoeEcommerce.Application.Features.Product.Queries.GetProductsPaged;
using ShoeEcommerce.Application.Features.Products.Commands.CreateProduct;
using ShoeEcommerce.Application.Features.Products.DTOs;
using ShoeEcommerce.Application.Features.Products.Validators;
using ShoeEcommerce.Application.Features.Users.Commands.BlockUser;
using ShoeEcommerce.Application.Features.Users.DTOs;
using ShoeEcommerce.Application.Features.Users.Queries.GetAllUsers;
using ShoeEcommerce.Application.Features.Users.Queries.GetUserById;
using ShoeEcommerce.Application.Features.Users.Validators;
using ShoeEcommerce.Application.Features.Wishlist.Commands.ToggleWishlist;
using ShoeEcommerce.Application.Features.Wishlist.Queries.GetWishlist;
using ShoeEcommerce.Application.Interfaces.Repositories;
using ShoeEcommerce.Infrastructure.Data;
using ShoeEcommerce.Infrastructure.Repositories;
using ShoeEcommerce.Infrastructure.Services;
using System.Text;

namespace ShoeEcommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =================================================================
            // 1. DATABASE & CORE INFRASTRUCTURE
            // =================================================================
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHttpContextAccessor();

            // Core Services
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IPaymentService, RazorpayPaymentService>();

            // Background Workers
            builder.Services.AddHostedService<RefreshTokenCleanupService>();

            // =================================================================
            // 2. REPOSITORIES (Data Access)
            // =================================================================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

            // =================================================================
            // 3. APPLICATION FEATURES (Handlers & Validators)
            // =================================================================

            // --- Authentication Feature ---
            builder.Services.AddScoped<RegisterCommandHandler>();
            builder.Services.AddScoped<LoginCommandHandler>();
            builder.Services.AddScoped<RefreshTokenCommandHandler>();
            builder.Services.AddScoped<LogoutCommandHandler>();
            builder.Services.AddScoped<ChangePasswordCommandHandler>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            // --- User Management (Admin) ---
            builder.Services.AddScoped<GetAllUsersQueryHandler>();
            builder.Services.AddScoped<GetUserByIdQueryHandler>();
            builder.Services.AddScoped<BlockUserCommandHandler>();
            builder.Services.AddScoped<IValidator<BlockUserRequest>, BlockUserValidator>();

            // --- Product Feature ---
            builder.Services.AddScoped<GetProductByIdQueryHandler>();
            builder.Services.AddScoped<GetProductsPagedQueryHandler>();
            builder.Services.AddScoped<GetProductByVariantQueryHandler>();
            builder.Services.AddScoped<CreateProductCommandHandler>();
            builder.Services.AddScoped<IValidator<CreateProductRequest>, CreateProductValidator>();

            // --- Brand Feature ---
            builder.Services.AddScoped<CreateBrandCommandHandler>();
            builder.Services.AddScoped<IValidator<CreateBrandRequest>, CreateBrandValidator>();

            // --- Category Feature ---
            builder.Services.AddScoped<CreateCategoryCommandHandler>();
            builder.Services.AddScoped<IValidator<CreateCategoryRequest>, CreateCategoryValidator>();

            // --- Cart Feature ---
            builder.Services.AddScoped<AddToCartCommandHandler>();
            builder.Services.AddScoped<IValidator<AddToCartRequest>, AddToCartValidator>();
            builder.Services.AddScoped<GetCartQueryHandler>();

            // --- Wishlist Feature ---
            builder.Services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderValidator>();
            builder.Services.AddScoped<ToggleWishlistCommandHandler>();
            builder.Services.AddScoped<GetWishlistQueryHandler>();

            // --- Order Feature ---
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<CreateOrderCommandHandler>();
            builder.Services.AddScoped<VerifyPaymentCommandHandler>();
            builder.Services.AddScoped<IValidator<VerifyPaymentRequest>, VerifyPaymentValidator>();

            // Fluent Validation Auto-Wireup (Legacy)
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            // =================================================================
            // 4. API SECURITY (JWT & CORS)
            // =================================================================
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is missing in appsettings.json");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.FromSeconds(20)
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // =================================================================
            // 5. API DOCUMENTATION (Swagger)
            // =================================================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Shoe Ecommerce API",
                    Version = "v1",
                    Description = "Clean Architecture ASP.NET Core Web API"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // =================================================================
            // 6. PIPELINE CONFIGURATION
            // =================================================================
            var app = builder.Build();

            // Seeder
            await DbInitializer.InitializeAsync(app.Services);

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shoe Ecommerce API v1");
                options.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}