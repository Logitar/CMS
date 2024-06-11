using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Localization.Queries;

public record SearchLanguagesQuery(SearchLanguagesPayload Payload) : IRequest<SearchResults<Language>>;
