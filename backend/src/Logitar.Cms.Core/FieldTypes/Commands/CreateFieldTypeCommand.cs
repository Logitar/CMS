using Logitar.Cms.Contracts.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

public record CreateFieldTypeCommand(CreateFieldTypePayload Payload) : Activity, IRequest<FieldType>;
