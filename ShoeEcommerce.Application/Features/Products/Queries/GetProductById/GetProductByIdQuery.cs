namespace ShoeEcommerce.Application.Features.Product.Queries.GetProductById
{
    public class GetProductByIdQuery
    {
        public Guid Id { get; set; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}