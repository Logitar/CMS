using Logitar.Cms.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.Search;

public record SearchParameters
{
  protected const string DescendingKeyword = "DESC";
  protected const char SortSeparator = '.';

  [FromQuery(Name = "ids")]
  public List<Guid>? Ids { get; set; }

  [FromQuery(Name = "search_terms")]
  public List<string>? SearchTerms { get; set; }

  [FromQuery(Name = "search_operator")]
  public SearchOperator SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public List<string>? Sort { get; set; }

  [FromQuery(Name = "skip")]
  public int? Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int? Limit { get; set; }

  protected void FillPayload(SearchPayload payload)
  {
    if (Ids != null)
    {
      payload.Ids.AddRange(Ids);
    }

    if (SearchTerms != null)
    {
      payload.Search.Terms.AddRange(SearchTerms.Select(value => new SearchTerm(value)));
      payload.Search.Operator = SearchOperator;
    }

    if (Sort != null)
    {
      foreach (string sort in Sort)
      {
        SortOption? option = ParseSortOption(sort);
        if (option != null)
        {
          payload.Sort.Add(option);
        }
      }
    }

    if (Skip.HasValue)
    {
      payload.Skip = Skip.Value;
    }

    if (Limit.HasValue)
    {
      payload.Limit = Limit.Value;
    }
  }

  protected SortOption ParseSortOption(string sort)
  {
    int index = sort.IndexOf(SortSeparator);
    if (index < 0)
    {
      return new SortOption(sort);
    }

    string field = sort[(index + 1)..];
    bool isDescending = sort[..index].Equals(DescendingKeyword, StringComparison.InvariantCultureIgnoreCase);
    return new SortOption(field, isDescending);
  }
}
