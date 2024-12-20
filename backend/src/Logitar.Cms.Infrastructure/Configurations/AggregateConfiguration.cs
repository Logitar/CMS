﻿using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Cms.Infrastructure.Configurations;

internal abstract class AggregateConfiguration<T> where T : AggregateEntity // TODO(fpion): refactor
{
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasIndex(x => x.StreamId).IsUnique();
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.StreamId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
