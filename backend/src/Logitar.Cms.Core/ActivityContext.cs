using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core;

public record ActivityContext(Configuration Configuration, ApiKey? ApiKey, Session? Session, User? User);
