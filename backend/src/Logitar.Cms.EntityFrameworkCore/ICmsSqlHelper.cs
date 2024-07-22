using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;

namespace Logitar.Cms.EntityFrameworkCore;

public interface ICmsSqlHelper : ISqlHelper
{
  IUpdateBuilder Update(TableId table);
}
