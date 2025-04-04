using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSearchDemos.Core;
using VectorSearchDemos.Core.Entities;

namespace VectorSearchDemos.Infra;
public class DemoDbContext : DbContext
{
  public DbSet<Shoe> Shoes { get; set; } = null!;

  public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
  {
  }

  public DemoDbContext()
  {
    
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoDbContext).Assembly);
  }
}

