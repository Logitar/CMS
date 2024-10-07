using Logitar.Cms.Contracts.Languages;
using MediatR;

namespace Logitar.Cms.Core.Languages.Queries;

public record ReadLanguageQuery(Guid? Id, string? Locale, bool IsDefault) : Activity, IRequest<Language?>;
