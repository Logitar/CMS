using Logitar.Cms.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.EntityFrameworkCore.Configurations;

internal class TextFieldIndexConfiguration : FieldIndexConfiguration<TextFieldIndexEntity, string>, IEntityTypeConfiguration<TextFieldIndexEntity>
{
  public override void Configure(EntityTypeBuilder<TextFieldIndexEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(CmsDb.TextFieldIndex.Table.Table ?? string.Empty, CmsDb.TextFieldIndex.Table.Schema);

    builder.Property(x => x.FieldIndexId).HasColumnName(CmsDb.TextFieldIndex.TextFieldIndexId.Name);
  }
}
