using Logitar.Cms.Core.Fields;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Identity.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.Infrastructure.Configurations;

internal class FieldDefinitionConfiguration : IEntityTypeConfiguration<FieldDefinitionEntity>
{
  public void Configure(EntityTypeBuilder<FieldDefinitionEntity> builder)
  {
    builder.ToTable(CmsDb.FieldDefinitions.Table.Table ?? string.Empty, CmsDb.FieldDefinitions.Table.Schema);
    builder.HasKey(x => x.FieldDefinitionId);

    builder.HasIndex(x => new { x.ContentTypeId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.ContentTypeId, x.Order }).IsUnique();
    builder.HasIndex(x => new { x.ContentTypeId, x.UniqueNameNormalized }).IsUnique();

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Placeholder).HasMaxLength(Placeholder.MaximumLength);

    builder.HasOne(x => x.ContentType).WithMany(x => x.Fields)
      .HasPrincipalKey(x => x.ContentTypeId).HasForeignKey(x => x.ContentTypeId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldType).WithMany(x => x.FieldDefinitions)
      .HasPrincipalKey(x => x.FieldTypeId).HasForeignKey(x => x.FieldTypeId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
