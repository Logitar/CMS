using Logitar.Cms.Contracts.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

public record UpdateFieldTypeCommand(Guid Id, UpdateFieldTypePayload Payload) : Activity, IRequest<FieldType?>;
