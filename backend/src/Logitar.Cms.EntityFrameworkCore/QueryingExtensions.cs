using Logitar.Cms.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdInFilter(this IQueryBuilder builder, ColumnId column, SearchPayload payload)
  {
    if (payload.Ids == null || payload.Ids.Count < 1)
    {
      return builder;
    }

    string[] aggregateIds = payload.Ids.Distinct().Select(id => new AggregateId(id).Value).ToArray();

    return builder.Where(column, Operators.IsIn(aggregateIds));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    if (payload.Skip.HasValue)
    {
      query = query.Skip(payload.Skip.Value);
    }

    if (payload.Limit.HasValue)
    {
      query = query.Take(payload.Limit.Value);
    }

    return query;
  }
}
