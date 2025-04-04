using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSearchDemos.Infra;
public class DemoDbContextFactory : IDesignTimeDbContextFactory<DemoDbContext>
{
  public DemoDbContext CreateDbContext(string[] args)
  {
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddUserSecrets<DemoDbContext>();

    IConfiguration config = configurationBuilder.Build();

    string connectionString = config.GetConnectionString("DemoDb")!;

    DbContextOptionsBuilder<DemoDbContext> dbContextOptionsBuilder = new();

    dbContextOptionsBuilder.UseSqlServer(connectionString, options =>
    {
      options.UseVectorSearch();
    });

    return new DemoDbContext(dbContextOptionsBuilder.Options);
  }
}
