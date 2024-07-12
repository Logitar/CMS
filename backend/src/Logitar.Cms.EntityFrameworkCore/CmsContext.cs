using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore;

public class CmsContext : DbContext
{
  public CmsContext(DbContextOptions<CmsContext> options) : base(options)
  {
  }

  internal DbSet<ContentItemEntity> ContentItems { get; private set; }
  internal DbSet<ContentLocaleEntity> ContentLocales { get; private set; }
  internal DbSet<ContentTypeEntity> ContentTypes { get; private set; }
  internal DbSet<FieldTypeEntity> FieldTypes { get; private set; }
  internal DbSet<LanguageEntity> Languages { get; private set; }
  internal DbSet<LogEventEntity> LogEvents { get; private set; }
  internal DbSet<LogExceptionEntity> LogExceptions { get; private set; }
  internal DbSet<LogEntity> Logs { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
