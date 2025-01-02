using Logitar.Cms.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure;

public class CmsContext : DbContext
{
  public CmsContext(DbContextOptions<CmsContext> options) : base(options)
  {
  }

  public DbSet<FieldTypeEntity> FieldTypes => Set<FieldTypeEntity>();
  public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
