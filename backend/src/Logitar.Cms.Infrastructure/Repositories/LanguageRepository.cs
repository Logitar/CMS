﻿using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Repositories;

internal class LanguageRepository : Repository, ILanguageRepository
{
  public LanguageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(id.StreamId, version, cancellationToken);
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}