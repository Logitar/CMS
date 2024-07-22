using Logitar.Cms.Contracts.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

public record ReplaceFieldTypeCommand(Guid Id, ReplaceFieldTypePayload Payload, long? Version) : Activity, IRequest<FieldType?>;
