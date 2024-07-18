using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class StringFieldIndexConfiguration : FieldIndexConfiguration<StringFieldIndexEntity, string>, IEntityTypeConfiguration<StringFieldIndexEntity>
{
  public override void Configure(EntityTypeBuilder<StringFieldIndexEntity> builder)
  {
    builder.ToTable(CmsDb.StringFieldIndex.Table.Table ?? string.Empty, CmsDb.StringFieldIndex.Table.Schema);

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.StringFieldIndex.StringFieldIndexId.Name);
    builder.Property(x => x.Value).HasMaxLength(StringFieldIndexEntity.MaximumLength);
  }
}
