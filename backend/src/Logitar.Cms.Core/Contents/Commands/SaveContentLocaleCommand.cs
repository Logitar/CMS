using Logitar.Cms.Contracts.Contents;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentLocaleCommand(Guid ContentId, Guid? LanguageId, SaveContentLocalePayload Payload) : Activity, IRequest<ContentItem?>;
