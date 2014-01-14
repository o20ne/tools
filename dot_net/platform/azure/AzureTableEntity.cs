using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalAzure
{
  public class AzureTableEntity : Microsoft.WindowsAzure.StorageClient.TableServiceEntity
  {
    public AzureTableEntity()
    {
      PartitionKey = "a";
      RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
    }

    public DateTime EventDateTime
    {
      get
      {
        return new DateTime(long.Parse(this.PartitionKey.Substring(1)));
      }
    }
  }
}