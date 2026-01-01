namespace ShoeEcommerce.Application.Features.Product.Queries.GetProductByVariant
{
    public class GetProductByVariantQuery
    {
        public Guid VariantId { get; set; }

        public GetProductByVariantQuery(Guid variantId)
        {
            VariantId = variantId;
        }
    }
}