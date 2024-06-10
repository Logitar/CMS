using Logitar.Cms.Contracts.Fields;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record CreateFieldTypeCommand(CreateFieldTypePayload Payload) : Activity, IRequest<FieldType>;
