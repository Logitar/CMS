using Logitar.Cms.Contracts.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Queries;

public record ReadFieldTypeQuery(Guid? Id, string? UniqueName) : IRequest<FieldType?>;
