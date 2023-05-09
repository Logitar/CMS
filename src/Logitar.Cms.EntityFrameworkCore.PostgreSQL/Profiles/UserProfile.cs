using AutoMapper;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Profiles;

internal class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<UserEntity, User>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(y => new AggregateId(y.AggregateId).ToGuid()))
      .ForMember(x => x.PasswordChangedBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.PasswordChangedById)))
      .ForMember(x => x.DisabledBy, x => x.MapFrom((y, _, __, context) => context.GetActor(y.DisabledById)))
      .ForMember(x => x.Email, x => x.MapFrom(GetEmail));
  }

  private static Email? GetEmail(UserEntity user, User _, Email? __, ResolutionContext context)
  {
    return user.EmailAddress == null ? null : new Email
    {
      Address = user.EmailAddress,
      VerifiedBy = context.GetActor(user.EmailVerifiedById),
      VerifiedOn = user.EmailVerifiedOn,
      IsVerified = user.IsEmailVerified
    };
  }
}
