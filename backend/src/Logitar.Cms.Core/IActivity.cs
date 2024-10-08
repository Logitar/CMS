namespace Logitar.Cms.Core;

public interface IActivity
{
  IActivity Anonymize();
  void Contextualize(ActivityContext context);
}
