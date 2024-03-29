﻿using Logitar.Cms.Core.Shared;

namespace Logitar.Cms.Core.ContentTypes;

public interface IContentTypeRepository
{
  Task<ContentTypeAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ContentTypeAggregate?> LoadAsync(IdentifierUnit uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(ContentTypeAggregate contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentTypeAggregate> contentTypes, CancellationToken cancellationToken = default);
}
