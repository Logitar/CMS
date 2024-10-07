using Logitar.Cms.Contracts.Languages;
using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

public record CreateLanguageCommand(CreateLanguagePayload Payload) : Activity, IRequest<Language>;
