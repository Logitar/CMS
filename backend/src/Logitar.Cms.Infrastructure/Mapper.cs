using Logitar.Cms.Core;
using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Roles.Models;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Users.Models;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.Infrastructure;

public class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];
  private readonly ActorModel _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (ActorModel actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static ActorModel ToActor(ActorEntity actor) => new()
  {
    Id = new ActorId(actor.Id).ToGuid(),
    Type = Enum.Parse<ActorType>(actor.Type),
    IsDeleted = actor.IsDeleted,
    DisplayName = actor.DisplayName,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public ContentModel ToContent(ContentEntity source)
  {
    if (source.ContentType == null)
    {
      throw new ArgumentException($"The {nameof(source.ContentType)} is required.", nameof(source));
    }

    ContentModel destination = new()
    {
      ContentType = ToContentType(source.ContentType)
    };

    foreach (ContentLocaleEntity locale in source.Locales)
    {
      ContentLocaleModel invariantOrLocale = ToContentLocale(locale, destination);
      if (invariantOrLocale.Language == null)
      {
        destination.Invariant = invariantOrLocale;
      }
      else
      {
        destination.Locales.Add(invariantOrLocale);
      }
    }

    MapAggregate(source, destination);

    return destination;
  }

  public ContentLocaleModel ToContentLocale(ContentLocaleEntity source) => ToContentLocale(source, content: null);
  public ContentLocaleModel ToContentLocale(ContentLocaleEntity source, ContentModel? content)
  {
    if (content == null)
    {
      if (source.Content == null)
      {
        throw new ArgumentException($"The {nameof(source.Content)} is required.");
      }

      content = ToContent(source.Content);
    }
    if (source.LanguageId.HasValue && source.Language == null)
    {
      throw new ArgumentException($"The {nameof(source.Language)} is required.", nameof(source));
    }

    ContentLocaleModel destination = new(content)
    {
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      CreatedBy = TryFindActor(source.CreatedBy) ?? _system,
      CreatedOn = source.CreatedOn.AsUniversalTime(),
      UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system,
      UpdatedOn = source.UpdatedOn.AsUniversalTime(),
      IsPublished = source.IsPublished,
      PublishedBy = TryFindActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    if (source.Language != null)
    {
      destination.Language = ToLanguage(source.Language);
    }

    foreach (KeyValuePair<Guid, string> fieldValue in source.GetFieldValues())
    {
      destination.FieldValues.Add(new FieldValue(fieldValue));
    }

    return destination;
  }

  public ContentTypeModel ToContentType(ContentTypeEntity source)
  {
    ContentTypeModel destination = new()
    {
      IsInvariant = source.IsInvariant,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      FieldCount = source.FieldCount
    };

    foreach (FieldDefinitionEntity field in source.Fields)
    {
      destination.Fields.Add(ToFieldDefinition(field));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public FieldDefinitionModel ToFieldDefinition(FieldDefinitionEntity source)
  {
    if (source.FieldType == null)
    {
      throw new ArgumentException($"The {nameof(source.FieldType)} is required.", nameof(source));
    }

    return new FieldDefinitionModel
    {
      Id = source.Id,
      Order = source.Order,
      FieldType = ToFieldType(source.FieldType),
      IsInvariant = source.IsInvariant,
      IsRequired = source.IsRequired,
      IsIndexed = source.IsIndexed,
      IsUnique = source.IsUnique,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Placeholder = source.Placeholder
    };
  }

  public FieldTypeModel ToFieldType(FieldTypeEntity source)
  {
    FieldTypeModel destination = new()
    {
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      DataType = source.DataType
    };

    switch (source.DataType)
    {
      case DataType.Boolean:
        destination.Boolean = (source.Settings == null ? null : JsonSerializer.Deserialize<BooleanSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.DateTime:
        destination.DateTime = (source.Settings == null ? null : JsonSerializer.Deserialize<DateTimeSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.Number:
        destination.Number = (source.Settings == null ? null : JsonSerializer.Deserialize<NumberSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.RelatedContent:
        destination.RelatedContent = (source.Settings == null ? null : JsonSerializer.Deserialize<RelatedContentSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.RichText:
        destination.RichText = (source.Settings == null ? null : JsonSerializer.Deserialize<RichTextSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.Select:
        destination.Select = (source.Settings == null ? null : JsonSerializer.Deserialize<SelectSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.String:
        destination.String = (source.Settings == null ? null : JsonSerializer.Deserialize<StringSettingsModel>(source.Settings)) ?? new();
        break;
      case DataType.Tags:
        destination.Tags = (source.Settings == null ? null : JsonSerializer.Deserialize<TagsSettingsModel>(source.Settings)) ?? new();
        break;
      default:
        throw new DataTypeNotSupportedException(source.DataType);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public LanguageModel ToLanguage(LanguageEntity source)
  {
    LanguageModel destination = new()
    {
      IsDefault = source.IsDefault,
      Locale = new LocaleModel(source.Code)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public PublishedContentLocale ToPublishedContentLocale(PublishedContentEntity source, PublishedContent content, LanguageSummary? language)
  {
    PublishedContentLocale destination = new(content)
    {
      Language = language,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      PublishedBy = TryFindActor(source.PublishedBy) ?? _system,
      PublishedOn = source.PublishedOn.AsUniversalTime()
    };

    foreach (KeyValuePair<Guid, string> fieldValue in source.GetFieldValues())
    {
      destination.FieldValues.Add(new FieldValue(fieldValue));
    }

    return destination;
  }

  public RoleModel ToRole(RoleEntity source)
  {
    RoleModel destination = new(source.UniqueName)
    {
      Id = new EntityId(source.EntityId).ToGuid(),
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public SessionModel ToSession(SessionEntity source)
  {
    if (source.User == null)
    {
      throw new ArgumentException($"The {nameof(source.User)} is required.", nameof(source));
    }
    UserModel user = ToUser(source.User);

    SessionModel destination = new(user)
    {
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryFindActor(source.SignedOutBy),
      SignedOutOn = source.SignedOutOn?.AsUniversalTime()
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public UserModel ToUser(UserEntity source)
  {
    UserModel destination = new(source.UniqueName)
    {
      Id = new EntityId(source.EntityId).ToGuid(),
      HasPassword = source.HasPassword,
      PasswordChangedBy = TryFindActor(source.PasswordChangedBy),
      PasswordChangedOn = source.PasswordChangedOn?.AsUniversalTime(),
      DisabledBy = TryFindActor(source.DisabledBy),
      DisabledOn = source.DisabledOn?.AsUniversalTime(),
      IsDisabled = source.IsDisabled,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = source.Birthdate?.AsUniversalTime(),
      Gender = source.Gender,
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      AuthenticatedOn = source.AuthenticatedOn?.AsUniversalTime()
    };

    if (source.Locale != null)
    {
      destination.Locale = new LocaleModel(source.Locale);
    }

    if (source.AddressStreet != null && source.AddressLocality != null && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new AddressModel(source.AddressStreet, source.AddressLocality, source.AddressPostalCode, source.AddressRegion, source.AddressCountry, source.AddressFormatted)
      {
        IsVerified = source.IsAddressVerified,
        VerifiedBy = TryFindActor(source.AddressVerifiedBy),
        VerifiedOn = source.AddressVerifiedOn?.AsUniversalTime()
      };
    }
    if (source.EmailAddress != null)
    {
      destination.Email = new EmailModel(source.EmailAddress)
      {
        IsVerified = source.IsEmailVerified,
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = source.EmailVerifiedOn?.AsUniversalTime()
      };
    }
    if (source.PhoneNumber != null && source.PhoneE164Formatted != null)
    {
      destination.Phone = new PhoneModel(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted)
      {
        IsVerified = source.IsPhoneVerified,
        VerifiedBy = TryFindActor(source.PhoneVerifiedBy),
        VerifiedOn = source.PhoneVerifiedOn?.AsUniversalTime()
      };
    }

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    foreach (IdentifierEntity identifier in source.Identifiers)
    {
      destination.CustomIdentifiers.Add(new CustomIdentifierModel(identifier.Key, identifier.Value));
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role));
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Id = new StreamId(source.StreamId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private ActorModel? TryFindActor(string? id) => TryFindActor(id == null ? null : new ActorId(id));
  private ActorModel? TryFindActor(ActorId? actorId)
  {
    if (actorId.HasValue)
    {
      return _actors.TryGetValue(actorId.Value, out ActorModel? actor) ? actor : null;
    }

    return null;
  }
}
