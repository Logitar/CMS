﻿using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public class FieldType : Aggregate
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public DataType DataType { get; set; }
  public BooleanProperties? BooleanProperties { get; set; }
  public DateTimeProperties? DateTimeProperties { get; set; }
  public NumberProperties? NumberProperties { get; set; }
  public StringProperties? StringProperties { get; set; }
  public TextProperties? TextProperties { get; set; }

  public FieldType() : this(string.Empty)
  {
  }

  public FieldType(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
