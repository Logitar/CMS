using Logitar.Cms.Contracts.Fields;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record UpdateFieldTypeCommand(Guid Id, UpdateFieldTypePayload Payload) : Activity, IRequest<FieldType?>;
