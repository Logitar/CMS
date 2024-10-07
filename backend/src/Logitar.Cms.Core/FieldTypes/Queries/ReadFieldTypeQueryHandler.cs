using Logitar.Cms.Contracts.FieldTypes;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Queries;

internal class ReadFieldTypeQueryHandler : IRequestHandler<ReadFieldTypeQuery, FieldType?>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;

  public ReadFieldTypeQueryHandler(IFieldTypeQuerier fieldTypeQuerier)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
  }

  public async Task<FieldType?> Handle(ReadFieldTypeQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, FieldType> fieldTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      FieldType? fieldType = await _fieldTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (fieldType != null)
      {
        fieldTypes[fieldType.Id] = fieldType;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      FieldType? fieldtype = await _fieldTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (fieldtype != null)
      {
        fieldTypes[fieldtype.Id] = fieldtype;
      }
    }

    if (fieldTypes.Count > 1)
    {
      throw TooManyResultsException<FieldType>.ExpectedSingle(fieldTypes.Count);
    }

    return fieldTypes.Values.SingleOrDefault();
  }
}
