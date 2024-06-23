using Logitar.Cms.Contracts.Localization;
using MediatR;

namespace Logitar.Cms.Core.Localization.Queries;

public record ReadLanguageQuery(Guid? Id, string? Code, bool IsDefault) : IRequest<Language?>;
