using Logitar.Cms.Contracts.Localization;
using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

public record CreateLanguageCommand(CreateLanguagePayload Payload) : Activity, IRequest<Language>;
