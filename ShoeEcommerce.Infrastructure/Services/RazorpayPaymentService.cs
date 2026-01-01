using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using ShoeEcommerce.Application.Common.Interfaces.Services;

namespace ShoeEcommerce.Infrastructure.Services
{
    public class RazorpayPaymentService : IPaymentService
    {
        private readonly string _keyId;
        private readonly string _keySecret;

        public RazorpayPaymentService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"]
                     ?? throw new ArgumentNullException("Razorpay KeyId is missing");
            _keySecret = configuration["Razorpay:KeySecret"]
                         ?? throw new ArgumentNullException("Razorpay KeySecret is missing");
        }

        public Task<string> CreateOrderAsync(decimal amount, string currency, string receiptId)
        {
            var client = new RazorpayClient(_keyId, _keySecret);

            var amountInPaise = (long)(amount * 100);

            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", currency },
                { "receipt", receiptId }, // internal Order ID
                { "payment_capture", 1 }
            };

            Razorpay.Api.Order order = client.Order.Create(options);

            return Task.FromResult(order["id"].ToString());
        }

        public bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
        {
            var client = new RazorpayClient(_keyId, _keySecret);

            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", orderId },
                { "razorpay_payment_id", paymentId },
                { "razorpay_signature", signature }
            };

            try
            {
                // This method throws an exception if verification fails
                Utils.verifyPaymentSignature(attributes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}