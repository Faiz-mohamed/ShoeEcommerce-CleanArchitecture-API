using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShoeEcommerce.Application.Common.Interfaces.Services;

namespace ShoeEcommerce.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? IpAddress => GetIpAddress();

        private string GetIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "Unknown";

            // 1. Check X-Forwarded-For (Standard for Load Balancers/Proxies like Nginx/Cloudflare)
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                // The first IP is the original client
                return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "Unknown";
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}