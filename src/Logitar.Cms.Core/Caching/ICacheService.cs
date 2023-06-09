﻿using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Caching;

public interface ICacheService
{
  ConfigurationAggregate? Configuration { get; set; }

  Actor? GetActor(AggregateId id);
  void SetActor(UserAggregate user);
}
