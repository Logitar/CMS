﻿using Logitar.Cms.Contracts.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Queries;

public record ReadFieldTypeQuery(Guid? Id, string? UniqueName) : Activity, IRequest<FieldTypeModel?>;

internal class ReadFieldTypeQueryHandler : IRequestHandler<ReadFieldTypeQuery, FieldTypeModel?>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;

  public ReadFieldTypeQueryHandler(IFieldTypeQuerier fieldTypeQuerier)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
  }

  public async Task<FieldTypeModel?> Handle(ReadFieldTypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, FieldTypeModel> fieldTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      FieldTypeModel? fieldType = await _fieldTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (fieldType != null)
      {
        fieldTypes[fieldType.Id] = fieldType;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      FieldTypeModel? fieldType = await _fieldTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (fieldType != null)
      {
        fieldTypes[fieldType.Id] = fieldType;
      }
    }

    if (fieldTypes.Count > 1)
    {
      throw TooManyResultsException<FieldTypeModel>.ExpectedOne(fieldTypes.Count);
    }

    return fieldTypes.Values.SingleOrDefault();
  }
}
