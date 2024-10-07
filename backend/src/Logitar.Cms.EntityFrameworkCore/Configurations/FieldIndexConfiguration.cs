using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal abstract class FieldIndexConfiguration<TEntity, TValue> where TEntity : FieldIndexEntity<TValue>
{
  public virtual void Configure(EntityTypeBuilder<TEntity> builder)
  {
    builder.HasKey(x => x.FieldIndexId);

    builder.HasIndex(x => x.ContentTypeId);
    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => x.ContentTypeName);
    builder.HasIndex(x => x.FieldTypeId);
    builder.HasIndex(x => x.FieldTypeUid);
    builder.HasIndex(x => x.FieldTypeName);
    builder.HasIndex(x => new { x.FieldDefinitionId, x.ContentLocaleId }).IsUnique();
    builder.HasIndex(x => x.FieldDefinitionUid);
    builder.HasIndex(x => x.FieldDefinitionName);
    builder.HasIndex(x => x.ContentItemId);
    builder.HasIndex(x => x.ContentItemUid);
    builder.HasIndex(x => x.ContentLocaleId);
    builder.HasIndex(x => x.ContentLocaleUid);
    builder.HasIndex(x => x.ContentLocaleName);
    builder.HasIndex(x => x.LanguageId);
    builder.HasIndex(x => x.LanguageUid);
    builder.HasIndex(x => x.LanguageCode);
    builder.HasIndex(x => x.Value);

    builder.Property(x => x.ContentTypeName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.FieldTypeName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.FieldDefinitionName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.ContentLocaleName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.LanguageCode).HasMaxLength(LocaleUnit.MaximumLength);
  }
}
