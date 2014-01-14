using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;

public partial class DataClassesDataContext
{
  public override void SubmitChanges(ConflictMode failureMode)
  {
    // Get the entities that are to be inserted / updated / deleted
    ChangeSet changeSet = GetChangeSet();

    // Get a single list of all the entities in the change set
    IEnumerable<object> changeSetEntities = changeSet.Deletes;
    changeSetEntities = changeSetEntities.Union(changeSet.Inserts);
    changeSetEntities = changeSetEntities.Union(changeSet.Updates);

    // Get a single list of all the enitities that inherit from EntityBase
    IEnumerable<ChangeEntity> entities =
         from entity in changeSetEntities.Cast<EntityBase>()
         select new ChangeEntity()
         {
           ChangeAction =
                changeSet.Deletes.Contains(entity) ? ChangeAction.Delete
              : changeSet.Inserts.Contains(entity) ? ChangeAction.Insert
              : changeSet.Updates.Contains(entity) ? ChangeAction.Update
              : ChangeAction.None,
           Entity = entity as EntityBase
         };

    // "Raise" the OnSaving event for the entities
    foreach (ChangeEntity entity in entities)
    {
      entity.Entity.OnSaving(entity.ChangeAction);
    }

    // Save the changes
    base.SubmitChanges(failureMode);

    // "Raise" the OnSaved event for the entities
    foreach (ChangeEntity entity in entities)
    {
      entity.Entity.OnSaved();
    }
  }
}