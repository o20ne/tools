using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Runtime.Serialization;

[DataContractAttribute()]
internal class ChangeEntity
{
  public ChangeAction ChangeAction { get; set; }

  public EntityBase Entity { get; set; }
}