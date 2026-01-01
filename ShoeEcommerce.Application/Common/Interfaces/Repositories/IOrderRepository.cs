using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Application.Common.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order?> GetByRazorpayOrderIdAsync(string razorpayOrderId);
        Task UpdateAsync(Order order);
    }
}