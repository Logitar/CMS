using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class StringFieldIndexConfiguration : IEntityTypeConfiguration<StringFieldIndexEntity>
{
  public void Configure(EntityTypeBuilder<StringFieldIndexEntity> builder) // TODO(fpion): refactor
  {
    builder.ToTable(CmsDb.StringFieldIndex.Table.Table ?? string.Empty, CmsDb.StringFieldIndex.Table.Schema);
    builder.HasKey(x => x.FieldIndexId);

    builder.HasIndex(x => x.ContentTypeId);
    builder.HasIndex(x => x.ContentTypeUid);
    builder.HasIndex(x => x.ContentTypeName);
    builder.HasIndex(x => x.FieldTypeId);
    builder.HasIndex(x => x.FieldTypeUid);
    builder.HasIndex(x => x.FieldTypeName);
    builder.HasIndex(x => x.FieldDefinitionId);
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

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.StringFieldIndex.StringFieldIndexId.Name);
    builder.Property(x => x.ContentTypeName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.FieldTypeName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.FieldDefinitionName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.ContentLocaleName).HasMaxLength(UniqueNameUnit.MaximumLength);
    builder.Property(x => x.LanguageCode).HasMaxLength(LocaleUnit.MaximumLength);
    builder.Property(x => x.Value).HasMaxLength(StringFieldIndexEntity.MaximumLength);
  }
}
