using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceСoffeeRoom.Domain;

namespace ServiceCoffeeRoom.Infrastructure.Entityframework.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
            builder.HasOne(x => x.Admin).WithMany();
            builder.HasOne(x => x.CoffeeMachine).WithMany();
            builder.Navigation(x => x.Admin).AutoInclude();
            builder.Navigation(x => x.CoffeeMachine).AutoInclude();
        }
    }
}
