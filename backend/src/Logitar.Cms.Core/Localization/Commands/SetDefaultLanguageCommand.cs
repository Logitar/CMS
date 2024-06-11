using Logitar.Cms.Contracts.Localization;
using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

public record SetDefaultLanguageCommand(Guid Id) : Activity, IRequest<Language?>;
