using AutoMapper;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Profiles;

internal class LanguageProfile : Profile
{
  public LanguageProfile()
  {
    CreateMap<LanguageEntity, Language>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => new AggregateId(y.AggregateId).ToGuid()));
  }
}
