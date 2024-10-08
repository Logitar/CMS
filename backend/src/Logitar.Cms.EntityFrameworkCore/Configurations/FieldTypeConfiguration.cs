using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class FieldTypeConfiguration : AggregateConfiguration<FieldTypeEntity>, IEntityTypeConfiguration<FieldTypeEntity>
{
  public override void Configure(EntityTypeBuilder<FieldTypeEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.FieldTypes.Table.Table ?? string.Empty, CmsDb.FieldTypes.Table.Schema);
    builder.HasKey(x => x.FieldTypeId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.DataType);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.DataType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<DataType>());
  }
}
