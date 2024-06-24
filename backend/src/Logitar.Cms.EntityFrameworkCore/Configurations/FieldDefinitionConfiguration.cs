using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class FieldDefinitionConfiguration : IEntityTypeConfiguration<FieldDefinitionEntity>
{
  public void Configure(EntityTypeBuilder<FieldDefinitionEntity> builder)
  {
    builder.ToTable(nameof(CmsContext.FieldDefinitions));
    builder.HasKey(x => x.FieldDefinitionId);

    builder.HasIndex(x => new { x.ContentTypeId, x.Id }).IsUnique();
    builder.HasIndex(x => x.ContentTypeName);
    builder.HasIndex(x => new { x.ContentTypeId, x.Order }).IsUnique();
    builder.HasIndex(x => x.FieldTypeName);
    builder.HasIndex(x => x.FieldDataType);
    builder.HasIndex(x => x.IsInvariant);
    builder.HasIndex(x => x.IsRequired);
    builder.HasIndex(x => x.IsIndexed);
    builder.HasIndex(x => x.IsUnique);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.ContentTypeId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Placeholder);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.ContentTypeName).HasMaxLength(IdentifierValidator.MaximumLength);
    builder.Property(x => x.FieldTypeName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.FieldDataType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<DataType>());
    builder.Property(x => x.UniqueName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Placeholder).HasMaxLength(PlaceholderUnit.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.ContentType).WithMany(x => x.FieldDefinitions).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.FieldType).WithMany(x => x.FieldDefinitions).OnDelete(DeleteBehavior.Restrict);
  }
}
