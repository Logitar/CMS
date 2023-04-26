using AutoMapper;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Profiles;

internal class SessionProfile : Profile
{
  public SessionProfile()
  {
    CreateMap<SessionEntity, Session>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => new AggregateId(y.AggregateId).ToGuid()))
      .ForMember(x => x.SignedOutBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.SignedOutById)));
  }
}
