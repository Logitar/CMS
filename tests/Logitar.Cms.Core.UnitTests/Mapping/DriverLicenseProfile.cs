using AutoMapper;

namespace Logitar.Cms.Core.Mapping;

internal class DriverLicenseProfile : Profile
{
  public DriverLicenseProfile()
  {
    CreateMap<DriverLicenseEntity, DriverLicense>()
      .ForMember(x => x.RegisteredBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.RegisteredById)));
  }
}
