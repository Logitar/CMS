using AutoMapper;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Mapping;

namespace Logitar.Cms.Core.Configurations;

internal class ConfigurationProfile : Profile
{
  public ConfigurationProfile()
  {
    CreateMap<ConfigurationAggregate, Configuration>()
      .ForMember(x => x.CreatedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.CreatedById)))
      .ForMember(x => x.UpdatedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.UpdatedById)));
    CreateMap<ReadOnlyLoggingSettings, LoggingSettings>();
    CreateMap<ReadOnlyUsernameSettings, UsernameSettings>();
    CreateMap<ReadOnlyPasswordSettings, PasswordSettings>();
  }
}
