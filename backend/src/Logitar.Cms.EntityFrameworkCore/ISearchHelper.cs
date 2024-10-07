using Logitar.Cms.Contracts.Search;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

public interface ISearchHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns);
}
