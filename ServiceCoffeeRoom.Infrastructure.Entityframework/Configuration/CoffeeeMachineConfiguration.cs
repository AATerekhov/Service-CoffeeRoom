using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ServiceСoffeeRoom.Domain;

namespace ServiceCoffeeRoom.Infrastructure.Entityframework.Configuration
{
    public class CoffeeeMachineConfiguration : IEntityTypeConfiguration<CoffeeMachine>
    {
        public void Configure(EntityTypeBuilder<CoffeeMachine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
            builder.HasOne(x => x.Beans).WithMany();
            builder.Navigation(x => x.Beans).AutoInclude();
        }
    }
}
