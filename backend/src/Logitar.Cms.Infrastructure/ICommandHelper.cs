using Logitar.Data;

namespace Logitar.Cms.Infrastructure;

public interface ICommandHelper
{
  IDeleteBuilder Delete(TableId table);
  IUpdateBuilder Update();
}
