using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class BooleanFieldIndexConfiguration : FieldIndexConfiguration<BooleanFieldIndexEntity, bool>, IEntityTypeConfiguration<BooleanFieldIndexEntity>
{
  public override void Configure(EntityTypeBuilder<BooleanFieldIndexEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.BooleanFieldIndex.Table.Table ?? string.Empty, CmsDb.BooleanFieldIndex.Table.Schema);

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.BooleanFieldIndex.BooleanFieldIndexId.Name);
  }
}
