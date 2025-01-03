using Logitar.Cms.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure;

public class CmsContext : DbContext
{
  public CmsContext(DbContextOptions<CmsContext> options) : base(options)
  {
  }

  public DbSet<ContentEntity> Contents => Set<ContentEntity>();
  public DbSet<ContentLocaleEntity> ContentLocales => Set<ContentLocaleEntity>();
  public DbSet<ContentTypeEntity> ContentTypes => Set<ContentTypeEntity>();
  public DbSet<FieldDefinitionEntity> FieldDefinitions => Set<FieldDefinitionEntity>();
  public DbSet<FieldTypeEntity> FieldTypes => Set<FieldTypeEntity>();
  public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
