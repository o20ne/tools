using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Runtime.Serialization;

[DataContractAttribute()]
public abstract class EntityBase
{
  internal virtual void OnSaving(ChangeAction changeAction) { }

  internal virtual void OnSaved() { }
}