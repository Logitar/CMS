using Logitar.Cms.Contracts.Fields;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record ReplaceFieldTypeCommand(Guid Id, ReplaceFieldTypePayload Payload, long? Version) : Activity, IRequest<FieldType?>;
