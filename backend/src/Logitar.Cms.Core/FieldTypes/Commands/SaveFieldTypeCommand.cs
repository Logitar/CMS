using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

public record SaveFieldTypeCommand(FieldTypeAggregate FieldType) : IRequest;
