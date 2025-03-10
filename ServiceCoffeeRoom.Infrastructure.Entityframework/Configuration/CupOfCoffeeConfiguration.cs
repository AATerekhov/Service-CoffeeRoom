using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceСoffeeRoom.Domain.Processes;

namespace ServiceCoffeeRoom.Infrastructure.Entityframework.Configuration
{
    public class CupOfCoffeeConfiguration : IEntityTypeConfiguration<CupOfCoffe>
    {
        public void Configure(EntityTypeBuilder<CupOfCoffe> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TimeHappened).IsRequired().HasConversion
            (
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            );
        }
    }
}
