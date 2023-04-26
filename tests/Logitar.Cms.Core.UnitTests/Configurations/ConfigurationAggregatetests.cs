using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Configurations;

[Trait(Traits.Category, Categories.Unit)]
public class ConfigurationAggregateTests
{
  [Theory]
  [InlineData("CmsConfig")]
  public void When_it_is_constructed_with_id_Then_it_has_correct_id(string id)
  {
    AggregateId aggregateId = new(id);
    ConfigurationAggregate configuration = new(aggregateId);

    Assert.Equal(aggregateId, configuration.Id);
  }
}
