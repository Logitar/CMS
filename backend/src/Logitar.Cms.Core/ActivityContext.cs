using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
namespace Logitar.Cms.Core;

public record ActivityContext(ConfigurationModel Configuration, ApiKeyModel? ApiKey, SessionModel? Session, UserModel? User);
