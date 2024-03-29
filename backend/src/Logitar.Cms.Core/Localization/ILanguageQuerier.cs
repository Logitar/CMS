﻿using Logitar.Cms.Contracts.Localization;

namespace Logitar.Cms.Core.Localization;

public interface ILanguageQuerier
{
  Task<Language> ReadAsync(LanguageAggregate language, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(string locale, CancellationToken cancellationToken = default);
}
