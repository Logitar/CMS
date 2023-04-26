using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL;

public class CmsContext : DbContext
{
  public CmsContext(DbContextOptions<CmsContext> options) : base(options)
  {
  }

  internal DbSet<LanguageEntity> Languages { get; private set; } = null!;
  internal DbSet<UserEntity> Users { get; private set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(CmsContext).Assembly);
    modelBuilder.HasDefaultSchema("cms");
  }
}
