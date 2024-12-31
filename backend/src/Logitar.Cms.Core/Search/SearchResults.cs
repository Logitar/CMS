namespace Logitar.Cms.Core.Search;

public record SearchResults<T>
{
  public List<T> Items { get; set; } = [];
  public long Total { get; set; }

  public SearchResults()
  {
  }

  public SearchResults(long total) : this(items: [], total)
  {
  }

  public SearchResults(IEnumerable<T> items) : this(items, items.LongCount())
  {

  }

  public SearchResults(IEnumerable<T> items, long total)
  {
    Items.AddRange(items);
    Total = total;
  }
}
