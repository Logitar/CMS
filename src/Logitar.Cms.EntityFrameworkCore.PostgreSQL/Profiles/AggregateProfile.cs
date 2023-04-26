using AutoMapper;
using Logitar.Cms.Contracts;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Profiles;

internal class AggregateProfile : Profile
{
  public AggregateProfile()
  {
    CreateMap<AggregateEntity, Aggregate>()
      .ForMember(x => x.CreatedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.CreatedById)))
      .ForMember(x => x.UpdatedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.UpdatedById ?? y.CreatedById)))
      .ForMember(x => x.UpdatedOn, x => x.MapFrom(y => y.UpdatedOn ?? y.CreatedOn));
  }
}
