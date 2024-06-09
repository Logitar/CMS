using Logitar.Cms.Contracts.Search;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

public interface ISearchHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch? search, params ColumnId[] columns);
}
