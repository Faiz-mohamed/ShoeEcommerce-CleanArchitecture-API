namespace ShoeEcommerce.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,        // Order created in DB, waiting for Razorpay payment
        PaymentFailed,  // Razorpay failed or user cancelled
        Paid,           // Payment Verified successfully
        Processing,     // Warehouse is packing it
        Shipped,        // Handed to courier
        Delivered,      // Customer received it
        Cancelled,      // Admin or User cancelled AFTER payment
        Refunded        // Money returned
    }
}