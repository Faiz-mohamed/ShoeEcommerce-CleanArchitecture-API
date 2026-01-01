namespace ShoeEcommerce.Application.Common.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> CreateOrderAsync(decimal amount, string currency, string receiptId);
        bool VerifyPaymentSignature(string orderId, string paymentId, string signature);
    }
}