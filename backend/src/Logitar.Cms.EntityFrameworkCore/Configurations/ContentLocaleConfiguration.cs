using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class ContentLocaleConfiguration : IEntityTypeConfiguration<ContentLocaleEntity>
{
  public void Configure(EntityTypeBuilder<ContentLocaleEntity> builder)
  {
    builder.ToTable(nameof(CmsContext.ContentLocales));
    builder.HasKey(x => x.ContentLocaleId);

    builder.HasIndex(x => new { x.ContentItemId, x.LanguageId }).IsUnique();
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.ContentTypeId, x.LanguageId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.ContentItem).WithMany(x => x.ContentLocales).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.ContentLocales).OnDelete(DeleteBehavior.Restrict);
  }
}
