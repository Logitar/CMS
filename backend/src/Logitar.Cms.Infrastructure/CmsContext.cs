using Logitar.Cms.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure;

public class CmsContext : DbContext
{
  public const string Schema = "Cms";

  public CmsContext(DbContextOptions<CmsContext> options) : base(options)
  {
  }

  public DbSet<ContentEntity> Contents => Set<ContentEntity>();
  public DbSet<ContentLocaleEntity> ContentLocales => Set<ContentLocaleEntity>();
  public DbSet<ContentTypeEntity> ContentTypes => Set<ContentTypeEntity>();
  public DbSet<FieldDefinitionEntity> FieldDefinitions => Set<FieldDefinitionEntity>();
  public DbSet<FieldIndexEntity> FieldIndex => Set<FieldIndexEntity>();
  public DbSet<FieldTypeEntity> FieldTypes => Set<FieldTypeEntity>();
  public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();
  public DbSet<PublishedContentEntity> PublishedContents => Set<PublishedContentEntity>();
  public DbSet<UniqueIndexEntity> UniqueIndex => Set<UniqueIndexEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
