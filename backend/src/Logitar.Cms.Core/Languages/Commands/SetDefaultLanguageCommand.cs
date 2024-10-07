using Logitar.Cms.Contracts.Languages;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

public record SetDefaultLanguageCommand(Guid Id) : Activity, IRequest<Language?>;
