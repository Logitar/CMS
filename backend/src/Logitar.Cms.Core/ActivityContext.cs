using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core;

public record ActivityContext(Configuration Configuration, User? User, Session? Session);
