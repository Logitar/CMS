using Logitar.Cms.Contracts.Fields;
using MediatR;

namespace Logitar.Cms.Core.Fields.Queries;

public record ReadFieldTypeQuery(Guid? Id, string? UniqueName) : IRequest<FieldType?>;
