using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceСoffeeRoom.Domain;

namespace ServiceCoffeeRoom.Infrastructure.Entityframework.Configuration
{
    public class BeansConfiguration : IEntityTypeConfiguration<Beans>
    {
        public void Configure(EntityTypeBuilder<Beans> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TimeHappened).IsRequired().HasConversion
            (
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            );
            builder.Property(x => x.Mark).IsRequired().HasMaxLength(32);
        }
    }
}
