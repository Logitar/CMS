using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record SaveFieldTypeCommand(FieldTypeAggregate FieldType) : IRequest;
