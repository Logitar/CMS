using FluentValidation;
using Logitar.Cms.Contracts.Configurations;
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

  [Theory]
  [InlineData(40)]
  public void When_generating_a_secret_Then_it_has_correct_length(int length)
  {
    string secret = ConfigurationAggregate.GenerateSecret(length);
    Assert.Equal(length, secret.Length);
  }

  [Fact]
  public void When_it_is_constructed_with_default_arguments_Then_it_is_initialized()
  {
    ConfigurationAggregate configuration = new(actorId: AggregateId.NewId());

    Assert.NotNull(configuration.Secret);
    Assert.Equal(256 / 8, configuration.Secret.Length);

    Assert.NotNull(configuration.LoggingSettings);
    Assert.NotNull(configuration.UsernameSettings);
    Assert.NotNull(configuration.PasswordSettings);
  }

  [Fact]
  public void When_it_is_constructed_with_invalid_arguments_Then_ValidationException_is_thrown()
  {
    AggregateId actorId = AggregateId.NewId();
    string secret = ConfigurationAggregate.GenerateSecret(byte.MaxValue);
    ReadOnlyLoggingSettings loggingSettings = new()
    {
      Extent = LoggingExtent.None,
      OnlyErrors = true
    };
    ReadOnlyUsernameSettings usernameSettings = new() { AllowedCharacters = string.Empty };
    ReadOnlyPasswordSettings passwordSettings = new()
    {
      RequiredLength = 6,
      RequiredUniqueChars = 8
    };

    var exception = Assert.Throws<ValidationException>(()
      => new ConfigurationAggregate(actorId, secret, loggingSettings, usernameSettings, passwordSettings));

    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.PropertyName == "Secret");
    Assert.Contains(exception.Errors, e => e.PropertyName == "LoggingSettings.OnlyErrors");
    Assert.Contains(exception.Errors, e => e.PropertyName == "UsernameSettings.AllowedCharacters");
    Assert.Contains(exception.Errors, e => e.PropertyName == "PasswordSettings.RequiredUniqueChars");
  }

  [Fact]
  public void When_it_is_constructed_with_valid_arguments_Then_it_is_initialized()
  {
    AggregateId actorId = AggregateId.NewId();
    string secret = ConfigurationAggregate.GenerateSecret();
    ReadOnlyLoggingSettings loggingSettings = new();
    ReadOnlyUsernameSettings usernameSettings = new();
    ReadOnlyPasswordSettings passwordSettings = new();

    ConfigurationAggregate configuration = new(actorId, secret, loggingSettings, usernameSettings,
      passwordSettings);

    Assert.Equal(ConfigurationAggregate.GlobalId, configuration.Id);
    Assert.Equal(actorId, configuration.CreatedById);
    Assert.Equal(actorId, configuration.UpdatedById);
    Assert.Equal(secret, configuration.Secret);
    Assert.Same(loggingSettings, configuration.LoggingSettings);
    Assert.Same(usernameSettings, configuration.UsernameSettings);
    Assert.Same(passwordSettings, configuration.PasswordSettings);
  }
}
