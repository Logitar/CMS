using Logitar.Cms.Contracts.Contents;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record UpdateContentLocaleCommand(Guid Id, UpdateContentLocalePayload Payload, Guid? LanguageId) : Activity, IRequest<ContentItem?>;
