using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class ArchetypeConfiguration : AggregateConfiguration<ArchetypeEntity>, IEntityTypeConfiguration<ArchetypeEntity>
{
  public override void Configure(EntityTypeBuilder<ArchetypeEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(CmsContext.Archetypes));
    builder.HasKey(x => x.ArchetypeId);

    builder.HasIndex(x => x.IsInvariant);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(IdentifierValidator.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(IdentifierValidator.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
  }
}
