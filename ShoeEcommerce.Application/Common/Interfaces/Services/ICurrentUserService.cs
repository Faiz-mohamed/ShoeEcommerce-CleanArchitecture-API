namespace ShoeEcommerce.Application.Common.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? IpAddress { get; }
    }
}