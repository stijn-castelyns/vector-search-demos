using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSearchDemos.Core.Entities;

namespace VectorSearchDemos.Infra.EntityConfigurations;
internal class ShoeConfiguration : IEntityTypeConfiguration<Shoe>
{
  public void Configure(EntityTypeBuilder<Shoe> shoe)
  {
    shoe.HasKey(s => s.Id);

    shoe.Property(s => s.Name)
        .IsRequired()
        .HasMaxLength(100);
    
    shoe.Property(s => s.Description)
        .IsRequired()
        .HasMaxLength(500);

    shoe.Property(s => s.DescriptionVector)
        .HasColumnType("vector(1536)");

    shoe.Property(s => s.ImageVector)
        .HasColumnType("vector(1024)");
  }
}
