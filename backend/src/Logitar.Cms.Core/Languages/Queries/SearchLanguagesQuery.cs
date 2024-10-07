using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using MediatR;

namespace Logitar.Cms.Core.Languages.Queries;

public record SearchLanguagesQuery(SearchLanguagesPayload Payload) : Activity, IRequest<SearchResults<Language>>;
