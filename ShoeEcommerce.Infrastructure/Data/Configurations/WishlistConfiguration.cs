using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoeEcommerce.Domain.Entities;

namespace ShoeEcommerce.Infrastructure.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasIndex(w => w.UserId).IsUnique();

            builder.HasMany(w => w.Items)
                   .WithOne(i => i.Wishlist)
                   .HasForeignKey(i => i.WishlistId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}