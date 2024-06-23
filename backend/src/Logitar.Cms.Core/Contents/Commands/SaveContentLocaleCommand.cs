using Logitar.Cms.Contracts.Contents;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentLocaleCommand(Guid Id, SaveContentLocalePayload Payload, Guid? LanguageId) : Activity, IRequest<ContentItem?>;
