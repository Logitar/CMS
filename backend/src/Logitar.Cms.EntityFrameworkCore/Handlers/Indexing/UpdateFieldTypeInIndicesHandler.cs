using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Indexing;

internal class UpdateFieldTypeInIndicesHandler : INotificationHandler<FieldTypeUpdatedEvent>
{
  private static readonly TableId[] _tables =
  [
    CmsDb.BooleanFieldIndex.Table, CmsDb.DateTimeFieldIndex.Table, CmsDb.NumberFieldIndex.Table,
    CmsDb.StringFieldIndex.Table, CmsDb.TextFieldIndex.Table, CmsDb.UniqueFieldIndex.Table
  ];

  private readonly CmsContext _context;
  private readonly ICmsSqlHelper _sqlHelper;

  public UpdateFieldTypeInIndicesHandler(CmsContext context, ICmsSqlHelper sqlHelper)
  {
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public async Task Handle(FieldTypeUpdatedEvent @event, CancellationToken cancellationToken)
  {
    string? uniqueName = @event.UniqueName?.Value;
    if (uniqueName != null)
    {
      string fieldTypeUid = @event.AggregateId.ToGuid().ToString();

      StringBuilder statement = new();
      IEnumerable<object>? parameters = null;
      foreach (TableId table in _tables)
      {
        ICommand command = _sqlHelper.Update(table)
          .Set(new Update(new ColumnId("FieldTypeName", table), uniqueName))
          .Where(new OperatorCondition(new ColumnId("FieldTypeUid", table), Operators.IsEqualTo(fieldTypeUid)))
          .Build();
        statement.Append(command.Text).Append(';').AppendLine().AppendLine();
        parameters ??= command.Parameters;
      }

      await _context.Database.ExecuteSqlRawAsync(statement.ToString(), parameters ?? [], cancellationToken);
    }
  }
}
